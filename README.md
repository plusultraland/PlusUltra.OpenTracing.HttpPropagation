# PlusUltra.OpenTracing.HttpPropagation
Conjunto de bibliotecas para substituir opentracing-contrib/csharp-netcore em relação a Http

## Obter contexto no Action do controler

Para extrair o contexto de uma requisição recebida devemos adicionar o `Action Filter`, exemplo configurando de forma global:

```csharp
services.AddControllers(options =>
{
    opttion.AddTraceIncomingRequestFilter();
});
```

## Injetar contexto com HttpCliente 

Para injar o contexto quando usar HttpClient é necessário adicionar o handler ao seu DI:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClientTracig();
}
```

E então, adicionar o Handler na chamada do HttpCliente, um exemplo com `Refit`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRefitCliente(configuration)
            .AddHttpMessageHandler<TraceHttpRequestHandler>();
}
```
