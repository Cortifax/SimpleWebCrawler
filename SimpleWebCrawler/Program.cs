using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace SimpleWebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            startCrawler();
            Console.ReadLine();
        }


        public static async Task startCrawler()
        {
            //Load html site
            var url = "https://www.sahibinden.com/";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            //Get all the href of the list elements
            var nodes = htmlDocument.DocumentNode.SelectNodes("//ul[@class='vitrin-list clearfix']/li/a");
           
            var adverts = new List<Advert>();

            foreach(var node in nodes)
            {
               var advert = await getInfo(node.Attributes["href"].Value);
               adverts.Add(advert);
            }

            foreach(var advert in adverts)
            {
                Console.WriteLine($"İlan Adı={ advert.Name}");
                Console.WriteLine($"İlan Fiyatı={ advert.Price}");
                Console.WriteLine($"İlan Adresi={ advert.Adress}");
                Console.WriteLine("\n ");
            }
        }

        public static async Task<Advert> getInfo(string href)
        {
            var url = "https://www.sahibinden.com" + href;
      
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            
            //Get all name,price and adress from detail html page
            var name = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='classifiedDetailTitle']/h1").InnerText.Trim();
            var price = htmlDocument.DocumentNode.SelectSingleNode("/html/body/div[4]/div[4]/div/div[2]/div[2]/h3").InnerText;
            var adress = htmlDocument.DocumentNode.SelectSingleNode("/html/body/div[4]/div[4]/div/div[2]/div[2]/h2").InnerText.Trim();
            
            //Modify Price and Adress string
            string trimmedPrice = Regex.Replace(price, @"[^\d]", "");
            string trimmedAdress = adress.Replace("\n", "").Replace("\r", "").Replace(" ", "");

            var advert = new Advert
            {
                Name =name,
                Price = trimmedPrice,
                Adress = trimmedAdress
            };

            return advert;
        }

        public class Advert
        {
           public string Name { get; set; }

           public string Price { get; set; }

           public string Adress { get; set; }
        }


    }
}
