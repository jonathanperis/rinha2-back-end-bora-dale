namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNpgsqlDataSource(
            configuration.GetConnectionString("DefaultConnection")!
        );

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();
    }
}