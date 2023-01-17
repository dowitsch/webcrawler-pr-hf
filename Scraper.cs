using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

class Scraper
{

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a website to download.");
            return;
        }

        foreach (string website in args)
        {
            WebCrawler crawler = new WebCrawler(website);
            crawler.work();
        }
    }
}

public class WebCrawler
{
    public string Website { get; private set; }

    public string? Folder { get; private set; }

    public WebCrawler(string website)
    {
        this.Website = website;
        this.createFolder();
    } 

    public void work() {
        using (HttpClient client = new HttpClient())
            {

                string htmlContent = fetchUrlContent(client, this.Website);

                MatchCollection imgLinks = Regex.Matches(htmlContent, @"<img[^>]+src=""([^"">]+)"">");
                foreach (Match match in imgLinks)
                {
                    string filePath = match.Groups[1].Value;
                    writeContentToFile(client, filePath);
                }

                MatchCollection cssLinks = Regex.Matches(htmlContent, @"<link.*?rel=['\""]stylesheet['\""].*?href=['\""](?<link>.*?)['\""].*?>", RegexOptions.Singleline);
                foreach (Match match in cssLinks)
                {
                    string filePath = match.Groups["link"].Value;
                    writeContentToFile(client, filePath);
                }

                File.WriteAllText($"{this.Folder}/website.html", htmlContent);
                Console.WriteLine($"Website {this.Website} successfully saved to multiple files.");
            }
    }

    private void writeContentToFile(HttpClient client, string filePath)
    {
        string fileName = Regex.Match(filePath, @"[^/\\&\?]+\.\w{3,4}(?=([\?&].*$|$))").Groups[0].Value;
        string content = fetchUrlContent(client, this.Website + filePath);
        File.WriteAllText($"{this.Folder}/{fileName}", content);
    }
    private string fetchUrlContent(HttpClient client, string url)
    {
        HttpResponseMessage response = client.GetAsync(url).Result;
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Successfull downloaded source: {url}");
            return System.Text.Encoding.Default.GetString(response.Content.ReadAsByteArrayAsync().Result);
        }
        else
        {
            Console.WriteLine($"Failed to download source: {response.StatusCode} - {url}");
            return "";
        }


    }

    private void createFolder() {
        Uri uri = new Uri(this.Website);
        string name = $"output/{uri.Host.ToString()}";
        System.IO.Directory.CreateDirectory(name);
        this.Folder = name;
    }
}