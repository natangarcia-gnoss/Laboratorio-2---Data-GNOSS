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
        private readonly HttpClient client
            ;

        public WikidataQuery()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "WikidataQueryBot/1.0");
        }
        /*
        public async Task<JObject> ExecuteQueryAsync(string sparqlQuery)
        {
            var query = Uri.EscapeDataString(sparqlQuery);
            var url = $"{WIKIDATA_ENDPOINT}?query={query}&format=json";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return JObject.Parse(content);
        }
        */
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
            if (String.IsNullOrEmpty(animal.GetWikidataId())) { return; }

            string query = "SELECT DISTINCT ?obra ?obraLabel WHERE { " +
                        "?obra wdt:P195 wd:Q160112; " +
                        "wdt:P180 wd:" + animal.GetWikidataId() + ". " +
                        "SERVICE wikibase:label { " +
                        "bd:serviceParam wikibase:language \"[AUTO_LANGUAGE],es,en\". } " +
                        "}";
            /*
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
            
            */
            
           

            var requestUrl = $"https://query.wikidata.org/sparql?query={query}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");

            var response = await client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se ha podido encontrar la consulta para {animal.Name} con ID {animal.GetWikidataId()}");
                Console.WriteLine(ex);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            

        }
        public async Task GetTotalObrasPorAnimal(Animal animal) {
            if (String.IsNullOrEmpty(animal.GetWikidataId())) { return; }
            string query = "SELECT DISTINCT (COUNT(DISTINCT ?obra ) AS ?TOTAL_OBRAS) WHERE { "+ 
                "?obra wdt:P195 wd:Q160112; "+
                        "wdt:P180 wd:"+animal.GetWikidataId()+"; "+  
                        "wdt:P31 wd:Q3305213. " +
                        "SERVICE wikibase:label { bd:serviceParam wikibase:language \"[AUTO_LANGUAGE],es,en\". } }";
            

            string requestUrl = "https://query.wikidata.org/sparql?query=" + query;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var response = await client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se ha podido encontrar la consulta para {animal.Name} con ID {animal.GetWikidataId()}");
                Console.WriteLine(ex);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());


        }


        public async Task GetSigloMasPopular(Animal animal)
        {
            if (String.IsNullOrEmpty(animal.GetWikidataId())) { return; }
            string query = "SELECT ?siglo (COUNT(DISTINCT ?obra) AS ?count) WHERE {"+
                " ?obra wdt:P195 wd:Q160112."+   //Obras del Museo del Prado
                "?obra wdt:P180 wd:"+animal.GetWikidataId()+".  "+ // Obras que representan al animal específico
                "?obra wdt:P571 ?fecha. "+     //# Fecha de creación de la obra
                "BIND(CEIL(YEAR(?fecha)/100) AS ?siglo) "+ //Calculamos el siglo al que pertenece
                "SERVICE wikibase:label { bd:serviceParam wikibase:language \"[AUTO_LANGUAGE],es,en\". } }"+ 
                " GROUP BY ?siglo "+ //Agrupamos por siglo para hacer luego un count
                " ORDER BY DESC(?count) "+ //Ordenamos por el count descendiente es decir, nos devolvera el mas popular el primero
                "LIMIT 1"; //Tomamos solo el mas popular

            string requestUrl = "https://query.wikidata.org/sparql?query=" + query;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var response = await client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se ha podido encontrar la consulta para {animal.Name} con ID {animal.GetWikidataId()}");
                Console.WriteLine(ex);
            }

            Console.WriteLine(await response.Content.ReadAsStringAsync());


        }






    }
}
