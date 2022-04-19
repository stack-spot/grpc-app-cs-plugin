using MediatR;
using {{project_name}}.{{project_type}};

namespace {{project_name}}.Application.SayHelloGrpc
{
    public class SayHelloCommand : IRequest<HelloReply>
    {
        public SayHelloCommand(HelloRequest helloRequest)
        {
            HelloRequest = helloRequest;
        }

        public HelloRequest HelloRequest { get; set; }
    }
}