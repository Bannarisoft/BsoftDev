using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ShiftMasters.Commands.UpdateShiftMaster
{
    public class UpdateShiftMasterCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public byte IsActive { get; set; }
    }
}