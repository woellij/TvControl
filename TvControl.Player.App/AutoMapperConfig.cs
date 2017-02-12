using System;
using System.IO;
using System.Windows.Media.Imaging;

using AutoMapper;

using TvControl.Player.App.Api.Models.Response;

namespace TvControl.Player.App
{
    internal class AutoMapperConfig
    {

        public static void Init(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<TvStation, ApiTvStation>().ForMember(station => station.ImageBase64, exp => exp.ResolveUsing(station =>
            {
                var encoder = new PngBitmapEncoder() {
                    
                };
                var stationImage = (BitmapSource) station.Image;
                encoder.Frames.Add(BitmapFrame.Create(stationImage));
                
                var stream = new MemoryStream();
                encoder.Save(stream);
                byte[] bitmapdata = stream.ToArray();

                return Convert.ToBase64String(bitmapdata);
            }));
        }

    }
}