namespace ProjectGamma.Application.Dto.Response;

public class AttractionResponse
{
    public Guid Id { get; set; }
    public string City { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Category { get; set; } = default!;
}