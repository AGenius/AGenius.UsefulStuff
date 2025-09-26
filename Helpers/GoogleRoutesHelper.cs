using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AGenius.UsefulStuff.Helpers
{
    public class GoogleRoutesHelper
    {
        private readonly string _apiKey;
        private readonly string _apiEndpoint;
        private readonly HttpClient _httpClient;

        public GoogleRoutesHelper(string apiKey, string endpoint)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _apiEndpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Calls Google Routes API to compute the optimal delivery sequence.
        /// Postcodes must already be in "lat,lng" format or resolvable addresses.
        /// </summary>
        public async Task<string> ComputeOptimizedRouteAsync(string shopAddress, List<string> postcodes)
        {
            if (postcodes == null || postcodes.Count < 2)
                throw new ArgumentException("At least 2 addresses are required");
            try
            {
                var request = new
                {
                    origin = new { address = shopAddress },
                    destination = new { address = shopAddress },
                    intermediates = postcodes.Select(pc => new { address = pc }).ToList(),
                    travelMode = "DRIVE",
                    routingPreference = "TRAFFIC_AWARE",
                    optimizeWaypointOrder = true,
                    computeAlternativeRoutes = false
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                content.Headers.Add("X-Goog-Api-Key", _apiKey);
                // This mask can be adjusted to return more or less information as needed
                //content.Headers.Add("X-Goog-FieldMask", "routes.optimizedIntermediateWaypointIndex,routes.distanceMeters,routes.duration,routes.legs");
                content.Headers.Add("X-Goog-FieldMask", "routes.optimizedIntermediateWaypointIndex");

                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}