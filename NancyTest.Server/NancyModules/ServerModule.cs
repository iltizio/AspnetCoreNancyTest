using System;
using Autofac.Features.Indexed;
using Nancy;
using Nancy.Responses;
using NancyTest.Server.Services;

namespace NancyTest.Server.NancyModules
{
    public class ServerModule : NancyModule
    {
        private readonly IIndex<string, IHelloService> _availableHelloServices;
        public ServerModule(IIndex<string, IHelloService> availableHelloServices)
        {
            _availableHelloServices = availableHelloServices;

            HelloWorld();

            SayFormalHello();

            SayInformalHello();
        }

        private void SayInformalHello()
        {
            Get("/SayHello2/{name}", args =>
            {
                string helloMessage;
                try
                {
                    helloMessage = SayHello("Informal", args.name);
                }
                catch (Exception ex)
                {
                    return new TextResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                return helloMessage;
            });
        }

        private void SayFormalHello()
        {
            Get("/SayHello", args =>
            {
                var name = Request.Query["name"];
                if(string.IsNullOrEmpty(name))
                    return new TextResponse(HttpStatusCode.BadRequest, "A name must be specified");

                string helloMessage;
                try
                {
                    helloMessage = SayHello("Formal", name);
                }
                catch (Exception ex)
                {
                    return new TextResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                return helloMessage;
            });
        }

        private void HelloWorld()
        {
            Get("/", args => "Hello World");
        }

        private string SayHello(string helloServiceKey, string name)
        {
            if(!_availableHelloServices.TryGetValue(helloServiceKey, out var helloFormal))
                throw new TypeAccessException("No HelloService implementation was found");

            return helloFormal.Hello(name);
        }
    }
}
