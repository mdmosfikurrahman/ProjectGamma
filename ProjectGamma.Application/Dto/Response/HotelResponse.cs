namespace ProjectGamma.Application.Dto.Response;

public class HotelResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public int Stars { get; set; }
}