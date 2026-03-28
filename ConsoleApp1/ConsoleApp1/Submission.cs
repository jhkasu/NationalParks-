using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class Submission
    {
        // GitHub Pages URLs
        public static string xmlURL = "https://raw.githubusercontent.com/jhkasu/NationalParks-/main/NationalParks.xml";
        public static string xmlErrorURL = "https://raw.githubusercontent.com/jhkasu/NationalParks-/main/NationalParksErrors.xml";
        public static string xsdURL = "https://raw.githubusercontent.com/jhkasu/NationalParks-/main/NationalParks.xsd";

        public static void Main(string[] args)
        {
            string result;

            // Q3-1: valid XML should pass with no errors
            result = Verification(xmlURL, xsdURL);
            Console.WriteLine("=== Valid XML ===");
            Console.WriteLine(result);
            Console.WriteLine();

            // Q3-2: error XML should fail and print the errors
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine("=== Error XML ===");
            Console.WriteLine(result);
            Console.WriteLine();

            // Q3-3: convert valid XML to JSON
            result = Xml2Json(xmlURL);
            Console.WriteLine("=== XML to JSON ===");
            Console.WriteLine(result);
        }

        // Q2.1
        // Validates xmlUrl against xsdUrl.
        // Returns "No errors are found" on success, or the error messages otherwise.
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            StringBuilder errors = new StringBuilder();

            try
            {
                // fetch both files remotely
                string xsdContent = Fetch(xsdUrl);
                string xmlContent = Fetch(xmlUrl);

                // load the schema
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add("", XmlReader.Create(new StringReader(xsdContent)));

                // parse the XML document (throws XmlException if malformed)
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                // attach schema and validate
                doc.Schemas = schemas;
                doc.Validate((sender, e) =>
                {
                    errors.AppendLine(e.Message);
                });
            }
            catch (XmlSchemaException ex)
            {
                return string.Format("Schema error at line {0}, position {1}: {2}",
                    ex.LineNumber, ex.LinePosition, ex.Message);
            }
            catch (XmlException ex)
            {
                return string.Format("XML error at line {0}, position {1}: {2}",
                    ex.LineNumber, ex.LinePosition, ex.Message);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (errors.Length == 0)
                return "No errors are found";

            return errors.ToString().Trim();
        }

        // Q2.2
        // Fetches the XML at xmlUrl and converts it to a JSON string.
        // The result is compatible with JsonConvert.DeserializeXmlNode().
        public static string Xml2Json(string xmlUrl)
        {
            string xmlContent = Fetch(xmlUrl);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            return JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
        }

        // helper — downloads a URL and returns the response as a string
        private static string Fetch(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}
