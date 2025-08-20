using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.PartyMaster.Command.UploadPartyMasterDocument
{
    public class PartyDocumetDto  : IMapFrom<PartyDocument>
    {
        public string? FileName { get; set; }
        public string? PartyDocumentBase64 { get; set; }
    }
}