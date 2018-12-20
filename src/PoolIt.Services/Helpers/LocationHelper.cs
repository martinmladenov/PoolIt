namespace PoolIt.Services.Helpers
{
    using System;
    using System.IO.Abstractions;
    using System.Threading.Tasks;
    using Contracts;
    using Newtonsoft.Json;

    public class LocationHelper : ILocationHelper
    {
        private const string TownsFileName = "towns.json";

        private Town[] towns;

        private readonly IFileSystem fileSystem;

        public LocationHelper(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
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
            var jsonText = this.fileSystem.File.ReadAllText(fileName);

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