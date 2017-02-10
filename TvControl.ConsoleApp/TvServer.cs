using System;

using Microsoft.Owin.Hosting;

namespace TvControl.ConsoleApp
{
    public class TvServer
    {

        private readonly Action<string> logAction;

        private readonly int udpPort;
        private UDPListener udpListener;
        private IDisposable webApp;

        public TvServer(int udpPort, Action<string> logAction)
        {
            this.udpPort = udpPort;
            this.logAction = logAction;
        }

        public async void Run()
        {
            this.udpListener = new UDPListener(this.udpPort, s => this.logAction($"UDPListener: {s}"));

            await this.udpListener.StartAsync();

            // Start OWIN host 
            var startOptions = new StartOptions();
            this.webApp = WebApp.Start<Startup>(startOptions);
        }

    }
}