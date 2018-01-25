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
        private readonly IArtistSearchService _artistSearchService;

        public ServerModule(IIndex<string, IHelloService> availableHelloServices, IArtistSearchService artistSearchService)
        {
            _availableHelloServices = availableHelloServices;
            _artistSearchService = artistSearchService;

            HelloWorld();

            SayFormalHello();

            SayInformalHello();

            SearchArtist();
        }

        private void SearchArtist()
        {
            Get("/searchArtist/{artistName}", args =>
            {
                var artists = _artistSearchService.Serach(args.artistName);

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
