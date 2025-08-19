using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.PartyMaster.Command.UploadPartyMasterDocument
{
    public class UploadPartyMasterDocumentCommand : IRequest<PartyDocumetDto>
    {
        public IFormFile? File { get; set; }  
    }
}