﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio_2____Data_GNOSS
{
    public class Animal
    {

        // Propiedades de la clase Animal
        public string Name { get; set; }

        // Descripción del animal
        public string Descripcion { get; set; }

        // URL de la imagen del animal
        public string Imagen { get; set; }

        // Velocidad del animal (en km/h, por ejemplo)
        public double Velocidad { get; set; }

        // Enlace a Wikidata del animal
        public string LinkWikidata { get; set; }

        // Nombre común del animal
        public string NombreComun { get; set; }

        // Nivel de interés en el animal (puede ser una escala numérica o textual)
        public string Interes { get; set; }
        private static string[] nivelesInteres = { "Alto", "Medio", "Bajo" };

        // Enlace a la página de DBpedia del animal
        public string LinkDbpedia { get; set; }
        //Nombre de las obra del Museo del Prado en las que aparece ese animal
        public string ObrasRelacionadas { get; set; }
        //Obras en las que aparece el animal en la pinacoteca
        public string ObrasPinacoteca { get; set; }
        //Siglo en el que ese animal ha sido más popular
        public string SigloDePopularidad { get; set; }
        // Constructor por defecto (Los valores que no admiten null se iniciaran a sus valores por defecto)
        public Animal(string name) {

            Name = name;
            Descripcion = "No hay descripción disponible";
            Imagen = "URL por defecto de imagen";
            Velocidad = 0.0;
            LinkWikidata = "Link no especificado";
            NombreComun = "Nombre común no disponible";
            Interes = "Desconocido";
            LinkDbpedia = "Link no especificado";
            ObrasRelacionadas = "Ninguna obra relacionada";
            ObrasPinacoteca = "Ninguna obra en la pinacoteca";
            SigloDePopularidad = "No disponible";

        }

        // Constructor que inicializa todas las propiedades
        public Animal(string name, string descripcion, string imagen, double velocidad, string linkWikidata, string nombreComun, string interes, string linkDbpedia, string obrasRelacionadas, string obrasPinacoteca, string sigloDepopularidad)
        {
            Name = name;    
            Descripcion = descripcion;
            Imagen = imagen;
            Velocidad = velocidad;
            LinkWikidata = linkWikidata;
            NombreComun = nombreComun;
            Interes = interes;
            LinkDbpedia = linkDbpedia;
            ObrasRelacionadas = obrasRelacionadas;
            ObrasPinacoteca = obrasPinacoteca;
            SigloDePopularidad = sigloDepopularidad;

        }

        // Método para mostrar la información del animal
        public void MostrarInfo()
        {
            Console.WriteLine($"Nombre Común: {NombreComun}");
            Console.WriteLine($"Descripción: {Descripcion}");
            Console.WriteLine($"Velocidad: {Velocidad} km/h");
            Console.WriteLine($"Interés: {Interes}");
            Console.WriteLine($"Imagen: {Imagen}");
            Console.WriteLine($"Wikidata: {LinkWikidata}");
            Console.WriteLine($"DBpedia: {LinkDbpedia}");
            Console.WriteLine($"Obras Relacionadas: {ObrasRelacionadas}");
            Console.WriteLine($"Obras Pinacoteca {ObrasPinacoteca}");
            Console.WriteLine($"Siglo de Popularidad: {SigloDePopularidad}");
        }

        public void AssingProperty(string property, string value)
        {
            switch (property.Trim().ToLower())
            {
                case "descripción":
                    this.Descripcion = check_descripcion(value);
                    break;
                case "imagen":
                    this.Imagen = check_imagen(value);
                    break;
                case "velocidad":
                    this.Velocidad = check_velocidad_fromString(value);
                    break;
                case "link wikidata":
                    this.LinkWikidata = check_linkWikidata(value);
                    break;
                case "nombre común":
                    this.NombreComun = check_nombreComun(value);
                    break;
                case "interés":
                    this.Interes = check_interes(value);
                    break;
                case "link dbpedia":
                    this.LinkDbpedia = check_linkDbpedia(value);
                    break;
                default:
                    // Acción para valor no válido
                    break;
            }
        }
        //Devolvemos el identificador de WIkidata (Parte del "Link Wikidata"
        public string GetWikidataId()
        {
            return this.LinkWikidata.Split('/').Last();
        }
        #region FuncionesDeVerificacion



       
        // Verifica la propiedad "Descripción"
        internal string check_descripcion(string descripcion)
        {
            //Verificamos que la descripccion este hablando del animal
            if (string.IsNullOrEmpty(descripcion) || string.IsNullOrEmpty(this.Name))
            {
                return "";
            }

            return descripcion.ToLower().Contains(this.Name.ToLower()) ? descripcion : "";
        }

        // Verifica la propiedad "Imagen"
        internal string check_imagen(string imagen)
        {
            if (String.IsNullOrEmpty(imagen)) {
                return "";
            } else {
                //Verificamos que el link sea https y si no lo corregimos
                string[] parts = imagen.Split(':');
                return (parts[0].Equals("https")) ? imagen : "https:" + parts[1];
            }

        }

        // Verifica la propiedad "Velocidad"
        internal double check_velocidad_fromString(string velocidad)
        {
            //Si la cadena esta vacia o nula devolvemos un valor predetermindado "0"
            if (String.IsNullOrEmpty(velocidad))
            {
                return 0.0;
            } else {
                //Parsear el numero y pasarlo al valor real si esta en km/h, si esta en unidades desconocidas devuelve 0
                double num = double.Parse(string.Concat(velocidad.TakeWhile(c => char.IsDigit(c) || c == '.')));
                return (velocidad.Contains("km/h")) ? num : velocidad.Contains("millas/h") ? num * 1.60934 : 0 ;
            }
        }

        // Verifica la propiedad "LinkWikidata"
        internal string check_linkWikidata(string linkWikidata)
        {
            // Comprobamos si el valor contiene un enlace completo a Wikidata, si no, lo añadimos al inicio
            return String.IsNullOrEmpty(linkWikidata) ? "Link no especificado" : linkWikidata.Contains("https://www.wikidata.org/wiki/") ? linkWikidata : "https://www.wikidata.org/wiki/" + linkWikidata.Split('/')[linkWikidata.Split('/').Length - 1];
        }

        // Verifica la propiedad "NombreComun"
        internal string check_nombreComun(string nombreComun)
        {
            // Vefiricamos si la cadena es vacia 
            return String.IsNullOrEmpty(nombreComun ) ? "" : nombreComun;
        }

        // Verifica la propiedad "Interes"
        internal string check_interes(string interes)
        {
            // Devolvemos el valor corespondiente si la cadena contine los niveles de interes especificados en la clase. Si la cadena es vacia o 
            //no contiene un valor de los especificados devolvemos "No especificado"
            return String.IsNullOrEmpty(interes) ? "No especificado" : nivelesInteres.FirstOrDefault(nivel => interes.IndexOf(nivel, StringComparison.OrdinalIgnoreCase) >= 0)?? "No especificado";
        }

        // Verifica la propiedad "LinkDbpedia"
        internal string check_linkDbpedia(string linkDbpedia)
        {
            // Comprobamos si el valor contiene un enlace completo a Wikidata, si no, lo añadimos lo que falta  al inicio
            return String.IsNullOrEmpty(linkDbpedia) ? "Link no especificado" : linkDbpedia.Contains("https://es.dbpedia.org/page/") ? linkDbpedia : "https://es.dbpedia.org/page/" + linkDbpedia.Split('/')[linkDbpedia.Split('/').Length - 1];
        }

        #endregion FuncionesDeVerificacion



        // Verifica la propiedad "ObrasRelacionadas"
        internal string add_obrasRelacionadas(string obrasRelacionadas)
        {
            // Implementación
            return obrasRelacionadas;
        }

        // Verifica la propiedad "ObrasPinacoteca"
        internal string add_obrasPinacoteca(string obrasPinacoteca)
        {
            // Implementación
            return obrasPinacoteca;
        }

        // Verifica la propiedad "SigloDePopularidad"
        internal string add_sigloDePopularidad(string sigloDePopularidad)
        {
            // Implementación
            return sigloDePopularidad;
        }








    }

}
