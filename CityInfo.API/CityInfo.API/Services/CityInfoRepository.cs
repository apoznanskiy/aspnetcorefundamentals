using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services;

public class CityInfoRepository : ICityInfoRepository
{
    private readonly CityInfoContext _cityInfoContext;

    public CityInfoRepository(CityInfoContext cityInfoContext)
    {
        _cityInfoContext = cityInfoContext;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _cityInfoContext.Cities.OrderBy(city => city.Name).ToListAsync();
    }

    public async Task<City?> GetCityAsync(int? cityId, bool includePointsOfInterest)
    {
        if (includePointsOfInterest)
        {
            return await _cityInfoContext.Cities
                .Include(city => city.PointsOfInterest)
                .Where(city => city.Id == cityId)
                .FirstOrDefaultAsync();
        }

        return await _cityInfoContext.Cities
            .Where(city => city.Id == cityId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
    {
        return await _cityInfoContext.PointsOfInterest.Where(city => city.Id == cityId).ToListAsync();
    }

    public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
    {
        return await _cityInfoContext.PointsOfInterest
            .Where(poi => poi.Id == pointOfInterestId && poi.CityId == cityId)
            .FirstOrDefaultAsync();
    }

    public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);
        if (city != null)
        {
            city.PointsOfInterest.Add(pointOfInterest);
        }
    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        _cityInfoContext.PointsOfInterest.Remove(pointOfInterest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _cityInfoContext.SaveChangesAsync() >= 0;
    }
}
