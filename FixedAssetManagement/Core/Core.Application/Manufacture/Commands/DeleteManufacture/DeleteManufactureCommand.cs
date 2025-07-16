using Core.Application.Common.HttpResponse;
using Core.Application.Manufacture.Queries.GetManufacture;
using MediatR;

namespace Core.Application.Manufacture.Commands.DeleteManufacture
{
    public class DeleteManufactureCommand :  IRequest<ApiResponseDTO<ManufactureDTO>>  
    {
        public int Id { get; set; }  
    }
}