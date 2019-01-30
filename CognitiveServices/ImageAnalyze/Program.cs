using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;


namespace ImageAnalyze
{
    class Program
    {

        // Please Replace with valid Subscription Key
        private const string subscriptionKey = "cb5dxxxx482xxxxxxxx18503axxxx";

        // Set remote Image URL
        private const string remoteImageUrl =
            "https://cogtrainingdatastrg.blob.core.windows.net/computervisiondata/ImageAnalysis/house.jpg";

        // Specify the features to return
        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
                    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                    VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                    VisualFeatureTypes.Tags
        };

        static void Main(string[] args)
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // Specify the Azure region
            computerVision.Endpoint = "https://southeastasia.api.cognitive.microsoft.com/";

            Console.WriteLine("Images being analyzed ...");
            AnalyzeRemoteAsync(computerVision, remoteImageUrl);

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        // Analyze a remote image
        private static async void AnalyzeRemoteAsync(
            ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            ImageAnalysis analysis =
                await computerVision.AnalyzeImageAsync(imageUrl, features);
            DisplayResults(analysis, imageUrl);
        }


        // Display the most relevant caption for the image
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
            Console.WriteLine(imageUri);
            Console.WriteLine(analysis.Description.Captions[0].Text + "\n");
            Console.WriteLine(analysis.Tags[1].Name);
        }
    }
}