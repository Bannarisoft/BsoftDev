using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Core.Application.Location.Command.CreateLocation;
using Core.Application.Location.Command.UpdateLocation;
using Core.Application.Location.Queries.GetLocations;
using Core.Application.Location.Command.DeleteLocation;
using Core.Application.Location.Queries.GetLocationAutoComplete;
using Core.Application.Location.Queries.GetLocationById;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class LocationController : ApiControllerBase
    {
        private readonly IValidator<CreateLocationCommand> _createLocationCommandValidator;
        private readonly IValidator<UpdateLocationCommand> _updateLocationCommandValidator;

        public LocationController(ISender mediator,IValidator<CreateLocationCommand> createLocationCommandValidator,IValidator<UpdateLocationCommand> updateLocationCommandValidator) 
        : base(mediator)
        {
            _createLocationCommandValidator = createLocationCommandValidator;
            _updateLocationCommandValidator = updateLocationCommandValidator;
        }
         [HttpGet]
        public async Task<IActionResult> GetAllLocationAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var locations = await Mediator.Send(
            new GetLocationQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = locations.Data.ToList(),
                TotalCount = locations.TotalCount,
                PageNumber = locations.PageNumber,
                PageSize = locations.PageSize
                });
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateLocationCommand createcommand)
        {
            
            var validationResult = await _createLocationCommandValidator.ValidateAsync(createcommand);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
            var response = await Mediator.Send(createcommand);
            if(response.IsSuccess)
            {
                return Ok(new { StatusCode=StatusCodes.Status201Created, message = response.Message, errors = "", data = response.Data });
            }
             
            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = response.Message, errors = "" }); 
            
        }
         [HttpGet("{id}")]
         [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           
            var division = await Mediator.Send(new GetLocationByIdQuery() { Id = id});
          
             if(division == null)
            {
                return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"Division ID {id} not found.", errors = "" });
            }
            return Ok(new { StatusCode=StatusCodes.Status200OK, data = division.Data});
        }

        [HttpPut]
        public async Task<IActionResult> Update( UpdateLocationCommand updateLocationcommand )
        {
            var validationResult = await _updateLocationCommandValidator.ValidateAsync(updateLocationcommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
          

             var locationExists = await Mediator.Send(new GetLocationByIdQuery { Id = updateLocationcommand.Id });

             if (locationExists == null)
             {
                 return NotFound(new { StatusCode=StatusCodes.Status404NotFound, message = $"Location ID {updateLocationcommand.Id} not found.", errors = "" }); 
             }

             var response = await Mediator.Send(updateLocationcommand);
             if(response.IsSuccess)
             {
                 return Ok(new { StatusCode=StatusCodes.Status200OK, message = response.Message, errors = "" });
             }
            
           

            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = response.Message, errors = "" }); 
        }


        [HttpDelete("{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
           
           var deletedlocation = await Mediator.Send(new DeleteLocationCommand { Id = id });

           if(deletedlocation.IsSuccess)
           {
            return Ok(new { StatusCode=StatusCodes.Status200OK, message = deletedlocation.Message, errors = "" });
              
           }

            return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = deletedlocation.Message, errors = "" });
            
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetLocation([FromQuery] string? name)
        {
            var locations = await Mediator.Send(new GetLocationAutoCompleteQuery {SearchPattern = name});
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = locations.Data });
        }
        
      
      
    }
}