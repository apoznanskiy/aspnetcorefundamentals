using System.Text.Json;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/cities")]
[ApiVersion("1.0")]
public class CitiesController : ControllerBase
{
    private const int MAX_CITIES_PAGE_SIZE = 20;

    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
        _cityInfoRepository = cityInfoRepository ??
                              throw new ArgumentNullException(nameof(cityInfoRepository));

        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Get cities from database
    /// </summary>
    /// <param name="name">City name</param>
    /// <param name="searchQuery">Search text</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
        string? name,
        string ? searchQuery,
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageSize > MAX_CITIES_PAGE_SIZE)
        {
            pageSize = MAX_CITIES_PAGE_SIZE;
        }

        var (cities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(
            name, searchQuery, pageNumber, pageSize);

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
    {
        var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

        if (city == null)
        {
            return NotFound();
        }

        if (includePointsOfInterest)
        {
            return Ok(_mapper.Map<CityDto>(city));
        }

        return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
    }
}
