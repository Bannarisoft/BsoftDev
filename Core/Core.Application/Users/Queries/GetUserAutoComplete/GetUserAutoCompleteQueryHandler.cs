using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Domain.Entities;
namespace Core.Application.Users.Queries.GetUserAutoComplete
{
    public class GetUserAutoCompleteQueryHandler: IRequestHandler<GetUserAutoCompleteQuery,List<UserAutoCompleteDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetUserAutoCompleteQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
           _userRepository =userRepository;
           _mapper =mapper;
        }  
        
       public async Task<List<UserAutoCompleteDto>> Handle(GetUserAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.SearchPattern);
            var userList = new List<UserAutoCompleteDto>
            {
                new UserAutoCompleteDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName
                }
            };

            return userList;
        }
    }
}