namespace HelloWorld
{

    using CsvHelper;
    using HtmlAgilityPack;
    using System.IO;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    class Program
    {

        /// <summary>
        /// Hauptfunktion des Programms, das die Verarbeitung der Website durchführt und die gefundenen Appartements in eine CSV-Datei schreibt.
        /// </summary>
        /// <param name="args">Argumente, die beim Aufruf des Programms übergeben werden</param>
        static void Main(string[] args)
        {
            // Initialisierung von Variablen
            var websites = new List<Website>();
            var writer = new StreamWriter("./output/apartments.csv");
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            var appartments = new List<Appartment>();

            VerarbeiteWebsites(websites, appartments);
            SchreibeAppartementsInCsv(appartments, csv);
        }

        /// <summary>
        /// Verarbeitet die übergebenen Websites und fügt die gefundenen Appartements zur Liste hinzu.
        /// </summary>
        /// <param name="websites">Liste von Websites, die verarbeitet werden sollen</param>
        /// <param name="appartments">Liste von Appartements, zu der die gefundenen Appartements hinzugefügt werden sollen</param>
        private static void VerarbeiteWebsites(List<Website> websites, List<Appartment> appartments)
        {
            HtmlWeb web = new HtmlWeb();
            websites.Add(new Website { url = "https://www.immoscout24.ch/de/immobilien/mieten/ort-bern?pt=2t&nrt=2.5&slf=100&r=5&map=1", name = "immoscout" });

            // Iteration über alle Websites in der Liste
            foreach (var website in websites)
            {
                VerarbeiteWebsite(website, appartments, web);
            }
        }

        /// <summary>
        /// Verarbeitet eine einzelne Website und fügt die gefundenen Appartements zur Liste hinzu.
        /// </summary>
        /// <param name="website">Website, die verarbeitet werden soll</param>
        /// <param name="appartments">Liste von Appartements, zu der die gefundenen Appartements hinzugefügt werden sollen</param>
        /// <param name="web">HTMLWeb-Objekt, das verwendet wird, um die Website zu laden</param>
        private static void VerarbeiteWebsite(Website website, List<Appartment> appartments, HtmlWeb web)
        {
            HtmlDocument doc = LadeWebsite(website, web);

            // Suche nach möglichen Appartements auf der aktuellen Website
            var possibleAppartments = doc.DocumentNode.SelectNodes("//article/a/div/div/div/div/h3");

            // Iteration über alle gefundenen möglichen Appartements
            foreach (var possibleAppartment in possibleAppartments)
            {
                VerarbeiteMöglichesAppartment(possibleAppartment, appartments);
            }
        }

        /// <summary>
        /// Verarbeitet ein mögliches Appartment und fügt es der Liste hinzu, falls es tatsächlich ein Appartment ist.
        /// </summary>
        /// <param name="possibleAppartment">Mögliches Appartment, das verarbeitet werden soll</param>
        /// <param name="appartments">Liste von Appartements, zu der das gefundene Appartment hinzugefügt werden soll</param>
        private static void VerarbeiteMöglichesAppartment(HtmlNode possibleAppartment, List<Appartment> appartments)
        {
            // Extrahierung von Informationen aus dem HTML-Code
            var appartmentInformationenExtraktor = new AppartmentInformationenExtraktor();
            var appartmentInformationen = appartmentInformationenExtraktor.ExtrahiertInformationenAusHtml(possibleAppartment);

            // Überprüfung, ob es sich tatsächlich um ein Appartment handelt
            var appartmentValidator = new AppartmentValidator();
            if (appartmentValidator.IstGültigesAppartment(appartmentInformationen))
            {
                // Füge das Appartment der Liste hinzu
                appartments.Add(appartmentInformationen);
                Console.WriteLine($"{appartmentInformationen.Miete}");
            }
        }



        /// <summary>
        /// Lädt die übergebene Website und gibt das resultierende HTMLDocument-Objekt zurück.
        /// </summary>
        /// <param name="website">Website, die geladen werden soll</param>
        /// <param name="web">HTMLWeb-Objekt, das verwendet wird, um die Website zu laden</param>
        /// <returns>HTMLDocument-Objekt, das die Website repräsentiert</returns>
        private static HtmlDocument LadeWebsite(Website website, HtmlWeb web)
        {
            return web.Load(website.url);
        }

        /// <summary>
        /// Schreibt die übergebenen Appartements in eine CSV-Datei.
        /// </summary>
        /// <param name="appartments">Liste von Appartements, die in die CSV-Datei geschrieben werden sollen</param>
        /// <param name="csv">CSV-Schreiber, der verwendet wird, um die Appartements in die Datei zu schreiben</param>
        private static void SchreibeAppartementsInCsv(List<Appartment> appartments, CsvWriter csv)
        {
            csv.WriteRecords(appartments);
        }
    }



    public class Website
    {
        public string url { get; set; }
        public string name { get; set; }
    }


    public class Row
    {
        public string title { get; set; }
    }

    public class Appartment
    {
        public int rent { get; set; }
        public int squaremeters { get; set; }
        public string location { get; set; }
    }
    public class AppartmentInformationenExtraktor
    {
        /// <summary>
        /// Extrahiert Informationen über ein Appartment aus dem übergebenen HTML-Code.
        /// </summary>
        /// <param name="html">HTML-Code, aus dem die Informationen extrahiert werden sollen</param>
        /// <returns>Informationen über das Appartment</returns>
        public AppartmentInformationen ExtrahiertInformationenAusHtml(HtmlNode html)
        {
            // Extrahierung von Informationen aus dem HTML-Code
            var text = html.DocumentNode.InnerHtml;
            Console.WriteLine(text);
            var miete = int.Parse(Regex.Match(html.InnerHtml, @"\d+").Value);

            return new AppartmentInformationen
            {
                Miete = miete,
                Quadratmeter = 120,
                Lage = "fickstrasse"
            };
        }
    }

    public class AppartmentValidator
    {
        /// <summary>
        /// Überprüft, ob es sich bei den übergebenen Informationen um ein gültiges Appartment handelt.
        /// </summary>
        /// <param name="appartmentInformationen">Informationen über das zu überprüfende Appartment</param>
        /// <returns>True, falls es sich um ein gültiges Appartment handelt, andernfalls false</returns>
        public bool IstGültigesAppartment(AppartmentInformationen appartmentInformationen)
        {
            // Hier könnten z.B. bestimmte Bedingungen für die Gültigkeit eines Appartments definiert werden,
            // z.B. dass die Miete nicht zu hoch oder zu niedrig ist, oder dass eine bestimmte Anzahl an Quadratmetern vorliegt.
            // In diesem Beispiel wird einfach immer true zurückgegeben, um die Funktionsweise zu verdeutlichen.
            return true;
        }
    }


}