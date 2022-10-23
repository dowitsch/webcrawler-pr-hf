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
        static void Main(string[] args)
        {
            var websites = new List<Website>();
            var writer = new StreamWriter("./output/huso.csv");
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            var Appartments = new List<Appartment>();

            HtmlWeb web = new HtmlWeb();
            websites.Add(new Website {url = "https://www.immoscout24.ch/de/immobilien/mieten/ort-bern?pt=2t&nrt=2.5&slf=100&r=5&map=1", name = "immoscout"});

            foreach(var item in websites) {
                Console.WriteLine("huso");
                HtmlDocument doc = web.Load(item.url);
                var possibleAppartments = doc.DocumentNode.SelectNodes("//article/a/div/div/div/div/h3");
                foreach(var possibleAppartment in possibleAppartments) {
                    var text = possibleAppartments.DocumentNode.InnerHtml;
                    Console.WriteLine(text);
                    var rent = int.Parse(Regex.Match(rentHeader.InnerHtml, @"\d+").Value);
                    var found = new Appartment {rent = rent, squaremeters = 120, location = "fickstrasse" };
                    Appartments.Add(found);
                    Console.WriteLine($"{found.rent}");
                    
                }
            }

            csv.WriteRecords(Appartments);
        }
    }



    public class Website
    {
        public string url {get; set;}
        public string name {get; set;}
    }


    public class Row
    {
        public string title {get; set;}
    }

    public class Appartment 
    {
        public int rent {get; set;}
        public int squaremeters {get; set;}
        public string location {get; set;}
    }


}