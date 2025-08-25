using MediatR;

namespace Core.Application.Item.ItemDetail.Queries.GetItemAutoComplete
{
    public class GetItemAutoCompleteQuery : IRequest<List<GetItemAutoCompleteDto>>    
    {
        public string? SearchPattern { get; set; }       
    }
}