using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetDepreciationGroupAutoComplete
{
    public class GetDepreciationGroupAutoCompleteQuery  : IRequest<List<DepreciationGroupAutoCompleteDTO>>
    {
        public string? SearchPattern { get; set; }
    }
}