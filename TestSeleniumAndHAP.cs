using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;


namespace AutomateStuffWithCSharp
{
    /// <summary>
    /// This simple script navigates to a url (using selenium), and gets links from the web page.
    /// </summary>
    class TestSeleniumAndHAP
    {
        // current directory of the application
        public static string driverPath = Directory.GetCurrentDirectory();
        
        /// <summary>
        /// Initializes a new instance of the ChromeDriver with required options for the browser
        /// </summary>
        /// <returns>a new instance of the ChromeDriver class</returns>
        public static IWebDriver InitWebDriver()
        {
            ChromeOptions options = new ChromeOptions();  // Chrome (browser) options
            options.AddArgument("--start-maximized");  // start browser with maximized window
            options.AddArgument("--incognito");  // browser on incognito option
            options.AddArgument("--headless"); // headless browser option

            // Initializes a new instance of the ChromeDriver class using the specified 
            // path to the directory containing ChromDriver.exe and options
            ChromeDriver cDriver = new ChromeDriver(driverPath, options);
            return cDriver;
        }

        /// <summary>
        /// Checks whether the given node is named "a" (a tag), has attribute that named 'href',
        /// and it's inner text is not null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="link">Represent an HTML node of "a" tag</param>
        /// <returns>true if a tag has href&text, or false</returns>
        public static bool HasHrefAndText(HtmlNode link)
        {
            // checks if node is named 'a', has attribute that named 'href', 
            // and it's inner text is not null or whitespace.
            bool aHrefAndText = ((link.Name == "a") && (link.Attributes.Contains(@"href")) &&
                (!string.IsNullOrWhiteSpace(link.InnerText)));
            return aHrefAndText;
        }

        /// <summary>
        /// check if the given url is valid (check if the new URI is accessed through HTTP/HTTPS
        /// </summary>
        /// <param name="givenUrl">string that should represent a URL</param>
        /// <returns>true if valid URL, or false</returns>
        public static bool IsValidUrl(string givenUrl)
        {
            bool validUrl = Uri.TryCreate(givenUrl, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp
                || uriResult.Scheme == Uri.UriSchemeHttps);
            return validUrl;
        }

        /// <summary>
        /// Parses and prints the required a tags (links - href&text) from the give HtmlDocument
        /// </summary>
        /// <param name="htmlDoc">Represent a complete HTML document</param>
        public static void PrintATags(HtmlDocument htmlDoc)
        {
            if (htmlDoc != null)
            {
                // Selects the first XmlNode that matches the XPath expression
                var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");

                foreach (HtmlNode link in htmlBody.SelectNodes("//a"))  // findall a tags in body tag
                {
                    if (HasHrefAndText(link))  // true if a tag has href&text, or false
                    {
                        HtmlAttribute hrefAttr = link.Attributes["href"];
                        Console.WriteLine(hrefAttr.Value);  // prints href of a tag
                        Console.WriteLine(link.InnerText + "\n");  // prints inner text of a tag
                    }
                }
            }
        }

        /// The main function. navigates to given url and gets content of the body tag 
        static void Main(string[] args)
        {
            string givenUrl = "";
            bool validUrl = false;
            int attempts = 0;
            do
            {
                Console.WriteLine("Please Enter a URL: ");
                givenUrl = Console.ReadLine();  // input from user
                validUrl = IsValidUrl(givenUrl);
                attempts++;
            } while ((givenUrl == "") || !(validUrl) && (attempts < 5));

            if (validUrl)
            {
                // Initializes a new instance of the ChromeDriver (Chrome browser)
                IWebDriver chromeDriver = InitWebDriver();
                // Load a new page in the current browser window
                chromeDriver.Navigate().GoToUrl(givenUrl);
                // get the source of the page last loaded by the browser
                string pageSource = chromeDriver.PageSource;
                // Quits the driver, closing every associated window
                chromeDriver.Quit();

                var htmlDoc = new HtmlDocument();  // creates an instance of an HTML document
                htmlDoc.LoadHtml(pageSource);  // Loads the HTML document from the specified string
                PrintATags(htmlDoc);  // prints required a tags from the HTML document

                // Keep the console window open in debug mode.
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();

            }

        }
    }
}
