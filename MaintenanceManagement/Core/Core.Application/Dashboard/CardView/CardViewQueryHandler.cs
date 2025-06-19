using Core.Application.Common.Interfaces.IDashboard;
using Core.Application.Dashboard.CardView;
using Core.Application.Dashboard.Common;
using Core.Application.Dashboard.DashboardQuery;
using MediatR;

public class CardViewQueryHandler : IRequestHandler<CardViewQuery, CardViewDto>
{
    private readonly IDashboardQueryRepository _repository;

    public CardViewQueryHandler(IDashboardQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CardViewDto> Handle(CardViewQuery request, CancellationToken cancellationToken)
    {       
        return await _repository.GetCardDashboardAsync(
            request.FromDate,
            request.ToDate,
            "CardView",
            request.DepartmentId,
            request.MachineGroupId
        );
    }
}
