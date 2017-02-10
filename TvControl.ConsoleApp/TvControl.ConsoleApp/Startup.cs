using System;

using Nancy.Owin;

using Owin;

namespace TvControl.ConsoleApp
{
    public class Startup
    {

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            try
            {
                appBuilder.UseNancy(new NancyOptions { Bootstrapper = new CustomBootstrapper() });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}