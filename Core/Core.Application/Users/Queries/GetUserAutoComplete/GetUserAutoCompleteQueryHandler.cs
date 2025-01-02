using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Domain.Entities;
using Core.Application.Users.Queries.GetUsers;

namespace Core.Application.Users.Queries.GetUserAutoComplete
{
    public class GetUserAutoCompleteQueryHandler: IRequestHandler<GetUserAutoCompleteQuery,List<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetUserAutoCompleteQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
           _userRepository =userRepository;
           _mapper =mapper;
        }  
        
       public async Task<List<UserDto>> Handle(GetUserAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.SearchPattern))
            {
                throw new ArgumentNullException(nameof(request.SearchPattern), "SearchPattern is null or empty");
            }

            var users = await _userRepository.GetByUsernameAsync(request.SearchPattern);
            return new List<UserDto> { _mapper.Map<UserDto>(users) };
            // var user = await _userRepository.GetByUsernameAsync(request.SearchPattern);
            // var userList = new List<UserAutoCompleteDto>
            // {
            //     new UserAutoCompleteDto
            //     {
            //         UserId = user.UserId,
            //         UserName = user.UserName
            //     }
            // };

            // return userList;
        }
    }
}