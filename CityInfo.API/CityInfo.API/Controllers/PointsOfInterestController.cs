using System.Runtime.CompilerServices;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            throw new Exception("Excption sample.");

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointOfInterests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception while getting points of interest for city with id {cityId}.");

            return StatusCode(500, "A problem happened handling your request1.");
        }
    }

    [HttpGet("{pointOfInterestId:int}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var pointOfInterest = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId)?.PointOfInterests
            .FirstOrDefault(pi => pi.Id == pointOfInterestId);

        if (pointOfInterest == null)
        {
            _logger.LogInformation($"City with id {cityId} was not found when accessing point of interest {pointOfInterestId}.");
            return NotFound();
        }

        return Ok(pointOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId,
        PointOfInterestForCreationDto pointOfInterestForCreationDto)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointOfInterests).Max(pi => pi.Id);
        var finalPointOfInterest = new PointOfInterestDto
        {
            Id = ++maxPointOfInterestId,
            Name =pointOfInterestForCreationDto.Name,
            Description = pointOfInterestForCreationDto.Description
        };

        city.PointOfInterests.Add(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pointOfInterestId = finalPointOfInterest.Id }, finalPointOfInterest);
    }

    [HttpPut("{pointOfInterestId}")]
    public ActionResult UdpatePointOfInterst(int cityId, int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterestForUpdateDto)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdate = city.PointOfInterests.FirstOrDefault(pi => pi.Id == pointOfInterestId);
        if (pointOfInterestToUpdate == null)
        {
            return NotFound();
        }

        pointOfInterestToUpdate.Name = pointOfInterestForUpdateDto.Name;
        pointOfInterestToUpdate.Description = pointOfInterestForUpdateDto.Description;

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public ActionResult PatchPointOfInterest(int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdate = city.PointOfInterests.FirstOrDefault(pi => pi.Id == pointOfInterestId);
        if (pointOfInterestToUpdate == null)
        {
            return NotFound();
        }

        var pointOfInterestToPatch = new PointOfInterestForUpdateDto
        {
            Name = pointOfInterestToUpdate.Name,
            Description = pointOfInterestToUpdate.Description
        };

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }

        pointOfInterestToUpdate.Name = pointOfInterestToPatch.Name;
        pointOfInterestToUpdate.Description = pointOfInterestToPatch.Description;

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToDelete = city.PointOfInterests.FirstOrDefault(pi => pi.Id == pointOfInterestId);
        if (pointOfInterestToDelete == null)
        {
            return NotFound();
        }

        city.PointOfInterests.Remove(pointOfInterestToDelete);
        _mailService.Send("Point of interest was deleted.", "Point of interest was deleted.");

        return NoContent();
    }
}
