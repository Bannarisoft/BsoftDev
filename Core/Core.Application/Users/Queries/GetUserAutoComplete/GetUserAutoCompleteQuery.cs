using MediatR;
using Core.Application.Users.Queries.GetUsers;

namespace Core.Application.Users.Queries.GetUserAutoComplete
{
    public class GetUserAutoCompleteQuery: IRequest<List<UserDto>>
    {
        public string SearchPattern { get; set; }
    }
}