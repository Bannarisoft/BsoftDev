/* using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetComposite;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral
{
    public class CreateAssetCompositeCommandHandler : IRequestHandler<CreateAssetCompositeCommand, int>
    {
        private readonly IMapper _mapper;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IMediator _mediator;

        public CreateAssetCompositeCommandHandler(
            IMapper mapper,
            IAssetMasterGeneralCommandRepository assetMasterGeneralRepository,
            IMediator mediator)
        {
            _mapper = mapper;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreateAssetCompositeCommand request, CancellationToken cancellationToken)
        {
            // Begin a transaction (if your repository supports transactions)
            using var transaction = await _assetMasterGeneralRepository.BeginTransactionAsync(cancellationToken);
            try
            {
                // Map the AssetMaster portion of the DTO to your domain entity
                var assetMaster = _mapper.Map<AssetMasterGenerals>(request.AssetComposite.AssetMaster);

                // Set the asset status based on the SaveAsDraft flag
                 assetMaster.AssetStatus = request.AssetComposite.SaveAsDraft ? AssetStatus.Draft : AssetStatus.Active;

                // Add the AssetMaster entity
                await _assetMasterGeneralRepository.AddAsync(assetMaster, cancellationToken);

                // When not saving as a draft, add detail records as well
                if (!request.AssetComposite.SaveAsDraft)
                {
                    // Map and add AssetSpecifications (if any)
                    if (request.AssetComposite.AssetSpecifications is not null)
                    {
                        foreach (var specDto in request.AssetComposite.AssetSpecifications)
                        {
                            var spec = _mapper.Map<AssetSpecifications>(specDto);
                            spec.AssetId = assetMaster.Id; // Use generated AssetId
                            // Add the specification to the navigation collection
                            assetMaster.AssetSpecification.Add(spec);
                        }
                    }

                    // Map and add AssetWarranties (if any)
                    if (request.AssetComposite.AssetWarranties is not null)
                    {
                        foreach (var warrantyDto in request.AssetComposite.AssetWarranties)
                        {
                            var warranty = _mapper.Map<AssetWarranties>(warrantyDto);
                            warranty.AssetId = assetMaster.Id;
                            assetMaster.AssetWarranty.Add(warranty);
                        }
                    }

                    // Similarly, map and add any other detail entities (e.g., AssetAdditionalCosts)
                }

                // Save changes to persist both the master and related details
                await _assetMasterGeneralRepository.SaveChangesAsync(cancellationToken);

                // Commit the transaction if everything is successful
                await transaction.CommitAsync(cancellationToken);

                // Optionally, publish a domain event to notify other parts of your system
                await _mediator.Publish(new AssetCreatedEvent(assetMaster.Id), cancellationToken);

                return assetMaster.Id;
            }
            catch (Exception)
            {
                // Rollback the transaction on error
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
} */