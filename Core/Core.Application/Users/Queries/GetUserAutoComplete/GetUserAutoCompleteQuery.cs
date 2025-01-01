using MediatR;

namespace Core.Application.Users.Queries.GetUserAutoComplete
{
    public class GetUserAutoCompleteQuery: IRequest<List<UserAutoCompleteDto>>
    {
        public string SearchPattern { get; set; }
    }
}