using Nancy;
using Nancy.Linker;
using Nancy.Routing;
using Nancy.TinyIoc;

namespace TvControl.Player.App
{
    internal class CustomNancyBoostrapper : DefaultNancyBootstrapper
    {

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<IResourceLinker>(
                (x, overloads) =>
                    new ResourceLinker(x.Resolve<IRouteCacheProvider>(),
                        x.Resolve<IRouteSegmentExtractor>(), x.Resolve<IUriFilter>()));

            base.ConfigureApplicationContainer(container);
        }

    }
}