using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace BSOFT.Application.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, int>
    {
          private readonly IUnitRepository _unitRepository;

        public DeleteUnitCommandHandler(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
            
        }
        public async Task<int> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
            var Updateunit = new BSOFT.Domain.Entities.Unit()
            {
                UnitId = request.UnitId,
                IsActive = request.IsActive 
            };
            
            return await _unitRepository.DeleteAsync(request.UnitId,Updateunit);
        }
    }
}