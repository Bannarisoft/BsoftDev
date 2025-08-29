using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IParty;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PartyMaster.Command.UpdatePartyMaster
{
    public class UpdatePartyMasterCommandHandler : IRequestHandler<UpdatePartyMasterCommand, bool>
    {
        private readonly IPartyMasterCommandRepository _partyMasterCommandRepository;
        private readonly IPartyMasterQueryRepository _ipartyMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILocationGrpcClient _locationGrpcClient;  // ✅ add this

        public UpdatePartyMasterCommandHandler(IPartyMasterCommandRepository partyMasterCommandRepository, IMapper mapper, IMediator mediator, IPartyMasterQueryRepository ipartyMasterQueryRepository, ILocationGrpcClient locationGrpcClient)
        {
            _partyMasterCommandRepository = partyMasterCommandRepository;
            _mapper = mapper;
            _mediator = mediator;
            _ipartyMasterQueryRepository = ipartyMasterQueryRepository;
            _locationGrpcClient = locationGrpcClient;
        }

        public async Task<bool> Handle(UpdatePartyMasterCommand request, CancellationToken cancellationToken)
        {
            // Fetch existing document IDs for this party from DB
            var existingDocIds = await _partyMasterCommandRepository
                .GetPartyDocumentIdsAsync(request.UpdatePartyMaster.Id);

            // Filter only new docs (not already in DB)
            var newDocuments = request.UpdatePartyMaster.PartyDocumentsUpdate?
                                .Where(d => !existingDocIds.Contains(d.DocumentId))
                                .ToList() ?? new List<UpdatePartyMasterDto.UpdatePartyDocumentDto>();
            if (newDocuments.Any())
            {
                string baseDirectory = await _ipartyMasterQueryRepository.GetBaseDirectoryAsync();
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory);
                EnsureDirectoryExists(uploadPath);

                foreach (var doc in newDocuments)
                {
                    if (string.IsNullOrWhiteSpace(doc.FileName) || doc.FileName == "string")
                        continue;

                    string oldFilePath = Path.Combine(uploadPath, doc.FileName);
                    if (File.Exists(oldFilePath))
                    {
                        // Rename file to PartyCode_DocId.ext
                        string newFileName = $"{request.UpdatePartyMaster.PartyCode}_{doc.DocumentId}{Path.GetExtension(oldFilePath)}";
                        string newFilePath = Path.Combine(uploadPath, newFileName);

                        File.Move(oldFilePath, newFilePath, overwrite: true);

                        doc.FileName = newFileName;
                        doc.UploadedDate = DateTime.Now;
                    }
                }
            }

               // Map DTO to Entity (Fix: Pass full DTO, not just Id)
            var partyEntity = _mapper.Map<Core.Domain.Entities.PartyMaster>(request.UpdatePartyMaster);

              // ------------------- Call gRPC Location Service -------------------
            if (request.UpdatePartyMaster.PartyAddressesUpdate != null && request.UpdatePartyMaster.PartyAddressesUpdate.Any())
            {
                for (int i = 0; i < request.UpdatePartyMaster.PartyAddressesUpdate.Count; i++)
                {
                    var addressDto = request.UpdatePartyMaster.PartyAddressesUpdate[i];
                    var addressEntity = partyEntity.PartyAddressTypes.ElementAt(i);

                    // Only call gRPC if City/State/Country is provided (update case)
                    if (!string.IsNullOrWhiteSpace(addressDto.City) &&
                        !string.IsNullOrWhiteSpace(addressDto.State) &&
                        !string.IsNullOrWhiteSpace(addressDto.Country))
                    {
                        var location = await _locationGrpcClient.GetOrCreateLocationAsync(
                            addressDto.City,
                            addressDto.State,
                            addressDto.Country
                        );

                        if (location == null)
                            throw new Exception("Location could not be resolved via gRPC.");

                        // Assign IDs to entity
                        addressEntity.CityId = location.CityId;
                        addressEntity.StateId = location.StateId;
                        addressEntity.CountryId = location.CountryId;
                    }
                    else
                    {
                        // Keep existing IDs if City/State/Country not provided
                        addressEntity.CityId = addressEntity.CityId;
                        addressEntity.StateId = addressEntity.StateId;
                        addressEntity.CountryId = addressEntity.CountryId;
                    }
                }
            }

            // Update main entity in repository
            var result = await _partyMasterCommandRepository.UpdateAsync(partyEntity.Id, partyEntity);

            if (!result)
                throw new ExceptionRules("PartyMaster update failed.");

            // Publish Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: partyEntity.Id.ToString(),
                actionName: partyEntity.PartyName ?? "NULL",
                details: "PartyMaster details were updated",
                module: "PartyMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return result;
        }
        
        private void EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }  
    }
}