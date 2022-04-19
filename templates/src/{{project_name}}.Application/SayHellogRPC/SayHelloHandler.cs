using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using {{project_name}}.{{project_type}};

namespace {{project_name}}.Application.SayHelloGrpc
{
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
}