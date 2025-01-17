using Core.Application.Common;
using Core.Application.Entity.Queries.GetEntity;
using MediatR;

namespace Core.Application.Entity.Queries.GetEntityAutoComplete
{
    public class GetEntityAutocompleteQuery : IRequest<Result<List<EntityDto>>>
    {
        public string SearchPattern { get; set; }
    }
}