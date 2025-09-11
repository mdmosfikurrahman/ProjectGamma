namespace ProjectGamma.Domain.Entities;

public class Hotel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? City { get; set; }
    public int Stars { get; set; }
}