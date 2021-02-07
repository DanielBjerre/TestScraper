using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using PuppeteerSharp;

namespace ScrapeTest {
    class Program {
        static async Task Main(string[] args) {
            string url1 = "https://www.hellofresh.dk/recipes/shakshuka-60016cce1aacb76865674154";
            string url = "https://www.hellofresh.dk/recipes/svinekod-i-tomatsauce-5ff6c4432ebdf4509d5211cf";
            //Selenium(url);
            //PuppeteerSharp(url);
            await CallUrl(url);
        }
        private static async Task CallUrl(string fullUrl) {
            HttpClient client = new();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            Task<string> response = client.GetStringAsync(fullUrl);
            ParseHtml(await response);
        }

        private static void ParseHtml(string html) {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);
            List<string> results = new();
            var nameh1 = htmlDoc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("data-test-id", "").Contains("recipeDetailFragment.recipe-name")).FirstOrDefault();
            string title = nameh1.InnerText;
            string subtitle = nameh1.NextSibling.InnerText;

            string description = htmlDoc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("data-translation-id", "").Contains("recipe-detail.read-more")).FirstOrDefault().ParentNode.NextSibling.FirstChild.FirstChild.InnerText;
            HtmlNode ingredientsDiv = htmlDoc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("data-test-id", "").Contains("recipeDetailFragment.ingredients")).FirstOrDefault().ChildNodes[3].FirstChild;
            
            List<string> ingredients = new();

            foreach (HtmlNode childNode in ingredientsDiv.ChildNodes) {
                ingredients.Add(childNode.ChildNodes[1].ChildNodes[1].InnerText);
            }
            
            Console.WriteLine($"Title: {title}");
            Console.WriteLine($"Subtitle: {subtitle}");
            Console.WriteLine("");
            Console.WriteLine("List of Ingredients:");
            foreach (string str in ingredients) {
                Console.WriteLine(str);
            }
        }
    private static async void PuppeteerSharp(string fullUrl) {
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
        Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions {
            Headless = true
        });

        Page page = await browser.NewPageAsync();
        await page.GoToAsync(fullUrl);


    }

    private static void Selenium(string fullUrl) {
        ChromeOptions options = new ChromeOptions();
        using IWebDriver driver = new ChromeDriver(options);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        driver.Navigate().GoToUrl(fullUrl);
        var stuff = driver.FindElements(By.TagName("div")).ToList();
    }

}
}
