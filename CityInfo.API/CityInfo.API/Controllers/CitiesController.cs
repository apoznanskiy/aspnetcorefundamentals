using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {
        return Ok(CitiesDataStore.Current.Cities);
    }

    [HttpGet("{id:int}")]
    public ActionResult<CityDto> GetCity(int id)
    {
        var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == id);

        if (cityToReturn == null)
        {
            return NotFound();
        }

        return Ok(cityToReturn);
    }
}
