using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Language.Queries.GetLanguages
{
    public class GetLanguageQuery : IRequest<ApiResponseDTO<List<LanguageDTO>>>
    {
        
    }
}