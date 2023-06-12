using CityInfo.API.Entities;

namespace CityInfo.API.Services;

public interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();

    Task<City?> GetCityAsync(int? cityId, bool includePointsOfInterest = false);

    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);

    Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);

    Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

    void DeletePointOfInterest(PointOfInterest pointOfInterest);

    Task<bool> SaveChangesAsync();
}
