using BSOFT.Application.Common.Interfaces;
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
                Id=request.UnitId,
                UnitName = request.UnitName,
                ShortName = request.ShortName,
              
                
                IsActive = request.IsActive
                    
            };

            return await _unitRepository.UpdateAsync(request.UnitId, UpdateunitEntity);
        }
    }
}