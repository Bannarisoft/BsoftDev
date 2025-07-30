using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetGroup.Command.DeleteAssetGroup
{
    public class DeleteAssetGroupCommand : IRequest<int>
    {
        public int Id { get; set; }
    }
}