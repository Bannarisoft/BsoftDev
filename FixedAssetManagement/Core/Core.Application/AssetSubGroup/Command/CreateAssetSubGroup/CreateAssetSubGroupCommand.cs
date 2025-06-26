using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubGroup.Command.CreateAssetSubGroup
{
    public class CreateAssetSubGroupCommand : IRequest<ApiResponseDTO<int>>
    {
        public string? Code { get; set; }
        public string? SubGroupName { get; set; }
        public int GroupId { get; set; }
        public decimal SubGroupPercentage { get; set; }       
        public byte AdditionalDepreciation { get; set; }    
    }
}