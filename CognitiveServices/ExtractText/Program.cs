using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace ExtractText
{
    class Program
    {

        // Please Replace with valid Subscription Key
        private const string subscriptionKey = "cxxxxxxx824c2494f24a818xxxxx9";

    

        // For printed text, change to TextRecognitionMode.Printed
        private const TextRecognitionMode textRecognitionMode = TextRecognitionMode.Printed;               
        private const string remoteImageUrl1 = "https://cogtrainingdatastrg.blob.core.windows.net/computervisiondata/OCR/ocrsampleimage.png";
        private const int numberOfCharsInOperationId = 36;


        static void Main(string[] args)
        {

            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // Specify the Azure region
            computerVision.Endpoint = "https://southeastasia.api.cognitive.microsoft.com/";

            Console.WriteLine("Images being analyzed ...");
            var t1 = ExtractRemoteTextAsync(computerVision, remoteImageUrl1);
            //var t2 = ExtractLocalTextAsync(computerVision, localImagePath);

            Task.WhenAll(t1).Wait(5000);
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }


        // Recognize text from a remote image

        private static async Task ExtractRemoteTextAsync(
            ComputerVisionClient computerVision, string imageUrl)
        {

            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }



            // Start the async process to recognize the text
            RecognizeTextHeaders textHeaders =
                await computerVision.RecognizeTextAsync(
                imageUrl, textRecognitionMode);
                await GetTextAsync(computerVision, textHeaders.OperationLocation);

        }

        // Retrieve the recognized text
        private static async Task GetTextAsync(
            ComputerVisionClient computerVision, string operationLocation)
        {
            // Retrieve the URI where the recognized text will be
            // stored from the Operation-Location header
            string operationId = operationLocation.Substring(
                operationLocation.Length - numberOfCharsInOperationId);

            Console.WriteLine("\nCalling GetHandwritingRecognitionOperationResultAsync()");
            TextOperationResult result =
                await computerVision.GetTextOperationResultAsync(operationId);

            // Wait for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                Console.WriteLine(
                    "Server status: {0}, waiting {1} seconds...", result.Status, i);
                await Task.Delay(1000);

                result = await computerVision.GetTextOperationResultAsync(operationId);

            }
            

            // Display the results

            Console.WriteLine();
            var lines = result.RecognitionResult.Lines;
            foreach (Line line in lines)
            {
                Console.WriteLine(line.Text);
            }
            Console.WriteLine();

        }
    }
}
