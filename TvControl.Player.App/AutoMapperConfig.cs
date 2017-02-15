using System;
using System.IO;
using System.Windows.Media.Imaging;

using AutoMapper;

using TvControl.Player.App.Api;
using TvControl.Player.App.Api.Models.Response;

namespace TvControl.Player.App
{
    internal class AutoMapperConfig
    {

        public static void Init(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<TvStation, ApiTvStation>().ForMember(station => station.Image, exp => exp.Ignore());
        }

    }
}