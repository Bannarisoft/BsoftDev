using AutoMapper;
using Core.Application.Budget.Commands.CreateBudget;
using Core.Application.Budget.Commands.UpdateBudget;
using Core.Domain.Entities.Budget;

namespace Core.Application.Budget.Mapping
{
    public class BudgetMappingProfile : Profile
    {
        public BudgetMappingProfile()
        {
            // ✅ Map Create Command to BudgetMaster
            CreateMap<CreateBudgetCommand, BudgetMaster>()
                .ForMember(dest => dest.BudgetDetail, opt => opt.MapFrom(src => src.BudgetDetails));

            // ✅ Map BudgetDetailDto to BudgetDetail entity
            CreateMap<BudgetDetailDto, BudgetDetail>();

            // ✅ Map Update Command details
            CreateMap<UpdateBudgetDetailDto, BudgetDetail>()
                .ForMember(dest => dest.BudgetAmount, opt => opt.MapFrom(src => src.NewAmount));
        }
    }
}
