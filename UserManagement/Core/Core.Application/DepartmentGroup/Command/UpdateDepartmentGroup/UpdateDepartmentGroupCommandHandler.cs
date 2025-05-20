using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepartmentGroup;
using MediatR;

namespace Core.Application.DepartmentGroup.Command.UpdateDepartmentGroup
{
    public class UpdateDepartmentGroupCommandHandler : IRequestHandler<UpdateDepartmentGroupCommand, ApiResponseDTO<int>>
    {
        private readonly IDepartmentGroupCommandRepository _departmentGroupCommandRepository;
        private readonly IMapper _mapper;

        private readonly IDepartmentGroupQueryRepository _departmentGroupQueryRepository;


        public UpdateDepartmentGroupCommandHandler(IDepartmentGroupCommandRepository departmentGroupCommandRepository, IMapper mapper, IDepartmentGroupQueryRepository departmentGroupQueryRepository)
        {
            _departmentGroupCommandRepository = departmentGroupCommandRepository;
            _mapper = mapper;
            _departmentGroupQueryRepository = departmentGroupQueryRepository;
        }
        
         public async Task<ApiResponseDTO<int>> Handle(UpdateDepartmentGroupCommand request, CancellationToken cancellationToken)
        {
            var existing = await _departmentGroupQueryRepository.GetDepartmentGroupByIdAsync(request.Id);
            if (existing == null)
            {
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "DepartmentGroup not found.",
                    Data = 0
                };
            }

            // Update fields
            existing.DepartmentGroupCode = request.DepartmentGroupCode;
            existing.DepartmentGroupName = request.DepartmentGroupName;            
            existing.IsActive = request.IsActive;

            var rows = await _departmentGroupCommandRepository.UpdateAsync(request.Id, existing);


            return new ApiResponseDTO<int>
            {
                 IsSuccess = rows,
                 Message = rows ? "DepartmentGroup updated successfully." : "Update failed.",
                 Data = rows ? 1 : 0
            };
        }
    }
}