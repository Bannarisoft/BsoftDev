using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using MediatR;

namespace Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup
{
    public class CreateDepreciationGroupCommand : IRequest<DepreciationGroupDTO> 
    {        
        public string? Code { get; set; }
        public string? DepreciationGroupName { get; set; } 
        public int BookType { get; set; }
        public int AssetGroupId { get; set; }
        public int DepreciationMethod { get; set; }
        public int UsefulLife { get; set; }
        public int ResidualValue { get; set; }
    }
}