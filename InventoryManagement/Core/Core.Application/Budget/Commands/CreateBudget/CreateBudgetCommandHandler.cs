using MediatR;
using Core.Domain.Entities.Budget;
using Core.Application.Budget.Commands.CreateBudget;
using Core.Application.Common.Interfaces.Budget;
using AutoMapper;

namespace Core.Application.Features.Budget.Commands.CreateBudget
{
    public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, int>
    {
        private readonly IBudgetCommandRepository _budgetRepository;
        private readonly IMapper _mapper;

        public CreateBudgetCommandHandler(IBudgetCommandRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<BudgetMaster>(request);
            var result = await _budgetRepository.CreateBudgetAsync(entity);
            return result;
        }
    }
}
