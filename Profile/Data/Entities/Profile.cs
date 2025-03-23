namespace Profile.Data.Entities;

public class Profile
{
    public int Id { get; set; }
    public Guid ProfileId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public DateOnly DateOfBirth { get; set; }
}
