using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using Laboratorio_2____Data_GNOSS;
using System.Formats.Asn1;
using CsvHelper;

namespace Laboratorio_2_GNOSS
{
    internal class Program
    {
        internal static List<string> campos = new List<string>() { "descripción", "imagen", "velocidad", "link wikidata", "nombre común", "interés", "link dbpedia", "interés " };
        static void Main()
        {
            // Ruta del archivo CSV
            //string filePath = "https://raw.githubusercontent.com/JeffSackmann/tennis_atp/a36a13fe21f9d0e8ea45a78b3a425ac9bf7a6991/atp_matches_2003.csv";
            //Obetenemos el archivo desde el directorio de recursos
            string filePath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Resources\Lab_data_alumno.csv");

            // Verificar si el archivo existe, si no existe se sale del programa
            if (!File.Exists(filePath))
            {
                Console.WriteLine("El archivo no existe.");
                Console.Read();
                return;
            }
            Dictionary<string,Animal> animals = new Dictionary<string, Animal>();

            // Leer el archivo CSV línea por línea (Ddelimitador ",")
            char delimiter = ';';
            foreach (string line in File.ReadLines(filePath).Skip(1))
            {
                // Dividir la línea en campos usando el delimitador especificado
                string[] fields = line.Split(delimiter);

                Animal animal;

                // Para cada linea verificamos si no tenemos un objeto ya construido,
                //si lo esta, lo asignamos a la variable animal,
                // si no es asi lo construimos, asignamos y añadimos al diccionario
                if (animals.ContainsKey(fields[0])) { 
                    animal = animals[fields[0]]; 
                } else {
                    animal = new Animal(fields[0]);
                    animals.Add(fields[0], animal); 
                }
                //En esta funcion se implementa la logica de transformacion y normalización de los datos.
                animal.AssingProperty(fields[1], fields[2]);
            }
            WikidataQuery wikidata = new WikidataQuery();

            foreach (Animal animal in animals.Values) {

                wikidata.GetTotalObrasPorAnimal(animal).Wait();
                wikidata.GetSigloMasPopular(animal).Wait();   
                wikidata.GetObrasAnimal(animal).Wait();
                
            }








            //Exportar los resultados obtenidos
            //Construimos el nombre del archivo de salida (Añadimos_output al final del nombre original), si el archivo no tiene exprension se le asigna un nombre generico
            int lastDotIndex = filePath.LastIndexOf('.');
            string outputName = (lastDotIndex != -1) ? filePath.Substring(0, lastDotIndex) + "_output" + filePath.Substring(lastDotIndex) : "Output_Data.csv";
            Task task = ExportCsv<Animal>(animals.Values, outputName);
            //task.Wait();


        }
        //Funcion para exportar los datos en un CSV con subscriptionsCSVwritter
        public static async Task ExportCsv<T>(IEnumerable<T> data, string fileName)
        {
            var subscriptionsWriter = new StreamWriter(fileName);
            var subscriptionsCsvWriter = new CsvWriter(subscriptionsWriter, CultureInfo.InvariantCulture);
            await subscriptionsCsvWriter.WriteRecordsAsync(data);
        }
    }
}