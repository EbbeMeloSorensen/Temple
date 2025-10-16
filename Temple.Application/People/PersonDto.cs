namespace Temple.Application.People;

public class PersonDto
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime Created { get; set; }
    public DateTime Superseded { get; set; }
    public string FirstName { get; set; } = null!;
    public string? Surname { get; set; }
    public string? Nickname { get; set; }
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool? Dead { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
