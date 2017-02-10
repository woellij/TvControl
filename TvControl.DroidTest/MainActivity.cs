using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Util;

using Sockets.Plugin;
using Sockets.Plugin.Abstractions;

namespace TvControl.DroidTest
{
    [Activity(Label = "TvControl.DroidTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        public UdpSocketReceiver BackingReceiver { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.StartSending();
            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }

        private async void StartSending()
        {
            this.BackingReceiver = new UdpSocketReceiver();

            this.BackingReceiver.MessageReceived += this.BackingReceiverOnMessageReceived;
            await this.BackingReceiver.StartListeningAsync(11010);

            while (true) {
                //List<CommsInterface> interfaces = await CommsInterface.GetAllInterfacesAsync();
                //interfaces = interfaces.Where(i => i.IsUsable && !i.IsLoopback).ToList();
                //foreach (CommsInterface ci in interfaces) {
                //    await this.BackingReceiver.SendToAsync(new byte[0], ci.BroadcastAddress, 11011);
                //}

                await this.BackingReceiver.SendToAsync(new byte[0], "255.255.255.255", 11011);
                await Task.Delay(500);
            }
        }

        private void BackingReceiverOnMessageReceived(object sender, UdpSocketMessageReceivedEventArgs args)
        {
            Log.Info("UDP", $"from {args.RemoteAddress}:{args.RemotePort}; msg: {Encoding.UTF8.GetString(args.ByteData)}");
        }

    }
}