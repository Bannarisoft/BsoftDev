using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetDepreciationGroupById
{
    public class GetDepreciationGroupByIdQuery : IRequest<DepreciationGroupDTO>
    {
        public int Id { get; set; }
    }
}