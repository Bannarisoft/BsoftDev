using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Menu.Queries.GetChildMenuByModule
{
    public class ChildMenuDTO
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public int ParentId { get; set; }
        public string MenuUrl { get; set; }
    }
}