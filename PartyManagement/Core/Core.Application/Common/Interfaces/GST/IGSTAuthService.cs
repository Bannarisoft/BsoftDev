using Core.Application.GST.DTOs;

namespace Core.Application.Interfaces.GST
{
    public interface IGSTAuthService
    {
        Task<GSTAuthResponseDto> GetAuthTokenAsync();
        Task<GSTINDetailsDto> GetGSTINDetailsAsync(string gstin);
    }
}