using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUser;
namespace Core.Application.Users.Queries.GetUserAutoComplete
{
    public class GetUserAutoCompleteQueryHandler: IRequestHandler<GetUserAutoCompleteQuery,List<UserAutoCompleteDto>>
    {
        private readonly IUserQueryRepository _userRepository;
        private readonly IMapper _mapper;
        public GetUserAutoCompleteQueryHandler(IUserQueryRepository userRepository, IMapper mapper)
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