namespace PoolIt.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Contracts;
    using Newtonsoft.Json;

    public class LocationService : ILocationService
    {
        private const string TownsFileName = "towns.json";

        private Town[] towns;

        public LocationService()
        {
            this.LoadFromFile(TownsFileName);
        }

        public async Task<string> GetTownNameAsync(double latitude, double longitude)
        {
            var name = await Task.Run(() =>
            {
                var smallestDistance = double.MaxValue;
                string nearestTownName = null;

                foreach (var town in this.towns)
                {
                    var distance = Math.Sqrt(Math.Abs(latitude - town.Latitude)
                                             + Math.Abs(longitude - town.Longitude));

                    if (smallestDistance > distance)
                    {
                        smallestDistance = distance;
                        nearestTownName = town.Name;
                    }
                }

                return nearestTownName;
            });

            return name;
        }

        private void LoadFromFile(string fileName)
        {
            var jsonText = File.ReadAllText(fileName);

            this.towns = JsonConvert.DeserializeObject<Town[]>(jsonText);
        }

        private class Town
        {
            public string Name { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}