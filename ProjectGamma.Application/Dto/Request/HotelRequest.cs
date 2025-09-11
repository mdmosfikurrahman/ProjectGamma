namespace ProjectGamma.Application.Dto.Request;

public class HotelRequest
{
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public int Stars { get; set; }
}