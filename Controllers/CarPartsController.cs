using System.Security.Claims;
using CarPartsProject.DTOs.Requests.CarParts;
using CarPartsProject.DTOs.Responses;
using CarPartsProject.Enums;
using CarPartsProject.Services.PartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[Tags("Parts")]
public class PartsController(IPartService partService) : ControllerBase
{
    private Guid _currentUserId
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    [Authorize(Roles = "Admin", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<ActionResult<CarPartResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<CarPartResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreatePartAsyncV1")]
    [EndpointSummary("Creates a new part")]
    [EndpointDescription("Creates a new part and returns the created result.")]
    public async Task<ActionResult<CarPartResponse>> CreatePartAsync([FromForm] CarPartRequest request)
    {
        var part = await partService.CreatePartAsync(request, _currentUserId);
        return Created("Part added successfully", part);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<ActionResult<CarPartResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<CarPartResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdatePartAsyncV1")]
    [EndpointSummary("Update a part")]
    [EndpointDescription("Update a part and returns the updated result.")]
    public async Task<ActionResult<CarPartResponse>> UpdatePartAsync(Guid id, [FromForm] CarPartRequest request)
    {
        var part = await partService.UpdatePartAsync(id, request, _currentUserId);
        return Ok(part);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<CarPartResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<CarPartResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeletePartAsyncV1")]
    [EndpointSummary("Delete a part by id")]
    [EndpointDescription("Delete a part and returns the deleted result.")]
    public async Task<ActionResult> DeletePartAsync(Guid id)
    {
        await partService.DeletePartAsync(id);
        return Ok("Part deleted successfully");
    }

    [HttpGet("random/{count:int:min(1)}")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<CarPartResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetRandomPartsAsyncV1")]
    [EndpointSummary("Retrieves random parts")]
    [EndpointDescription("Retrieves random parts and returns the retrieve result.")]
    public async Task<ActionResult<List<CarPartResponse>>> GetRandomPartsAsync(int count = 10)
    {
        var parts = await partService.GetRandomPartsAsync(count);
        return Ok(parts);
    }

    [HttpGet]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<CarPartResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetAllPartsAsync")]
    [EndpointSummary("Retrieve parts by pagination")]
    [EndpointDescription("Retrieve parts by pagination byDefault retrieve first page and contains 10 parts.")]
    public async Task<ActionResult<List<CarPartResponse>>> GetAllPartsAsync([FromQuery] int Page = 1, [FromQuery] int PageSize = 10)
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var parts = await partService.GetAllPartsAsync(Page, PageSize);
        return Ok(parts);
    }

    [HttpGet("status/{status:alpha}")]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<CarPartResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPartsByStatusAsyncV1")]
    [EndpointSummary("Retrieves parts by status")]
    [EndpointDescription("Retrieves parts by status and returns the retrieve result.")]
    public async Task<ActionResult<List<CarPartResponse>>> GetPartsByStatusAsync(string status, [FromQuery] int Page = 1, [FromQuery] int PageSize = 10)
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var parts = await partService.GetPartsByStatusAsync(status, Page, PageSize);
        return Ok(parts);
    }

    [HttpGet("brand/{brand}")]   
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<CarPartResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPartsByBrandAsyncV1")]
    [EndpointSummary("Retrieves parts by brand")]
    [EndpointDescription("Retrieves parts by brand and returns the retrieve result.")]
    public async Task<ActionResult<List<CarPartResponse>>> GetPartsByBrandAsync(string brand, [FromQuery] int Page = 1, [FromQuery] int PageSize = 10)
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var parts = await partService.GetPartsByBrandAsync(brand, Page, PageSize);
        return Ok(parts);
    }

    [HttpGet("search/{name}")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [ProducesResponseType<ActionResult<CarPartResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("SearchPartsAsyncV1")]
    [EndpointSummary("Retrieves parts by name")]
    [EndpointDescription("Retrieves parts by name and returns the retrieve result.")]
    public async Task<ActionResult<List<CarPartResponse>>> GetPartsByNameAsync(string name)
    {
        var parts = await partService.SearchPartsAsync(name);
        return Ok(parts);
    }

    [HttpGet("country/{countryName}")]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<CarPartResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPartsByCountryAsyncV1")]
    [EndpointSummary("Retrieves parts by country name")]
    [EndpointDescription("Retrieves parts by country name and returns the retrieve result.")]
    public async Task<ActionResult<List<CarPartResponse>>> GetPartsByCountryAsync(string countryName, [FromQuery] int Page = 1, [FromQuery] int PageSize = 10)
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var parts = await partService.GetPartsByCountryAsync(countryName, Page, PageSize);
        return Ok(parts);
    }
}
