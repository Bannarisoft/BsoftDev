using AutoMapper;
using Core.Application.Item.PutAway.Commands.CreatePutAwayRule;
using Core.Application.Item.PutAway.Queries.GetAllPutAwayRule;
using Core.Domain.Entities.Item.PutAway;

namespace Core.Application.Common.Mappings.Item
{
    public sealed class PutAwayProfile : Profile
    {
        public PutAwayProfile()
        {
            CreateMap<PutAwayRule, PutAwayRuleDetailDto>();
            CreateMap<PutAwayStrategy, PutAwayStrategyDto>();

            CreateMap<CreatePutAwayRuleRequest, PutAwayRule>();
            CreateMap<CreatePutAwayStrategyRequest, PutAwayStrategy>();
        }
    }
}