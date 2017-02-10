using Nancy;
using Nancy.Bootstrapper;
using Nancy.Json;
using Nancy.TinyIoc;

namespace TvControl.ConsoleApp
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // Enable Compression with Default Settings
            JsonSettings.MaxJsonLength = 200000000;
            base.ApplicationStartup(container, pipelines);
        }

    }
}