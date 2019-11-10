using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.BotBuilderSamples.Bots
{
    class CustomVisionService
    {
        public static List<string> probabilities = new List<string>();
        public static List<string> tags = new List<string>();
        
        
        public static List<string> MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid Prediction-Key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "70ffae35b98c4f1d83fb82c9cc36b306");
            // Prediction URL - replace this example URL with your valid Prediction URL.
            string url = @"https://northeurope.api.cognitive.microsoft.com/customvision/v3.0/Prediction/87225555-13ec-4d09-9ea5-cabf1ac80234/classify/iterations/Iteration1/image";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.

            
            var webClient = new WebClient();
            
            byte[] byteData = webClient.DownloadData(imageFilePath);

                using (var content = new ByteArrayContent(byteData))
            { 
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = client.PostAsync(url, content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                var resultRequest = result.ToString();

                dynamic parsed = JObject.Parse(resultRequest);
                var output = parsed.predictions;

                for (var i = 0; i < output.Count; i++)
                {
                    probabilities.Add(output[i].probability.ToString());
                    tags.Add(output[i].tagName.ToString());
                }

                // Method to send on VM to be analysed ... dictionary and userID
            }

            return probabilities;
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}
