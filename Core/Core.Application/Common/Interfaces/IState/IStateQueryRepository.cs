using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IState
{
    public interface IStateQueryRepository
    {
        Task<States>  GetByIdAsync(int stateId);
        Task<List<States>> GetAllStatesAsync();        
        Task<Result<List<States>>> GetByStateNameAsync(string stateName);           
    }
}