using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Nancy;

using TvControl.Player.App.Api.Models.Response;

namespace TvControl.Player.App.Api
{
    public class TvStationsModule : NancyModule
    {

        public TvStationsModule(ITvStations tvStations)
            : base("tvstations")
        {
            this.Get["getAll", "/", true] = async (o, token) =>
            {
                TvStation[] stations = await tvStations.GetAllAsync();
                List<ApiTvStation> apiTvStations = stations.Select(station => Mapper.Instance.Map<ApiTvStation>(station)).ToList();
                return apiTvStations;
            };

            this.Post["setCurrent", "/set/{id}"] = o =>
            {
                TvControlViewModel tvControlViewModel = TvControlViewModel.Current;
                if (!tvControlViewModel.SetCurrentStation(o.id)) {
                    var r = (Response) "invalid station id";
                    r.StatusCode = HttpStatusCode.BadRequest;
                    return r;
                }

                return HttpStatusCode.Accepted;
            };
        }

    }
}