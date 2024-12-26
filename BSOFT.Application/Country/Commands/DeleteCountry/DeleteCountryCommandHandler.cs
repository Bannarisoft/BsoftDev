using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Common.Interface;
using BSOFT.Domain.Entities;
using MediatR;
using static BSOFT.Application.Country.Commands.DeleteCountryCommand;

namespace BSOFT.Application.Country.Commands.Delete
{
  public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, int>
    {
        private readonly ICountryRepository _repository;

        public DeleteCountryCommandHandler(ICountryRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
                        var CountryUpdate = new Countries()
            {
                Id = request.Id,
                IsActive = request.IsActive 
            };
            return await _repository.DeleteAsync(request.Id,CountryUpdate);

                // var country = await _repository.GetByIdAsync(request.Id);
                // if (country == null)
                // {
                //     return 0; // No country found
                // }                
                // country.IsActive = request.IsActive;                   
                // return await _repository.UpdateAsync(country.Id, country);
        }
    }
}