using System.Data;
using System.Transactions;
using Contracts.Profile;
using Dapper;
using MassTransit;
using Profile.Data;
using Utils.Middlewares;

namespace Profile.Consumers;

public class CreateProfileConsumer(IDbConnection dbConnection) : IConsumer<CreateProfile>
{
    public async Task Consume(ConsumeContext<CreateProfile> context)
    {
        // await Task.Delay(65000);
        var message = context.Message;

        const string sql =
            @"
                INSERT INTO Profiles (profile_id, first_name, last_name, email, date_of_birth)
                VALUES (@ProfileId, @FirstName, @LastName, @Email, @DateOfBirth);
            ";

        var parameters = new
        {
            message.ProfileId,
            message.FirstName,
            message.LastName,
            message.Email,
            DateOfBirth = message.DateOfBirth.ToDateTime(TimeOnly.MinValue),
        };

        await Task.Delay(100);
        await Task.Delay(100);

        var transactionContext = context.GetPayload<TransactionContext>(); //Should handle if this doesn't exist I guess...
        using var txnScope = new TransactionScope(
            transactionContext.Transaction,
            TransactionScopeAsyncFlowOption.Enabled
        ); //Does not automatically flow

        //Dapper doesnt accept canceltion token? Weird...
        await dbConnection.ExecuteAsync(sql, parameters, commandTimeout: 30);

        await context.Publish<ProfileCreated>(
            new
            {
                __CorrelationId = context.CorrelationId, //Should write a middleware that auto-forwards this
                message.ProfileId,
                message.Email,
                message.FirstName,
                message.LastName,
            },
            context.CancellationToken
        );

        txnScope.Complete();
    }
}

public class CreateProfileConsumerDefinition : ConsumerDefinition<CreateProfileConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CreateProfileConsumer> consumerConfigurator,
        IRegistrationContext context
    )
    {
        endpointConfigurator.UseEntityFrameworkOutbox<ProfileDbContext>(context);
        endpointConfigurator.UseConsumeFilter(typeof(ConsumerInspectionFilter<>), context);
        endpointConfigurator.UseMessageRetry(r => r.Interval(10, 1000));
        endpointConfigurator.UseTransaction(opts => { });
        endpointConfigurator.AddPipeSpecification(new TestSpecification<CreateProfile>());
    }
}
