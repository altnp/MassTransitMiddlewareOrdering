namespace Contracts.Profile;

public class ProfileCreated
{
    public Guid ProfileId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
}
