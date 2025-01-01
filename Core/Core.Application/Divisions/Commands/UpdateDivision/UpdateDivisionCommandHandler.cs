using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Divisions.Commands.UpdateDivision
{
    public class UpdateDivisionCommandHandler : IRequestHandler<UpdateDivisionCommand, int>
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IMapper _imapper;
        public UpdateDivisionCommandHandler(IDivisionRepository divisionRepository,IMapper imapper)
        {
            _divisionRepository =divisionRepository;
            _imapper =imapper;
        }
          public async Task<int> Handle(UpdateDivisionCommand request, CancellationToken cancellationToken)
        {
            var division  = _imapper.Map<Division>(request);
         
            var divisionresult = await _divisionRepository.UpdateAsync(request.Id, division);
            return divisionresult;
        }
        
    }
}