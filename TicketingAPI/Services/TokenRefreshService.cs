using TicketingAPI.Repositories;

namespace TicketingAPI.Services;

public class TokenRefreshService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TokenRefreshService(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            using var scope =
                _scopeFactory.CreateScope();

            var repository =
                scope.ServiceProvider
                .GetRequiredService<TokenRepository>();
            
            Console.WriteLine("Regenerando tokens...");


            // await repository.RegenerarTokens();

            await Task.Delay(
                TimeSpan.FromSeconds(30),
                stoppingToken
            );
        }
    }
}