using Contracts.Profile;
using MassTransit;

namespace Gateway.Endpoints;

public class CreateProfileEndpoint : Endpoint
{
    public override void Register(WebApplication app)
    {
        app.MapPost(
            "/profiles/",
            async (CreateProfileReqeust request, ISendEndpointProvider sendEndpointProvider, CancellationToken ct) =>
            {
                var endpoint = await sendEndpointProvider.GetSendEndpoint<CreateProfile>(); //Custom extension to get by convention

                await endpoint.Send<CreateProfile>(
                    new
                    {
                        ProfileId = Guid.NewGuid(),
                        request.FirstName,
                        request.LastName,
                        request.Email,
                        request.DateOfBirth,
                    },
                    ct
                );
                return Results.Accepted();
            }
        );
    }
}

public class CreateProfileReqeust
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required string Email { get; set; }
}
