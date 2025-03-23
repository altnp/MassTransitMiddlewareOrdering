using MassTransit;

namespace Contracts.Profile;

[ConfigureConsumeTopology(false)]
public class CreateProfile
{
    public Guid ProfileId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required string Email { get; set; }
}
