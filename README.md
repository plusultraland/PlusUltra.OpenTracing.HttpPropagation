[![Build Status](https://alefcarlos.visualstudio.com/PlusUltra/_apis/build/status/alefcarlos.PlusUltra.OpenTracing.HttpPropagation?branchName=master)](https://alefcarlos.visualstudio.com/PlusUltra/_build/latest?definitionId=25&branchName=master)

[![Nuget](https://img.shields.io/nuget/v/PlusUltra.OpenTracing.HttpPropagation)](https://www.nuget.org/packages/PlusUltra.OpenTracing.HttpPropagation/)


# PlusUltra.OpenTracing.HttpPropagation
Conjunto de bibliotecas para substituir opentracing-contrib/csharp-netcore em relação a Http

## Obter contexto no Action do controller

Para extrair o contexto de uma requisição recebida devemos adicionar o `Action Filter`, exemplo configurando de forma global:

```csharp
services.AddControllers(options =>
{
    opttion.AddTraceIncomingRequestFilter();
});
```

## Injetar contexto no request usando HttpClientFactory

Quando os http cliente são construídos utilizando `HttpClientFactory` isso é feito de forma automática, quando configurado o `PropagateHttpTracingContext` no `IWebHostBuilder`

```csharp
WebHost.CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .PropagateHttpTracingContext();
```