using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }

    public static CitiesDataStore Current { get; } = new();

    public CitiesDataStore()
    {
        Cities = new List<CityDto>
        {
            new CityDto()
            {
                Id = 1, Name = "Kiev", Description = "The one in Ukraine.",
                PointOfInterests = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto() { Id = 1, Name = "Place 1", Description = "Description 1"},
                    new PointOfInterestDto() { Id = 2, Name = "Place 2", Description = "Description 2"}
                }
            },
            new CityDto()
            {
                Id = 2, Name = "Moscow", Description = "The one in Russia.",
                PointOfInterests = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto() { Id = 3, Name = "Place 3", Description = "Description 3"},
                    new PointOfInterestDto() { Id = 4, Name = "Place 4", Description = "Description 4"}
                }
            },
            new CityDto() { Id = 3, Name = "London", Description = "The one in England."}
        };
    }
}
