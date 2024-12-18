using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Role.Queries.GetRolesAutocomplete
{
    public class GetRolesAutocompleteQueryHandler : IRequestHandler<GetRolesAutocompleteQuery, List<GetRolesAutocompleteVm>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRolesAutocompleteQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<GetRolesAutocompleteVm>> Handle(GetRolesAutocompleteQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetRolesAsync(request.SearchTerm);

            // Map to the application-specific DTO
            return roles.Select(r => new GetRolesAutocompleteVm
            {
                RoleId = r.RoleId,
                RoleName = r.Name
            }).ToList();
        }
        
    }
}