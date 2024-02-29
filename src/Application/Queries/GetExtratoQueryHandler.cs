namespace Application.Queries;

public sealed class GetExtratoQueryHandler(NpgsqlConnection connection,
                                             IClienteRepository clienteRepository,
                                             ITransacaoRepository transacaoRepository) : IRequestHandler<GetExtratoQuery, GetExtratoQueryViewModel>
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly ITransacaoRepository _transacaoRepository = transacaoRepository;

    public async ValueTask<GetExtratoQueryViewModel> Handle(GetExtratoQuery request, CancellationToken cancellationToken)
    {
        await connection.OpenAsync(cancellationToken);

        var saldo = _clienteRepository.GetSaldoTotal(request.Id, connection);
        var ultimasTransacoes = _transacaoRepository.ListTransacao(request.Id, connection);
        
        return new GetExtratoQueryViewModel(OperationResult.Success, new ExtratoDto(saldo, ultimasTransacoes.ToList()));
    }
}