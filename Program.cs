using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace console_dogs
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        private static List<dynamic> breeds = new List<dynamic>();

        private static string json = "";

        static async Task Main(string[] args)
        {
            bool run = true;

            while (run == true)
            {
                Console.Clear();
                Console.WriteLine("1 - List all dog breeds");
                Console.WriteLine("2 - Download random dog image");
                Console.WriteLine("3 - Search sub-breeds");
                //Console.WriteLine("4 - Select dog breed for image download");
                Console.WriteLine("Please enter a numerical selection:");


                string input = Console.ReadLine();

                Console.Clear();

                if (int.TryParse(input, out int inputNumber) == true)
                {
                    switch (inputNumber)
                    {
                        case 1:
                            // Print all dog breeds to console
                            await ListAllBreeds();
                            break;

                        case 2:
                            // Download and save a random dog image
                            await RandomDogImage();
                            break;

                        case 3:
                            // Prompt user to enter the name of a breed
                            Console.WriteLine("Please enter a valid dog breed");
                            string breed = Console.ReadLine().ToLower();
                            // Requests and prints a list of that breeds sub-breeds
                            await ListSubBreeds(breed);
                            break;

                        //case 4:
                        // Promt user to enter the name of a breed
                        // Downloads and saved an image of that breed
                        // Open the image in the users preferred image viewer
                        // break;

                        default:
                            Console.WriteLine("Please enter a valid selection.");
                            break;
                    }

                }
                else
                {
                    Console.WriteLine("Please enter a valid numerical selection.");
                }



                Console.Write("\n\nReturn to menu? (y/n):");
                run = Console.ReadLine().ToLower() == "y";

            }

        }

        /* 
         * Print list of all dog breeds in API
         */
        public static async Task ListAllBreeds()
        {
            await GetApiData("https://api.thedogapi.com/v1/breeds");

            dynamic breeds = JsonConvert.DeserializeObject(json);

            // Write breeds to console
            foreach (var dog in breeds)
            {
                Console.WriteLine(dog.name);
            }

        }

        /*
         * Save one random dog image from API and returns the filepath
         * Return the location where the image was saved to the console
         * Open the image in the users preferred image viewer
         */
        public static async Task RandomDogImage()
        {
            await GetApiData("https://dog.ceo/api/breeds/image/random");

            // If json.status == "success", json is valid, download image
            dynamic image = JsonConvert.DeserializeObject(json);

            if (image.status == "success")
            {
                using (WebClient wc = new WebClient())
                {
                    // Create new 'dog-image' folder
                    if (!Directory.Exists("C:\\dog-images"))
                    {
                        Directory.CreateDirectory("C:\\dog-images");
                    }

                    // Download image
                    wc.DownloadFile(new Uri(image.message.ToString()), @"C:\dog-images\image.jpg");
                    // Return download path to console
                    Console.WriteLine("Image downloaded to new folder: C:\\dog-images");
                }

                // Open image in default image viewer
                Process photoViewer = new Process();
                photoViewer.StartInfo.FileName = (@"C:\dog-images\image.jpg");
                photoViewer.Start();

            }
            else
            {
                Console.WriteLine("Image download failed.");
            }

        }

        /*
         * Prints list of all sub-breeds of given breed
         */
        public static async Task ListSubBreeds(string breed)
        {
            // Create new path based on user input
            string sub_breed_path = string.Format("https://dog.ceo/api/breed/{0}/list", breed);

            await GetApiData(sub_breed_path);

            dynamic subbreeds = JsonConvert.DeserializeObject(json);

            // Write sub-breeds to console
            if (subbreeds.status == "success")
            {
                Console.WriteLine();
                foreach (var sub in subbreeds.message)
                {
                    Console.WriteLine(sub);
                }
            }

        }

        // GET API data at given path
        public static async Task GetApiData(string path)
        {
            // Create HTTP CLient request
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")); // Type of request
            client.DefaultRequestHeaders.Add("x-api-key", "1ad94d10-a62c-418b-b020-01b50b38ed8c"); // Add auth to header

            var stringTask = client.GetStringAsync(path);

            var message = await stringTask;
            if (stringTask.IsCompleted)
            {
                json = message;
            }

        }
    }
}