using System.Collections.Concurrent;
using ProjectGamma.Application.Dto.Request;
using ProjectGamma.Application.Dto.Response;
using ProjectGamma.Domain.Entities;
using ProjectGamma.Shared.Dto;
using static ProjectGamma.Shared.Dto.ApiResponseHelper;


namespace ProjectGamma.Application.Services;

public interface IHotelService
{
    Task<StandardResponse> GetAllAsync();
    Task<StandardResponse> GetByIdAsync(Guid id);
    Task<StandardResponse> CreateAsync(HotelRequest request);
}

public class HotelService : IHotelService
{
    private readonly ConcurrentDictionary<Guid, Hotel> _store = new();
    private const string EntityName = "Hotel";

    public HotelService()
    {
        var h1 = new Hotel { Id = Guid.NewGuid(), Name = "Blue Lagoon", City = "Dhaka", Stars = 4 };
        var h2 = new Hotel { Id = Guid.NewGuid(), Name = "Riverview Inn", City = "Chattogram", Stars = 3 };
        _store[h1.Id] = h1; _store[h2.Id] = h2;
    }

    public Task<StandardResponse> GetAllAsync()
    {
        var list = _store.Values.Select(ToResponse).ToList();
        return Task.FromResult(list.Count == 0 ? NotFound($"{EntityName}s") : Success($"{EntityName}s", list));
    }

    public Task<StandardResponse> GetByIdAsync(Guid id)
    {
        return Task.FromResult(!_store.TryGetValue(id, out var entity)
            ? NotFound($"{EntityName} with Id {id}")
            : Success(EntityName, ToResponse(entity)));
    }

    public Task<StandardResponse> CreateAsync(HotelRequest request)
    {
        var errors = new List<ErrorDetails>();
        if (string.IsNullOrWhiteSpace(request.Name)) errors.Add(new ErrorDetails(nameof(request.Name), "Name is required."));
        if (string.IsNullOrWhiteSpace(request.City)) errors.Add(new ErrorDetails(nameof(request.City), "City is required."));
        if (request.Stars < 1 || request.Stars > 5) errors.Add(new ErrorDetails(nameof(request.Stars), "Stars must be 1-5."));

        if (errors.Count > 0) return Task.FromResult(ValidationError(EntityName, errors));

        var entity = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            City = request.City.Trim(),
            Stars = request.Stars
        };
        _store[entity.Id] = entity;

        return Task.FromResult(Created(EntityName, ToResponse(entity)));
    }

    private static HotelResponse ToResponse(Hotel h) => new()
    {
        Id = h.Id, Name = h.Name!, City = h.City!, Stars = h.Stars
    };
}