var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, SourceGenerationContext.Default);
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

var clientes = new Dictionary<int, int>
{
    {1,   1000 * 100},
    {2,    800 * 100},
    {3,  10000 * 100},
    {4, 100000 * 100},
    {5,   5000 * 100}
};

app.MapGet("/", () => "Hello World!");

app.MapGet("/clientes/{id:int}/extrato", async (int id, ISender sender, CancellationToken cancellationToken) =>
{    
    if (!clientes.ContainsKey(id))
        return Results.NotFound();

    var result = await sender.Send(new GetExtratoQuery(id), cancellationToken);

    return result.OperationResult switch
    {
        Application.Common.Models.OperationResult.NotFound => Results.NotFound(),
        Application.Common.Models.OperationResult.Failed => Results.UnprocessableEntity(),
        Application.Common.Models.OperationResult.Success => Results.Ok(result.Extrato),
        _ => Results.NoContent(),
    };
});

app.MapPost("/clientes/{id:int}/transacoes", async (int id, TransacaoDto transacao, ISender sender, CancellationToken cancellationToken) =>
{
    if (!clientes.ContainsKey(id))
        return Results.NotFound();

    if (!transacao.Valida())
        return Results.UnprocessableEntity();

    var result = await sender.Send(new CreateTransacaoCommand(id, transacao), cancellationToken);

    return result.OperationResult switch
    {
        Application.Common.Models.OperationResult.NotFound => Results.NotFound(),
        Application.Common.Models.OperationResult.Failed => Results.UnprocessableEntity(),
        Application.Common.Models.OperationResult.Success => Results.Ok(result.Cliente),
        _ => Results.NoContent(),
    };
});

app.Run();

[JsonSerializable(typeof(ClienteDto))]
[JsonSerializable(typeof(SaldoDto))]
[JsonSerializable(typeof(TransacaoDto))]
[JsonSerializable(typeof(ExtratoDto))]
internal partial class SourceGenerationContext : JsonSerializerContext { }