using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Device.Location;
using NearestPlaces.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NearestPlaces.Controllers
{
    public class PlacesController : ApiController
    {
        private PlaceDB db = new PlaceDB();

        //[ResponseType(typeof(String))]
        public void GetAddress(string address)
        {
            if (address == "")
                return;
            var urlCoordinate = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyCqcpkN_yaOT4Ewn6IwwMbTYLH7zzlluFc";

            var request = WebRequest.Create(urlCoordinate);
            var webResponse = request.GetResponse();
            var stream = webResponse.GetResponseStream();
            var sr = new StreamReader(stream);

            var str = sr.ReadToEnd();

            var data = (JObject)JsonConvert.DeserializeObject(str);

            var coordinateData = data.First.First.First.ToList()[2].First["location"].ToList();


            var sCoord = new GeoCoordinate((double)coordinateData[0].Last, (double)coordinateData[1].Last);

            var coordinate = $"{sCoord.Latitude},{sCoord.Longitude}";

            var urlPlaces = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={coordinate}&radius=500&name=cruise&key=AIzaSyBFbOuj_JGMsSg8lZJ0J7hFeIz0vYTqdcA";

            request = WebRequest.Create(urlPlaces);
            webResponse = request.GetResponse();
            stream = webResponse.GetResponseStream();
            sr = new StreamReader(stream);

            str = sr.ReadToEnd();

            data = (JObject)JsonConvert.DeserializeObject(str);
            var placesData = data["results"];
            foreach (var placeData in placesData)
            {
                var coordinatePlaceData = placeData.First.First.First.First;

                var eCoord = new GeoCoordinate((double)coordinatePlaceData.First.Last, (double)coordinatePlaceData.Last.Last);

                db.Places.Add(new Place { Name = placeData.ToList()[3].Last.ToString(), Coordinates = $"{eCoord.Latitude},{eCoord.Longitude}", Distance = sCoord.GetDistanceTo(eCoord) });
            }
            db.SaveChanges();
            stream.Close();
        }
    }
}