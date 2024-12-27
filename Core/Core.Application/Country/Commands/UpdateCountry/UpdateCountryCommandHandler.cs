using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interface;
using MediatR;
using Core.Domain.Entities;
using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using Core.Application.Country.Commands.UpdateCountry;

namespace Core.Application.Country.Commands.Update
{    
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, int>
    {
        private readonly ICountryRepository _repository;
        private readonly IMapper _mapper;

        public UpdateCountryCommandHandler(ICountryRepository repository, IMapper mapper)
        {
            _repository = repository;
             _mapper = mapper;
        }

        public async Task<int> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
                var country = await _repository.GetByIdAsync(request.Id);
                if (country == null)
                {
                    return 0; // No country found
         
                }     

                 var updateCountryEntity = _mapper.Map<Countries>(request);
                  return await _repository.UpdateAsync(request.Id, updateCountryEntity);           
        }
    }
}