using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.UserRole.Queries.GetRolesAutocomplete
{
    public class GetRolesAutocompleteQueryHandler : IRequestHandler<GetRolesAutocompleteQuery, List<GetRolesAutocompleteVm>>
    {
        private readonly IUserRoleRepository _roleRepository;

        public GetRolesAutocompleteQueryHandler(IUserRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<GetRolesAutocompleteVm>> Handle(GetRolesAutocompleteQuery request, CancellationToken cancellationToken)
        {
            var userroles = await _roleRepository.GetRolesAsync(request.SearchTerm);

            // Map to the application-specific DTO
            return userroles.Select(r => new GetRolesAutocompleteVm
            {
                Id = r.Id,
                RoleName = r.RoleName
            }).ToList();
        }
        
    }
}