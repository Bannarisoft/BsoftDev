using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class UserCompany
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? user { get; set; }
        public int CompanyId { get; set; }
        public Company? company { get; set; }
        public byte IsActive { get; set; }
    }
}