
using Core.Application.Common.Interfaces.IState;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;
using MassTransit;

namespace UserManagement.API.GrpcServices
{
    public class StatesGrpcService : StateService.StateServiceBase
    {
        private readonly IStateQueryRepository _stateQueryRepository;
        public StatesGrpcService(IStateQueryRepository stateQueryRepository)
        {
            _stateQueryRepository = stateQueryRepository;
        }
        public override async Task<StatesListResponse> GetAllStates(Empty request, ServerCallContext context)
        {
            var (states, _) = await _stateQueryRepository.GetAllStatesAsync(1, int.MaxValue, null);

            var response = new StatesListResponse();
            foreach (var state in states)
            {
                response.States.Add(new StatesDto
                {
                    StateId = state.Id,
                    StateCode = state.StateCode,
                    StateName = state.StateName ,
                    CountryId = state.CountryId                  
                });
            }
            return response;
        }
    }
}
  