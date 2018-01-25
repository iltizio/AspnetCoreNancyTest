using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using FluentAssertions;
using Moq;
using Nancy;
using Nancy.Helpers;
using Nancy.Testing;
using NancyTest.Server.Models;
using NancyTest.Server.NancyModules;
using NancyTest.Server.Services;
using NUnit.Framework;

namespace NancyTest.Server.Test.NancyModules
{
    [TestFixture]
    public class ServerModuleTest
    {
        [Test]
        public async Task ServerModule_HelloWorld_Ok()
        {
            //Arrange
            var fakeHelloServices = Mock.Of<IIndex<string, IHelloService>>();
            var fakeArtistSearchService = Mock.Of<IArtistSearchService>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                var module = new ServerModule(fakeHelloServices, fakeArtistSearchService);
                with.Module(module);
            });

            var browser = new Browser(bootstrapper, defaults: to => to.Accept("application/json"));

            //Act
            var result = await browser.Get("/", with =>
            {
                with.HttpRequest();
            });

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Body.AsString().Should().Be("Hello World");
        }

        [Test]
        public async Task ServerModule_SayFormalHello_NullName_ShouldThrowException()
        {
            //Arrange
            var fakeHelloServices = Mock.Of<IIndex<string, IHelloService>>();
            var fakeArtistSearchService = Mock.Of<IArtistSearchService>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                var module = new ServerModule(fakeHelloServices, fakeArtistSearchService);
                with.Module(module);
            });

            var browser = new Browser(bootstrapper, defaults: to => to.Accept("application/json"));

            //Act
            var result = await browser.Get("/SayHello", with =>
            {
                with.HttpRequest();
            });

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Body.AsString().Should().Be("A name must be specified");
        }

        [Test]
        public async Task ServerModule_SayFormalHello_EmptyName_ShouldThrowException()
        {
            //Arrange
            var fakeHelloServices = Mock.Of<IIndex<string, IHelloService>>();
            var fakeArtistSearchService = Mock.Of<IArtistSearchService>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                var module = new ServerModule(fakeHelloServices, fakeArtistSearchService);
                with.Module(module);
            });

            var browser = new Browser(bootstrapper, defaults: to => to.Accept("application/json"));

            //Act
            var result = await browser.Get("/SayHello", with =>
            {
                with.Query("name", string.Empty);
                with.HttpRequest();
            });

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Body.AsString().Should().Be("A name must be specified");
        }

        [Test]
        public async Task ServerModule_SayFormalHello_Ok()
        {
            //Arrange
            var name = "John Doe";

            var fakeHelloServices = Mock.Of<IIndex<string, IHelloService>>();
            var helloMessage = "Hello Test";
            var fakeHelloService = Mock.Of<IHelloService>(it => it.Hello(It.IsAny<string>()) == helloMessage);
            Mock.Get(fakeHelloServices).Setup(it => it.TryGetValue(It.IsAny<string>(), out fakeHelloService))
                .Returns(true);

            var fakeArtistSearchService = Mock.Of<IArtistSearchService>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                var module = new ServerModule(fakeHelloServices, fakeArtistSearchService);
                with.Module(module);
            });

            var browser = new Browser(bootstrapper, defaults: to => to.Accept("application/json"));

            //Act
            var result = await browser.Get("/SayHello", with =>
            {
                with.Query("name", name);
                with.HttpRequest();
            });

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Body.AsString().Should().Be(helloMessage);
            Mock.Get(fakeHelloServices).Verify(it => it.TryGetValue(It.Is<string>(p => p == "Formal"), out fakeHelloService), Times.Once);
            Mock.Get(fakeHelloService).Verify(it => it.Hello(It.Is<string>(p => p == name)), Times.Once);
        }

        [Test]
        public async Task ServerModule_SayFormalHello_NoHelloServiceImplementation_ShouldThrowException()
        {
            //Arrange
            var name = "John Doe";

            var fakeHelloServices = Mock.Of<IIndex<string, IHelloService>>();
            var fakeHelloService = default(IHelloService);
            Mock.Get(fakeHelloServices).Setup(it => it.TryGetValue(It.IsAny<string>(), out fakeHelloService))
                .Returns(false);

            var fakeArtistSearchService = Mock.Of<IArtistSearchService>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                var module = new ServerModule(fakeHelloServices, fakeArtistSearchService);
                with.Module(module);
            });

            var browser = new Browser(bootstrapper, defaults: to => to.Accept("application/json"));

            //Act
            var result = await browser.Get("/SayHello", with =>
            {
                with.Query("name", name);
                with.HttpRequest();
            });

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.Body.AsString().Should().Be("No HelloService implementation was found");
            Mock.Get(fakeHelloServices).Verify(it => it.TryGetValue(It.Is<string>(p => p == "Formal"), out fakeHelloService), Times.Once);
        }

        [Test]
        public async Task ServerModule_SayInformalHello_Ok()
        {
            //Arrange
            var name = "John Doe";

            var fakeHelloServices = Mock.Of<IIndex<string, IHelloService>>();
            var helloMessage = "Hello Test";
            var fakeHelloService = Mock.Of<IHelloService>(it => it.Hello(It.IsAny<string>()) == helloMessage);
            Mock.Get(fakeHelloServices).Setup(it => it.TryGetValue(It.IsAny<string>(), out fakeHelloService))
                .Returns(true);

            var fakeArtistSearchService = Mock.Of<IArtistSearchService>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                var module = new ServerModule(fakeHelloServices, fakeArtistSearchService);
                with.Module(module);
            });

            var browser = new Browser(bootstrapper, defaults: to => to.Accept("application/json"));

            //Act
            BrowserResponse result = null;
            Func<Task> act = async() => result = await browser.Get($"/SayHello2/{HttpUtility.UrlEncode(name)}", with =>
            {
                with.HttpRequest();
            });

            //Assert
            act.ShouldNotThrow();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Body.AsString().Should().Be(helloMessage);
            Mock.Get(fakeHelloServices).Verify(it => it.TryGetValue(It.Is<string>(p => p == "Informal"), out fakeHelloService), Times.Once);
            Mock.Get(fakeHelloService).Verify(it => it.Hello(It.Is<string>(p => p == name)), Times.Once);
        }

        [Test]
        public async Task ServerModule_SayInformalHello_NoHelloServiceImplementation_ShouldThrowException()
        {
            //Arrange
            var name = "John Doe";

            var fakeHelloServices = Mock.Of<IIndex<string, IHelloService>>();
            var fakeHelloService = default(IHelloService);
            Mock.Get(fakeHelloServices).Setup(it => it.TryGetValue(It.IsAny<string>(), out fakeHelloService))
                .Returns(false);

            var fakeArtistSearchService = Mock.Of<IArtistSearchService>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                var module = new ServerModule(fakeHelloServices, fakeArtistSearchService);
                with.Module(module);
            });

            var browser = new Browser(bootstrapper, defaults: to => to.Accept("application/json"));

            //Act
            BrowserResponse result = null;
            Func<Task> act = async () => result = await browser.Get($"/SayHello2/{HttpUtility.UrlEncode(name)}", with =>
            {
                with.Query("name", name);
                with.HttpRequest();
            });

            //Assert
            act.ShouldNotThrow();
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.Body.AsString().Should().Be("No HelloService implementation was found");
            Mock.Get(fakeHelloServices).Verify(it => it.TryGetValue(It.Is<string>(p => p == "Informal"), out fakeHelloService), Times.Once);
        }

        [Test]
        public async Task ServerModule_SearchArtist_Ok()
        {
            //Arrange
            var artistName = "Test";

            var fakeHelloServices = Mock.Of<IIndex<string, IHelloService>>();

            var fakeArtistSerachResult = new ArtistSearchModel
            {
                Artists = new List<Artist>
                {
                    new Artist
                    {
                        Name = "Artist1",
                        BannerImgUri = "/img1.png"
                    },
                    new Artist
                    {
                        Name = "Artist2",
                        BannerImgUri = "/img2.png"
                    }
                }
            };
            var fakeArtistSearchService = Mock.Of<IArtistSearchService>();
            Mock.Get(fakeArtistSearchService).Setup(it => it.Serach(It.IsAny<string>()))
                .Returns(fakeArtistSerachResult);

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.RootPathProvider<TestingRootPathProvider>();
                with.ViewFactory<TestingViewFactory>();
                var module = new ServerModule(fakeHelloServices, fakeArtistSearchService);
                with.Module(module);
            });

            var browser = new Browser(bootstrapper, defaults: to => to.Accept("application/json"));

            //Act
            var result = await browser.Get($"/searchArtist/{artistName}", with =>
            {
                with.HttpRequest();
                with.Header("accept", "text/html");
            });

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.GetViewName().Should().Be("test");
            result.GetModel<ArtistSearchModel>().Artists.Any().Should().BeTrue();
            result.GetModel<ArtistSearchModel>().Should().Be(fakeArtistSerachResult);
            Mock.Get(fakeArtistSearchService).Verify(it => it.Serach(It.Is<string>(p => p == artistName)), Times.Once);
        }
    }

    public class TestingRootPathProvider : IRootPathProvider
    {
        private static readonly string RootPath;

        static TestingRootPathProvider()
        {
            var directoryName = Path.GetDirectoryName(typeof(ServerModule).Assembly.CodeBase);

            if (directoryName != null)
            {
                var assemblyPath = directoryName.Replace(@"file:\", string.Empty);
                RootPath = assemblyPath; //Path.Combine(assemblyPath, "..", "..", "..", "Escape.Web");
            }
        }

        public string GetRootPath()
        {
            return RootPath;
        }
    }
}
