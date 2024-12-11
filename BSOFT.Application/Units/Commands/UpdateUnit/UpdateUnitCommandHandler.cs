using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, int>
    {
       private readonly IUnitRepository _unitRepository;
       public UpdateUnitCommandHandler(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }  

        public async Task<int> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            var UpdateunitEntity = new BSOFT.Domain.Entities.Unit()
            {
                UnitId=request.UnitId,
                Name = request.Name,
                ShortName = request.ShortName,
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                CoId = request.CoId,
                DivId = request.DivId,
                UnitHeadName = request.UnitHeadName,
                Mobile = request.Mobile,
                Email = request.Email,
                IsActive = request.IsActive,
                ModifiedBy=request.ModifiedBy,
                ModifiedByName=request.ModifiedByName,
                ModifiedAt=request.ModifiedAt,
                ModifiedIP=request.ModifiedIP       
            };

            return await _unitRepository.UpdateAsync(request.UnitId, UpdateunitEntity);
        }
    }
}