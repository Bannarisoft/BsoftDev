using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Commands.DeleteItemImage;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteFileAssetMasterGeneral
{
    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, bool>
    {        
        private readonly IItemQueryRepository _itemQueryRepository;
        private readonly ILogger<DeleteFileCommandHandler> _logger;
        private readonly IFileUploadService _fileUploadService;

        public DeleteFileCommandHandler(ILogger<DeleteFileCommandHandler> logger, IItemQueryRepository itemQueryRepository, IFileUploadService fileUploadService)
        {
            _logger = logger;
            _itemQueryRepository = itemQueryRepository;
            _fileUploadService = fileUploadService;            
        }

        public async Task<bool> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {            
            string baseDirectory = await _itemQueryRepository.GetBaseDirectoryAsync();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                _logger.LogError("Base directory path not found in database.");
                throw new Exception("Base directory not configured.");                            
            }            
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory);       

            string filePath = Path.Combine(uploadPath, request.imagePath??string.Empty);

            var result = await _fileUploadService.DeleteFileAsync(filePath);

            await _itemQueryRepository.RemoveImageReferenceAsync(request.imagePath);
            if (result)
            {
                return result;
            }
            throw new Exception("File deletion failed");            
        }
    }
}
