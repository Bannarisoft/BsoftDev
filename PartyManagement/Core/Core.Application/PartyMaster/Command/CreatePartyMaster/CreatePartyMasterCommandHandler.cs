using AutoMapper;
using Contracts.Interfaces.External.IParty;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PartyMaster.Command.CreatePartyMaster
{
    public class CreatePartyMasterCommandHandler : IRequestHandler<CreatePartyMasterCommand, int>
    {
        private readonly IPartyMasterCommandRepository _partyMasterCommandRepository;
        private readonly IPartyMasterQueryRepository _ipartyMasterQueryRepository;
        private readonly IPartyActivityLogCommandRepository _ipartyActivityLogCommandRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ILocationGrpcClient _locationGrpcClient;  // ✅ add this

        public CreatePartyMasterCommandHandler(IPartyMasterCommandRepository partyMasterCommandRepository, IMapper mapper, IMediator mediator, IPartyMasterQueryRepository ipartyMasterQueryRepository, IPartyActivityLogCommandRepository ipartyActivityLogCommandRepository, ILocationGrpcClient locationGrpcClient)
        {
            _partyMasterCommandRepository = partyMasterCommandRepository;
            _mapper = mapper;
            _mediator = mediator;
            _ipartyMasterQueryRepository = ipartyMasterQueryRepository;
            _ipartyActivityLogCommandRepository = ipartyActivityLogCommandRepository;
            _locationGrpcClient = locationGrpcClient;
        }

        // public async Task<int> Handle(CreatePartyMasterCommand request, CancellationToken cancellationToken)
        // {

        //     var dto = request.PartyMaster;
        //     // Remove Address entries from Swagger placeholders
        //     if (dto.PartyAddresses?.Any() == true)
        //     {

        //         dto.PartyAddresses = dto.PartyAddresses
        //             .Where(c =>
        //                 !(
        //                     string.Equals(c.AddressType?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
        //                     string.Equals(c.AddressLine1?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
        //                     string.Equals(c.City?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
        //                     string.Equals(c.State?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
        //                     string.Equals(c.PostalCode?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
        //                     string.Equals(c.Country?.Trim(), "string", StringComparison.OrdinalIgnoreCase)

        //                 )
        //             )
        //             .ToList();

        //         if (!dto.PartyAddresses.Any())
        //             dto.PartyAddresses = null;
        //     }






        //     // Remove contact entries from Swagger placeholders
        //     if (dto.PartyContacts != null)
        //     {
        //         dto.PartyContacts = dto.PartyContacts
        //             .Where(c =>
        //                 !string.Equals(c.FirstName, "string", StringComparison.OrdinalIgnoreCase) &&
        //                 !string.Equals(c.MobileNo, "string", StringComparison.OrdinalIgnoreCase) &&
        //                 !string.Equals(c.ContactBy, "string", StringComparison.OrdinalIgnoreCase)

        //             )
        //             .ToList();

        //         if (!dto.PartyContacts.Any())
        //             dto.PartyContacts = null;
        //     }

        //     // Remove bank entries from Swagger placeholders
        //     if (dto.PartyBanks != null)
        //     {
        //         dto.PartyBanks = dto.PartyBanks
        //             .Where(b =>
        //                 !string.Equals(b.BankName, "string", StringComparison.OrdinalIgnoreCase) &&
        //                 !string.Equals(b.BankAccountNumber, "string", StringComparison.OrdinalIgnoreCase) &&
        //                 !string.Equals(b.BankBranch, "string", StringComparison.OrdinalIgnoreCase) &&
        //                 !string.Equals(b.IFSCCode, "string", StringComparison.OrdinalIgnoreCase) &&
        //                 b.AccountTypeId != 0

        //             )
        //             .ToList();

        //         if (!dto.PartyBanks.Any())
        //             dto.PartyBanks = null;
        //     }


        //    //  Get next PartyCode before mapping
        //     var nextPartyCode = await _partyMasterCommandRepository.GetNextPartyCodeAsync();

        //     // Filter & rename files using the generated PartyCode
        //     if (dto.PartyDocuments != null && dto.PartyDocuments.Any())
        //     {
        //         dto.PartyDocuments = dto.PartyDocuments
        //             .Where(d => d.DocumentId != 0 &&
        //                         !string.Equals(d.FileName, "string", StringComparison.OrdinalIgnoreCase))
        //             .ToList();

        //         if (dto.PartyDocuments.Any())
        //         {
        //             string baseDirectory = await _ipartyMasterQueryRepository.GetBaseDirectoryAsync();
        //             string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory);
        //             EnsureDirectoryExists(uploadPath);

        //             foreach (var doc in dto.PartyDocuments)
        //             {
        //                 if (string.IsNullOrWhiteSpace(doc.FileName))
        //                     continue;

        //                 string oldFilePath = Path.Combine(uploadPath, doc.FileName);
        //                 if (File.Exists(oldFilePath))
        //                 {
        //                     string newFileName = $"{nextPartyCode}_{doc.DocumentId}{Path.GetExtension(oldFilePath)}";
        //                     string newFilePath = Path.Combine(uploadPath, newFileName);

        //                     try
        //                     {
        //                         File.Move(oldFilePath, newFilePath, overwrite: true);
        //                         doc.FileName = newFileName;          // ✅ updated in DTO
        //                         doc.UploadedDate = DateTimeOffset.UtcNow; // ✅ updated in DTO
        //                     }
        //                     catch (Exception ex)
        //                     {
        //                         throw new InvalidOperationException(
        //                             $"File upload failed while renaming '{doc.FileName}' to '{newFileName}': {ex.Message}", ex);
        //                     }
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             dto.PartyDocuments = null;
        //         }
        //     }

        //     // Now map after renaming so AutoMapper picks up new names & dates
        //     var partyMasterEntity = _mapper.Map<Core.Domain.Entities.PartyMaster>(dto);

        //     // Assign the auto-generated PartyCode to the entity
        //     partyMasterEntity.PartyCode = nextPartyCode;



        //     // Save in DB using repository
        //     var result = await _partyMasterCommandRepository.CreateAsync(partyMasterEntity);

        //     // Publish audit log event
        //     var domainEvent = new AuditLogsDomainEvent(
        //         actionDetail: "Create",
        //         actionCode: partyMasterEntity.PartyCode ?? "NULL",
        //         actionName: partyMasterEntity.PartyName ?? "NULL",
        //         details: "PartyMaster details created",
        //         module: "PartyMaster"
        //     );

        //     await _mediator.Publish(domainEvent, cancellationToken);

        //     if (result > 0)
        //     {

        //         // ✅ Insert into PartyActivityLog immediately
        //         await _partyMasterCommandRepository.LogChange(partyMasterEntity.Id, "PartyMaster", "PartyName", "", partyMasterEntity.PartyName ?? " ", "Insert");  

        //     }
        //     else
        //     {
        //         throw new ExceptionRules("PartyMaster creation failed.");
        //     }

        //     return result;
        //     }
        
        public async Task<int> Handle(CreatePartyMasterCommand request, CancellationToken cancellationToken)
        {
            var dto = request.PartyMaster;

            // ------------------- Clean Address placeholders -------------------
            if (dto.PartyAddresses?.Any() == true)
            {
                dto.PartyAddresses = dto.PartyAddresses
                    .Where(c =>
                        !(
                            string.Equals(c.AddressType?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(c.AddressLine1?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(c.City?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(c.State?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(c.PostalCode?.Trim(), "string", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(c.Country?.Trim(), "string", StringComparison.OrdinalIgnoreCase)
                        )
                    )
                    .ToList();

                if (!dto.PartyAddresses.Any())
                    dto.PartyAddresses = null;
            }

           

            // ------------------- Clean Contacts -------------------
            if (dto.PartyContacts != null)
            {
                dto.PartyContacts = dto.PartyContacts
                    .Where(c =>
                        !string.Equals(c.FirstName, "string", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(c.MobileNo, "string", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(c.ContactBy, "string", StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();

                if (!dto.PartyContacts.Any())
                    dto.PartyContacts = null;
            }

            // ------------------- Clean Banks -------------------
            if (dto.PartyBanks != null)
            {
                dto.PartyBanks = dto.PartyBanks
                    .Where(b =>
                        !string.Equals(b.BankName, "string", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(b.BankAccountNumber, "string", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(b.BankBranch, "string", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(b.IFSCCode, "string", StringComparison.OrdinalIgnoreCase) &&
                        b.AccountTypeId != 0
                    )
                    .ToList();

                if (!dto.PartyBanks.Any())
                    dto.PartyBanks = null;
            }

            // ------------------- PartyCode Generation -------------------
            var nextPartyCode = await _partyMasterCommandRepository.GetNextPartyCodeAsync();

            // ------------------- Rename Uploaded Docs -------------------
            if (dto.PartyDocuments != null && dto.PartyDocuments.Any())
            {
                dto.PartyDocuments = dto.PartyDocuments
                    .Where(d => d.DocumentId != 0 &&
                                !string.Equals(d.FileName, "string", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (dto.PartyDocuments.Any())
                {
                    string baseDirectory = await _ipartyMasterQueryRepository.GetBaseDirectoryAsync();
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory);
                    EnsureDirectoryExists(uploadPath);

                    foreach (var doc in dto.PartyDocuments)
                    {
                        if (string.IsNullOrWhiteSpace(doc.FileName))
                            continue;

                        string oldFilePath = Path.Combine(uploadPath, doc.FileName);
                        if (File.Exists(oldFilePath))
                        {
                            string newFileName = $"{nextPartyCode}_{doc.DocumentId}{Path.GetExtension(oldFilePath)}";
                            string newFilePath = Path.Combine(uploadPath, newFileName);

                            try
                            {
                                File.Move(oldFilePath, newFilePath, overwrite: true);
                                doc.FileName = newFileName;
                                doc.UploadedDate = DateTimeOffset.UtcNow;
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException(
                                    $"File upload failed while renaming '{doc.FileName}' to '{newFileName}': {ex.Message}", ex);
                            }
                        }
                    }
                }
                else
                {
                    dto.PartyDocuments = null;
                }
            }

            // ------------------- Map to Entity -------------------
            var partyMasterEntity = _mapper.Map<Core.Domain.Entities.PartyMaster>(dto);

            // Assign generated PartyCode
            partyMasterEntity.PartyCode = nextPartyCode;


             // ------------------- Call gRPC Location Service -------------------
          if (partyMasterEntity.PartyAddressTypes != null && partyMasterEntity.PartyAddressTypes.Any())
            {
                // Assuming same order: dto.PartyAddresses[i] → entity.PartyAddressTypes[i]
                for (int i = 0; i < dto.PartyAddresses.Count; i++)
                {
                    var addressDto = dto.PartyAddresses[i];
                    var addressEntity = partyMasterEntity.PartyAddressTypes.ElementAt(i);

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

                        // Assign IDs directly to the matching entity address
                        addressEntity.CityId = location.CityId;
                        addressEntity.StateId = location.StateId;
                        addressEntity.CountryId = location.CountryId;
                    }
                }
            }


            // ------------------- Save to DB -------------------
            var result = await _partyMasterCommandRepository.CreateAsync(partyMasterEntity);

            // ------------------- Audit Logs -------------------
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: partyMasterEntity.PartyCode ?? "NULL",
                actionName: partyMasterEntity.PartyName ?? "NULL",
                details: "PartyMaster details created",
                module: "PartyMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            if (result > 0)
            {
            var log = new PartyActivityLog
            {
                PartyId = partyMasterEntity.Id,
                TableName = "PartyMaster",
                ColumnName = "PartyName",
                OldValue = "",
                NewValue = partyMasterEntity.PartyName ?? string.Empty,
                ActionType = "Insert",
                ChangedBy = partyMasterEntity.CreatedBy,
                ChangedByName=partyMasterEntity.CreatedByName ?? string.Empty,
                ChangedIp=partyMasterEntity.CreatedIP?? string.Empty,
                ChangedOn = DateTimeOffset.UtcNow
            };

                await _ipartyActivityLogCommandRepository.InsertAsync(log, cancellationToken);
            }
            else
            {
                throw new ExceptionRules("PartyMaster creation failed.");
            }

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