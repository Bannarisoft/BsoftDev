using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Divisions.Queries.GetDivisions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Divisions.Commands.CreateDivision
{
    public class CreateDivisionCommand : IRequest<ApiResponseDTO<DivisionDTO>>
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public byte IsActive { get; set; }
    }
}