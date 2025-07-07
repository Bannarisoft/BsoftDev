using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMenu;
using MediatR;

namespace Core.Application.Menu.Queries.GetParentMenu
{
    public class GetParentMenuQueryHandler : IRequestHandler<GetParentMenuQuery, ApiResponseDTO<List<ParentMenuDto>>>
    {
        private readonly IMenuQuery _menuQuery;
        private readonly IMapper _mapper;
        
        public GetParentMenuQueryHandler(IMenuQuery menuQuery, IMapper mapper)
        {
            _menuQuery = menuQuery;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<ParentMenuDto>>> Handle(GetParentMenuQuery request, CancellationToken cancellationToken)
        {
            
             
            var result = await _menuQuery.GetParentMenuAutoComplete(request.SearchPattern);
            var MenuList = _mapper.Map<List<ParentMenuDto>>(result);
             
            return new ApiResponseDTO<List<ParentMenuDto>> { IsSuccess = true, Message = "Success", Data = MenuList };  
        }
    }
}