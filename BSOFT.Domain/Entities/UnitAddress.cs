using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using BSOFT.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace BSOFT.Domain.Entities
{
    [Table("UnitAddress", Schema = "AppData")]
        public class UnitAddress
    {
        public int Id { get; set; }

        public int  UnitId { get; set; }

        public int  CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        [MaxLength(250)]
        public string AddressLine1 { get; set; }

        [MaxLength(250)]
        public string AddressLine2 { get; set; }

        public int PinCode  { get; set; }
        
        [MaxLength(15)]
        public string ContactNumber { get; set; }

        [MaxLength(15)]
        public string AlternateNumber { get; set; }
         

    }
}