using System;
using System.Net.Http;
using System.Reflection.Emit;
using System.Reflection.Metadata;
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

        public async Task GetPropertyFromWikiData(Animal animal, string propertyCode, string property)
        {
            Console.WriteLine($"CONSULTA {propertyCode} SOBRE {animal.Name} ID: {animal.GetWikidataId()}");
            string id = animal.GetWikidataId();
            if (!String.IsNullOrEmpty(id))
            {
                string space = "wd";
                string query = "SELECT *" +
                " WHERE {" +
                "wd:" + id + " "+space+":" + propertyCode + " ?value. " +
                "bd:serviceParam wikibase:language \"[AUTO_LANGUAGE],es,en\". } " +
                "}";


                var requestUrl = $"https://query.wikidata.org/sparql?query={query}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("Accept", "application/json");

                var response = await client.SendAsync(request);
                try
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();  //Leemos el contenido
                    JObject obj = JObject.Parse(content); //Pareseamos el contenido a un JObject
                    JToken bindings = obj["results"]["bindings"];  //Accedemos a los bindings de la respuesta
                    Console.WriteLine(bindings.ToString());
                    if (bindings == null || bindings.First == null) //Si estos no tienen datos dejamos el valor por defecto "No especificado" 
                    {
                        
                        Console.WriteLine("NO SE HA OBTENIDO RESULTADO");
                        return;
                    }
                    else
                    {
                        string value = bindings[0]["value"]["value"].ToString();
                        animal.AssingProperty(property, value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"No se ha podido encontrar la consulta {propertyCode} para {animal.Name} con ID {animal.GetWikidataId()}");
                    Console.WriteLine(ex);
                }
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else{
                Console.WriteLine($"No se ha podido obtener el link de {animal.Name} propiedad {property}");
            }
            





        }
        public async Task GetObrasAnimal(Animal animal)
        {
            Console.WriteLine($"CONSULTA OBRAS SOBRE {animal.Name} ID: {animal.GetWikidataId()}");
            if (String.IsNullOrEmpty(animal.GetWikidataId())) { return; }

            string query = "SELECT DISTINCT ?obra ?obraLabel WHERE { " +
                        "?obra wdt:P195 wd:Q160112; " +
                        "wdt:P180 wd:" + animal.GetWikidataId() + ". " +
                        "SERVICE wikibase:label { " +
                        "bd:serviceParam wikibase:language \"[AUTO_LANGUAGE],es,en\". } " +
                        "}";
            
           

            var requestUrl = $"https://query.wikidata.org/sparql?query={query}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");

            var response = await client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();  //Leemos el contenido
                JObject obj = JObject.Parse(content); //Pareseamos el contenido a un JObject
                JToken bindings = obj["results"]["bindings"];  //Accedemos a los bindings de la respuesta
                Console.WriteLine(bindings.ToString());
                if (bindings == null || bindings.First == null) //Si estos no tienen datos dejamos el valor por defecto "No especificado" 
                {
                    animal.add_numeroObrasEnPinacoteca(0);
                    Console.WriteLine("NO SE HA OBTENIDO RESULTADO");
                    return;
                }
                else
                {
                    int index = 0;
                    foreach (JToken bind in bindings) {//Aqui ya sabemos que al menos tiene un elemento por el condicional anterior 
                        string obra = bindings[index]["obraLabel"]["value"].ToString();  //Obtenemos el valor del TOTAL de obras
                        animal.add_obrasRelacionadas(obra); //Lo parseamos a int y asignamos
                        Console.WriteLine($"Se ha resuelto la consulta OBRAS: {obra}");
                        index++;
                    }
                    
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se ha podido encontrar la consulta OBRAS para {animal.Name} con ID {animal.GetWikidataId()}");
                Console.WriteLine(ex);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            

        }
        public async Task GetTotalObrasPorAnimal(Animal animal) {
            Console.WriteLine($"CONSULTA TOTAL OBRAS SOBRE {animal.Name} ID: {animal.GetWikidataId()}");
            if (String.IsNullOrEmpty(animal.GetWikidataId())) { return; }
            string query = "SELECT DISTINCT (COUNT(DISTINCT ?obra ) AS ?TOTAL_OBRAS) WHERE { "+ //Contamos las instancias devueltas
                           "?obra wdt:P195 wd:Q160112; "+ //Obras del prado
                        "wdt:P180 wd:"+animal.GetWikidataId()+"; "+  //En las que este retratado el animal
                        "wdt:P31 wd:Q3305213. " + //instancias de pinturas
                        "SERVICE wikibase:label { bd:serviceParam wikibase:language \"[AUTO_LANGUAGE],es,en\". } }";
            

            string requestUrl = "https://query.wikidata.org/sparql?query=" + query;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var response = await client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();  //Leemos el contenido
                JObject obj = JObject.Parse(content); //Pareseamos el contenido a un JObject
                JToken bindings = obj["results"]["bindings"];  //Accedemos a los bindings de la respuesta
                if (bindings == null || bindings.First == null) //Si estos no tienen datos dejamos el valor por defecto "No especificado" 
                {
                    animal.add_numeroObrasEnPinacoteca(0);
                    Console.WriteLine("NO SE HA OBTENIDO RESULTADO");
                    return;
                }
                else
                {
                    bindings = bindings.First; //Aqui ya sabemos que al menos tiene un elemento por el condicional anterior 
                    string total = bindings["TOTAL_OBRAS"]["value"].ToString();  //Obtenemos el valor del TOTAL de obras
                    animal.add_numeroObrasEnPinacoteca(int.Parse(total)); //Lo parseamos a int y asignamos
                    Console.WriteLine($"Se ha resuelto la consulta TOTAL: {total}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se ha podido encontrar la consulta TOTAL para {animal.Name} con ID {animal.GetWikidataId()}");
                Console.WriteLine(ex);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());

        }


        public async Task GetSigloMasPopular(Animal animal)
        {
            Console.WriteLine($"CONSULTA SIGLO SOBRE {animal.Name} ID: {animal.GetWikidataId()}");
            if (String.IsNullOrEmpty(animal.GetWikidataId()) ||animal.GetWikidataId().Equals("Link no especificado")) { return; }
            string query = "SELECT ?siglo (COUNT(DISTINCT ?obra) AS ?count) WHERE {"+
                " ?obra wdt:P195 wd:Q160112."+   //Obras del Museo del Prado
                " ?obra wdt:P180 wd:"+ animal.GetWikidataId()+ ".  "+ // Obras que representan al animal específico
                " ?obra wdt:P571 ?fecha. "+     //# Fecha de creación de la obra
                " BIND(CEIL(YEAR(?fecha)/100) AS ?siglo) "+ //Calculamos el siglo al que pertenece
                " SERVICE wikibase:label { bd:serviceParam wikibase:language \"[AUTO_LANGUAGE],es,en\". } }"+ 
                " GROUP BY ?siglo "+ //Agrupamos por siglo para hacer luego un count
                " ORDER BY DESC(?count) "+ //Ordenamos por el count descendiente es decir, nos devolvera el mas popular el primero
                " LIMIT 1"; //Tomamos solo el mas popular

            string requestUrl = "https://query.wikidata.org/sparql?query=" + query;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept", "application/json");
            var response = await client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode(); //Verificamos que la respuesta tiene codigo de estado aceptado
                var content = await response.Content.ReadAsStringAsync();  //Leemos el contenido
                JObject obj = JObject.Parse(content); //Pareseamos el contenido a un JObject
                JToken bindings = obj["results"]["bindings"];  //Accedemos a los bindings de la respuesta
                string siglo = "No Especificado";
                if (bindings == null || bindings.First == null) //Si estos no tienen datos dejamos el valor por defecto "No especificado" 
                {
                    animal.add_sigloDePopularidad(siglo);
                    Console.WriteLine("NO SE HA OBTENIDO RESULTADO");   
                    return;
                }
                else
                {
                    bindings = bindings.First; //Aqui ya sabemos que al menos tiene un elemento por el condicional anterior y por la consulta "LIMIT 1" sabemos que tiene como maximo uno, loo tomamos
                    siglo = bindings["siglo"]["value"].ToString();  //Obtenemos el valor del siglo
                    animal.add_sigloDePopularidad(ConvertirARomanos(int.Parse(siglo))); //Lo parseamos a numero romano y asignamos a animal mediante la funcion apropiada
                    Console.WriteLine($"Se ha resuelto la consulta SIGLO: {siglo}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se ha podido encontrar la consulta SIGLO para {animal.Name} con ID {animal.GetWikidataId()}");
                Console.WriteLine(ex);
            }

            Console.WriteLine(await response.Content.ReadAsStringAsync());


        }

        private static string ConvertirARomanos(int numero)
        {

            if (numero < 1 || numero > 100)
                throw new ArgumentOutOfRangeException("El número debe estar entre 1 y 3999");

            string[] decenas = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
            string[] unidades = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

            // Dividimos el número en unidades, decenas, centenas y miles
            string romano = decenas[(numero % 100) / 10] +
                            unidades[numero % 10];

            return romano;
        }




    }
}
