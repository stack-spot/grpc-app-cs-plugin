
## **Visão Geral**
### **gprc-app-cs-plugin**

O **gprc-app-cs-plugin** adiciona em uma stack a capacidade de expor serviços `gRPC` e suas configurações, bem como provê a criação de um `gRPC Client` que permite a distribuição e consumo desse serviço em outras aplicações.
## **Uso**

### **Pré-requisitos**
Para utilizar esse plugin é necessário ter uma stack dotnet criada pelo CLI do StackSpot que você pode baixar [**aqui**](https://stackspot.com.br/).

Ter instalado:
- .NET 5 ou 6 
- O template `rest-app-cs-template` já deverá estar aplicado para você conseguir utilizar este plugin.

## **Configuração**

#### **Inputs**

Os inputs necessários para utilizar o plugin são:
| **Campo** | **Valor** | **Descrição** |
| :--- | :--- | :--- |
| gPRC Port | ex.: 50051 |  Porta em que será exposta a comunicação gRPC |

#### **Configurações**
Adicione ao seu `IServiceCollection` via `services.AddGrpcServer();` no arquivo `Startup` ou `Program`. 

```csharp
services.AddGrpcServer();
```

Adicione ao seu `IApplicationBuilder` via `app.UseGrpc` no `Startup` ou `Program`. 

```csharp
app.UseGrpc(environment, Assembly.GetEntryAssembly(), Path.Combine(Directory.GetParent(environment.ContentRootPath).FullName, "Protos"));

```

Adicione às configurações de portas do seu `WebHost` via `builder.WebHost.ConfigureKestrel` no arquivo `Program`. 

```csharp
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 80, listenOptions =>
   {
       listenOptions.Protocols = HttpProtocols.Http1;
   });
    options.Listen(IPAddress.Any, 50051, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.ListenLocalhost(5005, o => o.Protocols = HttpProtocols.Http2); 
});
```

#### **Implementação**

O **gprc-app-cs-plugin** adiciona à sua stack alguns arquivos que auxiliam com um exemplo completo de implementação de um Server e de um Client gRPC.

#### ***Arquivo Proto***

O arquivo `.proto` possui as definições dos serviços e mensagens que serão utilizadas na comunicação.
Com as configurações de gRPC ativadas em seu projeto, ao Compilar sua aplicação serão criados arquivos no projeto `MyApp.Application\Base` com as classes geradas pelo plugin de gRPC. No nosso exemplo os arquivos são: `Greet.cs` e `GreetGrpc.cs`.

```proto
syntax = "proto3";

option csharp_namespace = "MyApp.Api";

package greet;

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
```

#### ***Service***

No projeto `Api` foi adicionado o arquivo `SayHelloService` que define o endpoint que será utilizado para receber as requisições.
Importante notar que essa classe herda da classe `Greeter.GreeterBase` que foi gerada a automaticamente a partir do arquivo .proto e compilação da solução.

```csharp
public class SayHelloService : Greeter.GreeterBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SayHelloService> _logger;

        public SayHelloService(IMediator mediator, ILogger<SayHelloService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Start Process", request);

            return await _mediator.Send(new SayHelloCommand(request));
        }
    }
```

Adicionalmente no projeto `Application` foram criadas classes `Command` e `Handler` seguindo o padrão inicial do template com Clean Architeture.

```csharp
    public class SayHelloCommand : IRequest<HelloReply>
    {
        public SayHelloCommand(HelloRequest helloRequest)
        {
            HelloRequest = helloRequest;
        }

        public HelloRequest HelloRequest { get; set; }
    }
```

```csharp
    public class SayHelloHandler : IRequestHandler<SayHelloCommand, HelloReply>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SayHelloHandler> _logger;

        public SayHelloHandler(IMediator mediator, ILogger<SayHelloHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Task<HelloReply> Handle(SayHelloCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Process Handler", request);

            return Task.FromResult(new HelloReply {Message = $"Hello {request.HelloRequest.Name }!"});

        }
    }
```

#### ***Client***

Um projeto `*.GrpcClient.csproj` é adicionado à sua solução e ao realizar a compilação do seu projeto, este irá criar as classes `Greet.cs` e `GreetGrpc.cs` com as implementações para aplicações clientes que irão lançar requisições para o seu endpoint gRPC.
Ele vem pré-configurado para que você possa, por exemplo, empacotar com o nuget e distribuir para quem quiser utilizar.

#### **Ambiente local**

Também como configuração adicional foi exposta nos arquivos `Dockerfile`e `docker-compose.yml` a porta para o serviço gRPC informada nos `Inputs`.


### **Exemplo de uso**
- [**Nuget**](https://www.nuget.org/packages/StackSpot.Grpc/)
