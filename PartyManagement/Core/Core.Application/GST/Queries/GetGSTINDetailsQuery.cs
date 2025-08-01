using Core.Application.GST.DTOs;
using MediatR;

namespace Core.Application.GST.Queries
{
    public record GetGSTINDetailsQuery(string Gstin) : IRequest<GSTINDetailsDto>;
}