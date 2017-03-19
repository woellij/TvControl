using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

using AutoMapper;

using Nancy;
using Nancy.Linker;

using TvControl.Player.App.Api.Models;
using TvControl.Player.App.Api.Models.Response;
using TvControl.Player.App.Model;
using TvControl.Player.App.ViewModels;

namespace TvControl.Player.App.Api.Modules
{
    public class TvStationsModule : NancyModule
    {

        private readonly IResourceLinker resourceLinker;

        public TvStationsModule(ITvStations tvStations, IResourceLinker resourceLinker)
            : base("tvstations")
        {
            this.resourceLinker = resourceLinker;
            this.Get["/", true] = async (o, token) =>
            {
                TvStation[] stations = await tvStations.GetAllAsync();
                List<ApiTvStation> apiTvStations = stations.Select(station => this.MapStation(station)).ToList();
                TvStation selectedStation = TvControlViewModel.Current.SelectedStation;
                return new {
                    Stations = apiTvStations,
                    Selected = selectedStation == null ? null : this.MapStation(selectedStation),
                    TvControlViewModel.Current.SelectedIndex
                };
            };

            this.Post["/{id}"] = o =>
            {
                TvControlViewModel tvControlViewModel = TvControlViewModel.Current;
                if (!tvControlViewModel.SetCurrentStation(o.id)) {
                    return InvalidStationIdResponse(o.id);
                }

                return HttpStatusCode.Accepted;
            };

            this.Get["StationImageRoute", "/image/{id}"] = o =>
            {
                dynamic id = o.id;
                TvStation station = TvControlViewModel.Current.TvStations.FirstOrDefault(s => s.Id == id);
                var encoder = new PngBitmapEncoder();
                if (station == null) {
                    return InvalidStationIdResponse(o.id);
                }

                var stationImage = (BitmapSource) station.Image;
                encoder.Frames.Add(BitmapFrame.Create(stationImage));

                var stream = new MemoryStream();
                encoder.Save(stream);
                byte[] bitmapdata = stream.ToArray();
                return new ByteArrayResponse(bitmapdata, "image/png");
            };
        }

        private static dynamic InvalidStationIdResponse(string id)
        {
            var r = (Response) $"invalid station id: {id}";
            r.StatusCode = HttpStatusCode.BadRequest;
            return r;
        }

        private ApiTvStation MapStation(TvStation station)
        {
            var apiTvStation = Mapper.Map<ApiTvStation>(station);
            dynamic parameters = new { id = station.Id };
            Uri image = this.resourceLinker.BuildAbsoluteUri(this.Context, "StationImageRoute", parameters);
            apiTvStation.Image = image.OriginalString;
            return apiTvStation;
        }

    }
}