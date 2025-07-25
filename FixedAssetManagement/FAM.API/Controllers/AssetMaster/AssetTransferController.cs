using Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Command.UpdateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAllAssetTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetCustodian;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTranferedById;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetBulkAssetToTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByCustodian;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByDeptId;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetTransferType;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Entities.AssetMaster;
using FluentValidation;
using MassTransit.Futures.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace FAM.API.Controllers.AssetMaster
{
    [Route("api/[controller]")]
    public class AssetTransferController : ApiControllerBase

    {

        private readonly IValidator<CreateAssetTransferIssueCommand> _createAssetTransferIssueCommandValidator;
        private readonly IValidator<UpdateAssetTransferIssueCommand> _UpdateAssetTransferIssueCommandValidator;
        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;

        public AssetTransferController(ISender mediator, IValidator<CreateAssetTransferIssueCommand> createAssetTransferIssueCommand, IValidator<UpdateAssetTransferIssueCommand> updateAssetTransferIssueCommand, IAssetTransferQueryRepository assetTransferQueryRepository) : base(mediator)
        {
            _createAssetTransferIssueCommandValidator = createAssetTransferIssueCommand;
            _UpdateAssetTransferIssueCommandValidator = updateAssetTransferIssueCommand;
            _assetTransferQueryRepository = assetTransferQueryRepository;

        }

        [HttpGet("GetAllAssetTransfers")]
        public async Task<IActionResult> GetAllAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] DateTimeOffset? FromDate = null, [FromQuery] DateTimeOffset? ToDate = null, [FromQuery] string? SearchTerm = null)
        {
            var assetTransferList = await Mediator.Send(
                new AssetTransferQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    FromDate = FromDate,
                    ToDate = ToDate

                });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = assetTransferList.Data,
                TotalCount = assetTransferList.TotalCount,
                PageNumber = assetTransferList.PageNumber,
                PageSize = assetTransferList.PageSize
            });
        }

        [HttpGet("GetAllAssetTransfersByAssetTransferId/{id}")]
        //[HttpGet("GetAllAssetTransfers/{id}")]
        public async Task<IActionResult> GetAllAssetTransfersAsync(int id)
        {
            var query = new GetAllTransferQuery { AssetTransferId = id };
            var result = await Mediator.Send(query);

            if (result == null || result.Data == null)
            {
                var notFoundResponse = new ApiResponseDTO<object>
                {
                    IsSuccess = false,
                    Message = $"Asset Transfer with ID {id} not found.",
                    Data = null,
                    StatusCode = StatusCodes.Status404NotFound
                };
                return StatusCode(notFoundResponse.StatusCode, notFoundResponse);
            }

            // Return success response with proper status code and structure
            return StatusCode(result.StatusCode, new
            {
                statusCode = result.StatusCode,
                isSuccess = result.IsSuccess,
                message = result.Message,
                data = result.Data
            });
        }

        // public async Task<IActionResult> GetAllAssetTransfersAsync(int id)
        // {
        //     var query = new GetAllTransferQuery { AssetTransferId = id };
        //     var result = await Mediator.Send(query);
        //     if (result == null)
        //     {
        //         return NotFound($"Asset Transfer with ID {id} not found.");
        //     }
        //     return Ok(result);
        // }


        [HttpGet("GetCategoryByDepartmentId/{id}")]
        public async Task<IActionResult> GetCategoriesByDepartmentAsync(int id)
        {
            var assetCategoryList = await Mediator.Send(
                new GetCategoryByDeptIQuery
                {
                    DepartmentId = id

                });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = assetCategoryList.Data,

            });
        }


        [HttpPost]
        // public async Task<ActionResult<ApiResponseDTO<AssetTransferIssueHdr>>> CreateAssetTransfer([FromBody] CreateAssetTransferIssueCommand command)
        public async Task<IActionResult> CreateAsync(CreateAssetTransferIssueCommand command)
        {

            var validationResult = await _createAssetTransferIssueCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            if (command == null)
                return BadRequest(new ApiResponseDTO<AssetTransferIssueHdr>
                {
                    IsSuccess = false,
                    Message = "Invalid request data"
                });

            var response = await Mediator.Send(command);
            if (response.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created, new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = response.Message,
                    Data = response.Data
                });
            }

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = response.Message,
                Errors = ""
            });
            // if (!response.IsSuccess)

            //     return BadRequest(response);

            // return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssetTransferByIdAsync(int id)
        {
            var query = new GetAssetTranferedByIdQuery { AssetTransferId = id };
            var result = await Mediator.Send(query);

            if (result == null || result.Data == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    isSuccess = false,
                    message = $"Asset Transfer with ID {id} not found.",
                    data = (object)null
                });
            }

            return StatusCode(result.StatusCode, new
            {
                statusCode = result.StatusCode,
                isSuccess = result.IsSuccess,
                message = result.Message,
                data = result.Data
            });
        }


        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetAssetTransferByIdAsync(int id)
        // {
        //     var query = new GetAssetTranferedByIdQuery { AssetTransferId = id };
        //     var result = await Mediator.Send(query);
        //     if (result == null)
        //     {
        //         return NotFound($"Asset Transfer with ID {id} not found.");
        //     }
        //     return Ok(result);
        // }




        [HttpPut]
        public async Task<IActionResult> UpdateAssetTransferIssue([FromBody] UpdateAssetTransferIssueCommand command)
        {

            var validationResult = await _UpdateAssetTransferIssueCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                errors = ""
            });
        }

         [HttpGet("GetAssetsByCategory/{categoryId}/{assetDepartmentId}")]
            public async Task<IActionResult> GetAssetsByCategoryAsync(int categoryId, int assetDepartmentId)
            {
                var query = new GetAssetsByCategoryQuery
                {
                    AssetCategoryId = categoryId,
                    AssetDepartmentId = assetDepartmentId
                };

                var result = await Mediator.Send(query);

                if (result == null || result.Data == null || (result.Data is IEnumerable<object> list && !list.Any()))
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        isSuccess = false,
                        message = $"No assets found for Category ID {categoryId} and Department ID {assetDepartmentId}.",
                        data = (object)null
                    });
                }

                return StatusCode(result.StatusCode, new
                {
                    statusCode = result.StatusCode,
                    isSuccess = result.IsSuccess,
                    message = result.Message,
                    data = result.Data
                });
            }

        // [HttpGet("GetAssetsByCategory/{categoryId}/{assetDepartmentId}")]
        // public async Task<IActionResult> GetAssetsByCategoryAsync(int categoryId, int assetDepartmentId)
        // {
        //     var query = new GetAssetsByCategoryQuery { AssetCategoryId = categoryId, AssetDepartmentId = assetDepartmentId };
        //     var result = await Mediator.Send(query);
        //     return Ok(result);

        // }
        
        [HttpGet("GetAssetDetailsToTransfer/{AssetId}")]
          
        public async Task<IActionResult> GetAssetDetailsToTransferByIdAsync(int AssetId)
        {
            // 🔹 Step 1: Restriction Check
            bool isRestricted = await _assetTransferQueryRepository.IsAssetPendingOrApprovedAsync(AssetId);
            if (isRestricted)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = $"Asset ID {AssetId} is in 'Pending' or 'Approved' state with unacknowledged status.",
                    data = (object)null
                });
            }

            // 🔹 Step 2: Query Execution
            var query = new GetAssetDetailsToTransferQuery { AssetId = AssetId };
            var result = await Mediator.Send(query);

            // 🔹 Step 3: Not Found Check
            if (result == null || result.Data == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = $"Asset with ID {AssetId} not found.",
                    data = (object)null
                });
            }

            // 🔹 Step 4: Success Response (Fixed 200 OK)
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                data = result.Data
            });
        }


        // [HttpGet("GetAssetDetailsToTransfer/{AssetId}")]
        // public async Task<IActionResult> GetAssetDetailsToTransferByIdAsync(int AssetId)
        // {

        //     // 🔹 Check if the asset is pending or approved (with AckStatus <> 1)
        //     bool isRestricted = await _assetTransferQueryRepository.IsAssetPendingOrApprovedAsync(AssetId);

        //     if (isRestricted)
        //     {
        //         var errorResponse = new ApiResponseDTO<object>
        //         {
        //             IsSuccess = false,
        //             Message = $"Asset ID {AssetId} is in 'Pending' or 'Approved' state with unacknowledged status.",
        //             Data = null,
        //             StatusCode = StatusCodes.Status400BadRequest
        //         };
        //         return BadRequest(errorResponse);
        //     }

        //     var query = new GetAssetDetailsToTransferQuery { AssetId = AssetId };
        //     var result = await Mediator.Send(query);
        //       if (result == null || result.Data == null)
        //         {
        //             var notFoundResponse = new ApiResponseDTO<object>
        //             {
        //                 IsSuccess = false,
        //                 Message = $"Asset with ID {AssetId} not found.",
        //                 Data = null,
        //                 StatusCode = StatusCodes.Status404NotFound
        //             };
        //             return NotFound(notFoundResponse);
        //         }

        //         // Return result as is, since it already has ApiResponseDTO<T>


        //         return Ok(result); 

        // }



        [HttpGet("TransferTypes")]
        public async Task<IActionResult> GetDisposalTypes()
        {
            var result = await Mediator.Send(new GetTransferTypeQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Transfer Types found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Transfer Types fetched successfully.",
                data = result.Data
            });
        }


      //  [HttpGet("GetCustodiansByDepartment")]
        [HttpGet("GetCustodiansByDepartment")]
        public async Task<IActionResult> GetCustodiansByDepartment([FromQuery] int departmentId, [FromQuery] string oldUnitId)
        {
            if (string.IsNullOrWhiteSpace(oldUnitId))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiResponseDTO<List<GetAssetCustodianDto>>
                {
                    IsSuccess = false,
                    Message = "OldUnitId not found in user token.",
                    Data = null,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var query = new GetAssetCustodianQuery
            {
                OldUnitId = oldUnitId,
                DepartmentId = departmentId
            };

            var response = await Mediator.Send(query);

            return StatusCode(response.StatusCode, response);
        }

        // public async Task<IActionResult> GetCustodiansByDepartment([FromQuery] int departmentId, [FromQuery] string oldUnitId)
        // {
        //     if (string.IsNullOrWhiteSpace(oldUnitId))
        //     {
        //         return BadRequest(new ApiResponseDTO<List<GetAssetCustodianDto>>
        //         {
        //             IsSuccess = false,
        //             Message = "OldUnitId not found in user token.",
        //             Data = null
        //         });
        //     }

        //     var query = new GetAssetCustodianQuery
        //     {
        //         OldUnitId = oldUnitId,
        //         DepartmentId = departmentId
        //     };

        //     var response = await Mediator.Send(query);

        //     return Ok(response);
        // }

        [HttpGet("GetCategoriesByCustodian")]
       
        public async Task<IActionResult> GetCategoriesByCustodianAsync([FromQuery] int departmentId, [FromQuery] string custodianId)
        {
            var assetCategoryList = await Mediator.Send(
                new GetCategoryByCustodianQuery
                {
                    DepartmentId = departmentId,
                    CustodianId = custodianId

                });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = assetCategoryList.Data,

            });
        }
   
        [HttpGet("GetBulkAssetsToTransfer")]
        public async Task<IActionResult> GetBulkAssetsToTransfer(int departmentId, string custodianId, string categoryId)
        {
            if (string.IsNullOrWhiteSpace(custodianId))
            {
                var errorResponse = new ApiResponseDTO<List<GetAssetDetailsToTransferHdrDto>>
                {
                    IsSuccess = false,
                    Message = "CustodianId is required.",
                    Data = null,
                    StatusCode = 400,
                    Errors = new List<string> { "CustodianId cannot be null or empty." }
                };
                return BadRequest(errorResponse);
            }

            if (string.IsNullOrWhiteSpace(categoryId))
            {
                var errorResponse = new ApiResponseDTO<List<GetAssetDetailsToTransferHdrDto>>
                {
                    IsSuccess = false,
                    Message = "CategoryId is required.",
                    Data = null,
                    StatusCode = 400,
                    Errors = new List<string> { "CategoryId cannot be null or empty." }
                };
                return BadRequest(errorResponse);
            }

            var query = new GetBulkAssetToTransferQuery
            {
                DepartmentId = departmentId,
                CustodianId = custodianId,
                CategoryID = categoryId
            };

            var response = await Mediator.Send(query);

            if (response == null || !response.IsSuccess)
            {
                var notFoundResponse = new ApiResponseDTO<List<GetAssetDetailsToTransferHdrDto>>
                {
                    IsSuccess = false,
                    Message = response?.Message ?? $"No assets found for DepartmentId {departmentId}.",
                    Data = null,
                    StatusCode = 404,
                    Errors = new List<string> { "No matching records found." }
                };
                return NotFound(notFoundResponse);
            }

            // Success case
            response.StatusCode = 200;
            return Ok(response);
        }

        
        
    }
}