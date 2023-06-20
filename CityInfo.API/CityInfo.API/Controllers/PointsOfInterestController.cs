using System.Runtime.CompilerServices;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
[Authorize(Policy = "MustBeFromLondon")]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger,
        IMailService mailService,
        ICityInfoRepository cityInfoRepository,
        IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _cityInfoRepository = cityInfoRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        try
        {
            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }


            var city = await _cityInfoRepository.GetCityAsync(cityId, true);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(city.PointsOfInterest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception while getting points of interest for city with id {cityId}.");

            return StatusCode(500, "A problem happened handling your request1.");
        }
    }

    [HttpGet("{pointOfInterestId:int}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = await _cityInfoRepository.GetCityAsync(cityId);

        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterest = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);

        if (pointOfInterest == null)
        {
            _logger.LogInformation($"City with id {cityId} was not found when accessing point of interest {pointOfInterestId}.");
            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
        PointOfInterestForCreationDto pointOfInterestForCreationDto)
    {
        var city = await _cityInfoRepository.GetCityAsync(cityId);
        if (city == null)
        {
            return NotFound();
        }

        var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterestForCreationDto);

        await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

        await _cityInfoRepository.SaveChangesAsync();

        var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pointOfInterestId = createdPointOfInterest.Id }, createdPointOfInterest);
    }

    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult> UdpatePointOfInterst(int cityId, int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterestForUpdateDto)
    {
        var city = await _cityInfoRepository.GetCityAsync(cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdate = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
        if (pointOfInterestToUpdate == null)
        {
            return NotFound();
        }

        _mapper.Map(pointOfInterestForUpdateDto, pointOfInterestToUpdate);

        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public async Task<ActionResult> PatchPointOfInterest(int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = await _cityInfoRepository.GetCityAsync(cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdate = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
        if (pointOfInterestToUpdate == null)
        {
            return NotFound();
        }

        var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestToUpdate);

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(pointOfInterestToPatch, pointOfInterestToUpdate);

        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = await _cityInfoRepository.GetCityAsync(cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToDelete = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
        if (pointOfInterestToDelete == null)
        {
            return NotFound();
        }

        _cityInfoRepository.DeletePointOfInterest(pointOfInterestToDelete);
        await _cityInfoRepository.SaveChangesAsync();

        _mailService.Send("Point of interest was deleted.", "Point of interest was deleted.");

        return NoContent();
    }
}
