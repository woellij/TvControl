using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Sockets.Plugin;
using Sockets.Plugin.Abstractions;

namespace TvControl.ConsoleApp
{
    public class UDPListener
    {

        private readonly int listenPort;
        private readonly Action<string> log;

        public UDPListener(int listenPort, Action<string> log)
        {
            this.listenPort = listenPort;
            this.log = log;
        }

        public async Task StartAsync()
        {
            List<CommsInterface> interfaces = await CommsInterface.GetAllInterfacesAsync();

            var receiver = new UdpSocketReceiver();

            receiver.MessageReceived += this.ReceiverOnMessageReceived;

            try {
                // listen for udp traffic on listenPort
                await receiver.StartListeningAsync(this.listenPort);
                this.log($"listening on port {this.listenPort}");
            }
            catch (Exception e) {
                this.log($"failed to listen on port {this.listenPort}.{Environment.NewLine}{e}");
            }
        }

        private void ReceiverOnMessageReceived(object sender, UdpSocketMessageReceivedEventArgs args)
        {
            // get the remote endpoint details and convert the received data into a string
            string from = $"{args.RemoteAddress}:{args.RemotePort}";
            string data = Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);

            var content = "roger";
            new UdpSocketClient().SendToAsync(Encoding.UTF8.GetBytes(content), args.RemoteAddress, 11010);

            this.log($"received from {from}; message: '{data}'");
        }

    }
}