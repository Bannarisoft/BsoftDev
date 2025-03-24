using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ShiftMasterDetailCQRS.Commands.UpdateShiftMasterDetail
{
    public class UpdateShiftMasterDetailCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public int ShiftMasterId { get; set; }
        public int UnitId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal DurationInHours { get; set; }
        public int BreakDurationInMinutes { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public int ShiftSupervisorId { get; set; }
        public byte IsActive { get; set; }
    }
}