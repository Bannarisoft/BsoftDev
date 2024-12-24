using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Application.Common.Mappings;
using BSOFT.Domain.Entities;

namespace BSOFT.Application.Companies.Queries.GetCompanies
{
    public class CompanyContactDTO 
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Remarks { get; set; }

        //  public void Mapping(Profile profile)
        //  {
        //      profile.CreateMap<CompanyContactDTO, CompanyContact>();
        //  }

    }
}