using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class MiscMaster
    {

        public int Id { get; set;}
        public int MasterId { get; set;}
        public string Code { get; set;}
        public string Description { get; set;}
        public string Unit { get; set;}
        public decimal Rate { get; set;}
        public bool Active { get; set;} = false;
        
    }
}