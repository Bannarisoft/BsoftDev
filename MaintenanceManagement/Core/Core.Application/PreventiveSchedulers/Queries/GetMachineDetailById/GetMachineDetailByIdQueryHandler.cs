using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Queries.GetMachineDetailById
{
    public class GetMachineDetailByIdQueryHandler : IRequestHandler<GetMachineDetailByIdQuery, ApiResponseDTO<List<MachineDetailByHeaderIdDto>>>
    {
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
         private readonly IMapper _mapper;
         private readonly IDepartmentGrpcClient _departmentGrpcClient;
        public GetMachineDetailByIdQueryHandler(IPreventiveSchedulerQuery preventiveSchedulerQuery, IMapper mapper, IDepartmentGrpcClient departmentGrpcClient)
        {
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _mapper = mapper;
            _departmentGrpcClient = departmentGrpcClient;
        }
        public async Task<ApiResponseDTO<List<MachineDetailByHeaderIdDto>>> Handle(GetMachineDetailByIdQuery request, CancellationToken cancellationToken)
        {
               var preventiveScheduler = await _preventiveSchedulerQuery.GetDetailSchedulerByPreventiveScheduleId(request.Id);
            var preventiveSchedulerList = _mapper.Map<List<MachineDetailByHeaderIdDto>>(preventiveScheduler);
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

                foreach (var dto in preventiveSchedulerList)
               {
                   if (departmentLookup.TryGetValue(dto.DepartmentId, out var departmentName))
                   {
                       dto.DepartmentName = departmentName;
                   }
               }

                var filteredPreventiveSchedulers = preventiveSchedulerList
            .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
            .ToList();
          
            return new ApiResponseDTO<List<MachineDetailByHeaderIdDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = filteredPreventiveSchedulers
            };
        }
    }
}