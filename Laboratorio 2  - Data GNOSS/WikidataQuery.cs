using System;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Laboratorio_2____Data_GNOSS
{
    internal class WikidataQuery
    {
        private const string WIKIDATA_ENDPOINT = "https://query.wikidata.org/sparql";
        private readonly HttpClient _httpClient;

        public WikidataQuery()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WikidataQueryBot/1.0");
        }

        public async Task<JObject> ExecuteQueryAsync(string sparqlQuery)
        {
            var query = Uri.EscapeDataString(sparqlQuery);
            var url = $"{WIKIDATA_ENDPOINT}?query={query}&format=json";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return JObject.Parse(content);
        }

        public void ProcessResults(JObject results)
        {
            var bindings = results["results"]["bindings"];

            foreach (var binding in bindings)
            {
                // Procesa cada resultado aquí
                // Por ejemplo:
                var itemLabel = binding["itemLabel"]["value"].ToString();
               Console.WriteLine(itemLabel);
            }
        }


        public async Task GetObrasAnimal(Animal animal)
        {
            string query = "SELECT DISTINCT ?obra ?obraLabel WHERE { " +
                        "?obra wdt:P195 wd:Q160112; " +
                        "wdt:P180 wd:" + animal.GetWikidataId() + ". " +
                        "SERVICE wikibase:label { " +
                        "bd:serviceParam wikibase:language \"[AUTO_LANGUAGE],es,en\". } " +
                        "}";

            var options = new RestClientOptions("https://query.wikidata.org")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(query, Method.Get);
            request.AddHeader("Accept", "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine(response.Content);

            Console.Read();
            
            
            
            /*

            var requestUrl = $"https://query.wikidata.org/sparql?query={query}";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            */

        }










    }
}
