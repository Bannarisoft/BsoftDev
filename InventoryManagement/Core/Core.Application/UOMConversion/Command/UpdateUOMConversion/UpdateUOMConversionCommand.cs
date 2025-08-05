using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.UOMConversion.Queries.GetAllUOMConversion;
using MediatR;

namespace Core.Application.UOMConversion.Command.UpdateUOMConversion
{
    public class UpdateUOMConversionCommand   : IRequest<ApiResponseDTO<UOMConversionDto>>
    {
         public int Id { get; set; }                
        public int FromUOMId { get; set; }         
        public int ToUOMId { get; set; }           
        public decimal ConversionValue { get; set; } 
        public byte IsActive { get; set; }
    }
}