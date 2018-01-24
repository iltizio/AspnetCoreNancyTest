using System;
using System.Collections.Generic;
using Autofac.Features.Indexed;
using Nancy;
using Nancy.Responses;
using NancyTest.Server.Models;
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

            Get("/test", args =>
            {
                var artists = new ArtistSearchModel
                {
                    Artists = new List<Artist>
                    {
                        new Artist
                        {
                            Name = "Queen",
                            BannerImgUri = "~/img/queen.jpg"
                        },
                        new Artist
                        {
                            Name = "Yes",
                            BannerImgUri = "~/img/yes.jpg"
                        }
                    }
                };

                return View["test", artists];
            });
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
