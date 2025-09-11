using System.Collections.Concurrent;
using ProjectGamma.Application.Dto.Request;
using ProjectGamma.Application.Dto.Response;
using ProjectGamma.Domain.Entities;
using ProjectGamma.Shared.Dto;
using static ProjectGamma.Shared.Dto.ApiResponseHelper;

namespace ProjectGamma.Application.Services;

public interface IAttractionService
{
    Task<StandardResponse> GetAllAsync();
    Task<StandardResponse> GetByIdAsync(Guid id);
    Task<StandardResponse> CreateAsync(AttractionRequest request);
}

public class AttractionService : IAttractionService
{
    private readonly ConcurrentDictionary<Guid, Attraction> _store = new();
    private const string EntityName = "Attraction";

    public AttractionService()
    {
        var a1 = new Attraction { Id = Guid.NewGuid(), City = "Sylhet",     Name = "Tea Garden", Category = "Nature" };
        var a2 = new Attraction { Id = Guid.NewGuid(), City = "Cox's Bazar", Name = "Sea Beach",  Category = "Nature" };
        _store[a1.Id] = a1; _store[a2.Id] = a2;
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

    public Task<StandardResponse> CreateAsync(AttractionRequest request)
    {
        var errors = new List<ErrorDetails>();
        if (string.IsNullOrWhiteSpace(request.City))     errors.Add(new ErrorDetails(nameof(request.City), "City is required."));
        if (string.IsNullOrWhiteSpace(request.Name))     errors.Add(new ErrorDetails(nameof(request.Name), "Name is required."));
        if (string.IsNullOrWhiteSpace(request.Category)) errors.Add(new ErrorDetails(nameof(request.Category), "Category is required."));

        if (errors.Count > 0) return Task.FromResult(ValidationError(EntityName, errors));

        var entity = new Attraction
        {
            Id = Guid.NewGuid(),
            City = request.City.Trim(),
            Name = request.Name.Trim(),
            Category = request.Category.Trim()
        };
        _store[entity.Id] = entity;

        return Task.FromResult(Created(EntityName, ToResponse(entity)));
    }

    private static AttractionResponse ToResponse(Attraction a) => new()
    {
        Id = a.Id, City = a.City!, Name = a.Name!, Category = a.Category!
    };
}