using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteDocumentAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteFileAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.SaveAssetDocument;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadDocumentAssetMaster;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetCodePattern;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterByIdSplit;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralAutoComplete;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetParentMaster;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationGroup.Queries.GetAssetTypeQuery;
using Core.Application.DepreciationGroup.Queries.GetWorkingStatusQuery;
using Core.Application.ExcelImport;
using Core.Application.ExcelImport.PhysicalStockVerification;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace FAM.API.Controllers.AssetMaster
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetMasterGeneralController : ApiControllerBase
    {
        private readonly IValidator<CreateAssetMasterGeneralCommand> _createAssetMasterGeneralCommandValidator;
        private readonly IValidator<UpdateAssetMasterGeneralCommand> _updateAssetMasterGeneralCommandValidator;
        private readonly IValidator<UploadFileAssetMasterGeneralCommand> _uploadFileCommandValidator;
        private readonly IValidator<DeleteAssetMasterGeneralCommand> _deleteAssetMasterGeneralCommandValidator;
        private readonly IValidator<UploadDocumentAssetMasterGeneralCommand> _deleteDocumentAssetMasterGeneralCommandValidator;

        public AssetMasterGeneralController(
            ISender mediator,
            IValidator<CreateAssetMasterGeneralCommand> createAssetMasterGeneralCommandValidator,
            IValidator<UpdateAssetMasterGeneralCommand> updateAssetMasterGeneralCommandValidator,
            IValidator<UploadFileAssetMasterGeneralCommand> uploadFileCommandValidator,
            IValidator<DeleteAssetMasterGeneralCommand> deleteAssetMasterGeneralCommandValidator,
            IValidator<UploadDocumentAssetMasterGeneralCommand> deleteDocumentAssetMasterGeneralCommandValidator
        )
        : base(mediator)
        {
            _createAssetMasterGeneralCommandValidator = createAssetMasterGeneralCommandValidator;
            _updateAssetMasterGeneralCommandValidator = updateAssetMasterGeneralCommandValidator;
            _uploadFileCommandValidator = uploadFileCommandValidator;
            _deleteAssetMasterGeneralCommandValidator = deleteAssetMasterGeneralCommandValidator;
            _deleteDocumentAssetMasterGeneralCommandValidator = deleteDocumentAssetMasterGeneralCommandValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssetMasterGeneralAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)

        {
            var assetMaster = await Mediator.Send(
                new GetAssetMasterGeneralQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm
                });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = assetMaster.Message,
                data = assetMaster.Data.ToList(),
                TotalCount = assetMaster.TotalCount,
                PageNumber = assetMaster.PageNumber,
                PageSize = assetMaster.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,

                    message = "Invalid Asset ID"
                });
            }

            var result = await Mediator.Send(new GetAssetMasterGeneralByIdQuery { Id = id });
            if (result is null)

            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,

                    message = $"AssetId {id} not found",
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data

            });
        }
        [HttpGet("{id}/split")]
        public async Task<IActionResult> GetByIdSplitAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,

                    message = "Invalid Asset ID"
                });
            }

            var result = await Mediator.Send(new GetAssetMasterByIdSplitQuery { Id = id });
            if (result is null)

            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,

                    message = $"AssetId {id} not found",
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data

            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAssetMasterGeneralCommand command)
        {
            var validationResult = await _createAssetMasterGeneralCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                throw new ExceptionRules(string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)) ?? "Validation failed");

            var result = await Mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new ApiResponseDTO<AssetMasterDto>
            {
                StatusCode = StatusCodes.Status201Created,
                IsSuccess = true,
                Message = "AssetMasterGeneral created successfully.",
                Data = result
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateAssetMasterGeneralCommand command)

        {
            var validationResult = await _updateAssetMasterGeneralCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });

            }
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,

                    message = result.Message,
                    asset = result.Data
                });
            }
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var command = new DeleteAssetMasterGeneralCommand { Id = id };
            var validationResult = await _deleteAssetMasterGeneralCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ExceptionRules(string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)) ?? "Validation failed");
            }
            var result = await Mediator.Send(command);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = $"AssetMasterGeneral ID {id} deleted successfully."
            });
        }

        // GET: api/AssetMasterGeneral/by-name?name=...

        [HttpGet("by-name")]
        public async Task<IActionResult> GetAssetName([FromQuery] string? name)

        {
            var result = await Mediator.Send(new GetAssetMasterGeneralAutoCompleteQuery { SearchPattern = name });
            if (!result.IsSuccess)
            {

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message

                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                data = result.Data
            });
        }

        // GET: api/AssetMasterGeneral/AssetType
        [HttpGet("AssetType")]
        public async Task<IActionResult> GetAssetTypes()
        {
            var result = await Mediator.Send(new GetAssetTypeQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Asset Types found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Asset Types fetched successfully.",
                data = result.Data
            });
        }
        // GET: api/AssetMasterGeneral/WorkingStatus
        [HttpGet("ParentAsset")]
        public async Task<IActionResult> GetParentAsset([FromQuery] string assetType)
        {
            var result = await Mediator.Send(new GetAssetParentMasterQuery { AssetType = assetType });
            if (!result.IsSuccess)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                data = result.Data
            });
        }
        // GET: api/AssetMasterGeneral/WorkingStatus
        [HttpGet("WorkingStatus")]
        public async Task<IActionResult> GetWorkingStatus()
        {
            var result = await Mediator.Send(new GetWorkingStatusQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Working Status found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Working Status fetched successfully.",
                data = result.Data
            });
        }
        // GET: api/AssetMasterGeneral/AssetType
        [HttpGet("AssetCodePattern")]
        public async Task<IActionResult> GetAssetCodePattern()
        {
            var result = await Mediator.Send(new GetAssetCodePatternQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Asset Pattern found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Asset Pattern fetched successfully.",
                data = result.Data
            });
        }

        // POST: api/AssetMasterGeneral/upload-logo
        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(UploadFileAssetMasterGeneralCommand uploadFileCommand)
        {
            var validationResult = await _uploadFileCommandValidator.ValidateAsync(uploadFileCommand);
            if (!validationResult.IsValid)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            var file = await Mediator.Send(uploadFileCommand);
            if (!file.IsSuccess)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = file.Message,
                    errors = ""
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file.Message,
                data = file.Data,
                errors = ""
            });
        }
        // DELETE: api/AssetMasterGeneral/delete-logo
        [HttpDelete("delete-logo")]

        public async Task<IActionResult> DeleteLogo([FromBody] DeleteFileAssetMasterGeneralCommand deleteFileCommand)
        {
            if (deleteFileCommand == null || string.IsNullOrWhiteSpace(deleteFileCommand.assetPath))
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid request. 'assetPath' cannot be null or empty.",
                    errors = ""
                });
            }

            var file = await Mediator.Send(deleteFileCommand);
            if (!file.IsSuccess)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = file.Message,
                    errors = ""
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file.Message,
                errors = ""
            });
        }
        //Excel Import
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromForm] ImportAssetDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Please upload a valid Excel file.");

            var result = await Mediator.Send(new ImportAssetCommand(dto));

            if (result.IsSuccess && result.Data)
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = result.Message,
                    errors = ""
                });

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message,
                errors = ""
            });
        }
        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument(UploadDocumentAssetMasterGeneralCommand uploadFileCommand)
        {
            var validationResult = await _deleteDocumentAssetMasterGeneralCommandValidator.ValidateAsync(uploadFileCommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            var file = await Mediator.Send(uploadFileCommand);
            if (!file.IsSuccess)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = file.Message,
                    errors = ""
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file.Message,
                data = file.Data,
                errors = ""
            });
        }
        // DELETE: api/AssetMasterGeneral/delete-logo
        [HttpDelete("delete-document")]
        public async Task<IActionResult> DeleteDocument([FromBody] DeleteDocumentAssetMasterGeneralCommand deleteFileCommand)
        {
            if (deleteFileCommand == null || string.IsNullOrWhiteSpace(deleteFileCommand.assetPath))
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid request. 'assetPath' cannot be null or empty.",
                    errors = ""
                });
            }
            var file = await Mediator.Send(deleteFileCommand);
            if (!file.IsSuccess)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = file.Message,
                    errors = ""
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file.Message,
                errors = ""
            });
        }
        [HttpPost("saveDocument")]
        public async Task<IActionResult> UpdateDocument([FromBody] SaveAssetDocumentCommand saveCommand)
        {
            if (saveCommand == null || string.IsNullOrWhiteSpace(saveCommand.assetPath))
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid request. 'assetPath' cannot be null or empty.",
                    errors = ""
                });
            }
            var saveDoc = await Mediator.Send(saveCommand);
            if (!saveDoc.IsSuccess)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = saveDoc.Message,
                    errors = ""
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = saveDoc.Message,
                errors = ""
            });
        }
        [HttpPost("PhysicalAuditUpload")]
        [RequestSizeLimit(2 * 1024 * 1024)] // 2 MB
        public async Task<IActionResult> UploadExcel([FromForm] ImportAssetAuditDto dto)
        {
            if (dto == null || dto.File == null || dto.File.Length == 0)
            {
                return BadRequest(new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "File is missing or empty.",
                    Data = false
                });
            }

            var command = new ImportAssetAuditCommand(dto);
            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = result.Message,
                    errors = ""
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                errors = ""
            });
        }

        [HttpPost("ScanAsset")]
        public async Task<IActionResult> ScanAsset([FromBody] ScanAssetAuditCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.AssetCode))
                  return BadRequest(new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "AssetCode is required.",
                    Data = false
                });

            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                errors = ""
            });

             return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = result.Message,
                    errors = ""
                });
        }


    }
}
