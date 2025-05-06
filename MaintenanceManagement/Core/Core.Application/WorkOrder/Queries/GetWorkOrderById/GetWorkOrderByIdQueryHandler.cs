
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, ApiResponseDTO<GetWorkOrderByIdDto>>
    {
        private readonly IWorkOrderQueryRepository _workOrderQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;   
        private readonly IIPAddressService _ipAddressService;
         private readonly IWorkOrderCommandRepository _workOrderRepository;      

        public GetWorkOrderByIdQueryHandler(IWorkOrderQueryRepository workOrderQueryRepository,  IMapper mapper, IMediator mediator, IIPAddressService ipAddressService,IWorkOrderCommandRepository workOrderRepository)
        {
            _workOrderQueryRepository =workOrderQueryRepository;
            _mapper =mapper;
            _mediator = mediator;           
            _ipAddressService = ipAddressService;
            _workOrderRepository = workOrderRepository; 
        }
        public async Task<ApiResponseDTO<GetWorkOrderByIdDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
        {          
            var (woResult, woActivity, woItem,woTechnician,woCheckList,woSchedule) = await _workOrderQueryRepository.GetWorkOrderByIdAsync(request.Id);
            if (woResult == null)
            {
                return new ApiResponseDTO<GetWorkOrderByIdDto>
                {
                    IsSuccess = false,
                    Message = $"WorkOrder with ID {request.Id} not found."
                };
            }

            var asset = _mapper.Map<GetWorkOrderByIdDto>(woResult);         
            if (woActivity != null)
            {
                asset.WOActivity  = _mapper.Map<List<GetWorkOrderActivityByIdDto>>(woActivity);
            }
            if (woItem != null)
            {
                asset.WOItem  = _mapper.Map<List<GetWorkOrderItemByIdDto>>(woItem);
               /* // 🔹 Path construction
                //string baseDirectory = "ItemImages"; // or fetch from DB
                 string baseDirectory =await _workOrderRepository.GetBaseDirectoryItemAsync();
                var companyId = _ipAddressService.GetCompanyId();
                var unitId = _ipAddressService.GetUnitId();
                var (companyName, unitName) = await _workOrderRepository.GetCompanyUnitAsync(companyId, unitId);
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory);

                foreach (var item in asset.WOItem)
                {
                    if (!string.IsNullOrEmpty(item.Image))
                    {
                        string imagePath = Path.Combine(uploadPath, companyName, unitName, item.Image);

                        if (File.Exists(imagePath))
                        {
                            byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
                            item.ImageBase64 = Convert.ToBase64String(imageBytes);
                        }
                    }
                } */
            }
            if (woTechnician != null)
            {
                asset.WOTechnician  = _mapper.Map<List<GetWorkOrderTechnicianByIdDto>>(woTechnician);
            }       
            if (woSchedule != null)
            {
                asset.WOSchedule  = _mapper.Map<List<GetWorkOrderScheduleByIdDto>>(woSchedule);
            }       
            if (woCheckList != null)
            {
                asset.WOCheckList  = _mapper.Map<List<GetWorkOrderCheckListByIdDto>>(woCheckList);
            }           
         
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode:"",        
                actionName: "",                
                details: $"Asset ",
                module:"AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<GetWorkOrderByIdDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = asset
            };       
        }      
    }
}