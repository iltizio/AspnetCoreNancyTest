using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Bootstrappers.Autofac;
using NancyTest.Server.Services;

namespace NancyTest.Server
{
    internal sealed class ServerBootstrapper : AutofacNancyBootstrapper
    {
        private readonly IServiceCollection _services;

        public ServerBootstrapper(IServiceCollection services)
        {
            _services = services;
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            base.ConfigureApplicationContainer(container);
            container.Update(builder =>
            {
                builder.Register(d => new FormalHelloService()).Keyed<IHelloService>("Formal").InstancePerLifetimeScope();
                builder.Register(d => new InformalHelloService()).Keyed<IHelloService>("Informal").InstancePerLifetimeScope();
                builder.RegisterType<StaticArtistSearchService>().AsImplementedInterfaces().InstancePerDependency();

                builder.Populate(_services);
            });
        }
    }
}