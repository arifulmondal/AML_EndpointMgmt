using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AddEndpoint
{
    class Program
    {
        static void Main(string[] args)
        {
            const string endpointName = "newendpoint1";//Set name
            const string workspaceId = "c79283d685684d8488fc...";//Get from workspace settings
            const string workspaceToken = "7df5facee554413399...";//Get from Settings, Auth Token
            const string webserviceId = "123e37e77a534a038e...";//Get from Azure portal or help page url of the service after /webservices/

            string endpointUrl = getEndpointUrl(workspaceId, webserviceId, endpointName);

            //Delete endpoint
            Console.WriteLine("Delete endpoint? y/n");
            string responseDelete = Console.ReadLine();
            if (responseDelete == "y")
            {
                DeleteEndpoint(endpointUrl, workspaceToken).Wait();
            }

            //Add new endpoint
            Console.WriteLine("Create endpoint? y/n");
            string responseCreate = Console.ReadLine();
            if (responseCreate == "y")
            {
                AddNewEndpoint(endpointUrl, workspaceToken).Wait();
            }
            Console.WriteLine("Press enter to close.");
            Console.ReadLine();
        }

        //**********************Add new end point**********************
        static async Task AddNewEndpoint(string requestUri, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                WebServiceEndpoint request = new WebServiceEndpoint
                {
                    Description = "New end point",
                    ThrottleLevel = "Low"//, High is available only for paid tier
                    //MaxConcurrentCalls = "500" //not available for free trial
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestMessage = new HttpRequestMessage(new HttpMethod("PUT"), requestUri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.Default, "application/json")
                };

                Console.WriteLine("Starting create endpoint.");

                var response = await client.SendAsync(requestMessage);
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseContent);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Endpoint created.");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Response status code {0}", response.StatusCode);
                    Console.WriteLine("Endpoint creation failed: {0}", response.ToString());
                    Console.ReadLine();
                }
            }
        }
        
        static string getEndpointUrl(string workspaceId, string webserviceId, string endpointName)
        {
            //Format is: https://management.azureml.net/workspaces/<workspaceId>/webservices/<webserviceid>/endpoints/<name> 
            //You can get the workspaceId and webesrviceId from Scoring service's RRS help page url (remove apihelp and id after endpoints)
            string endpointUrl = "https://management.azureml.net/workspaces/" + workspaceId + "/webservices/" + webserviceId + "/endpoints/" + endpointName;
            return endpointUrl;
        }
        /********************************Delete endpoint*****************************/
        static async Task DeleteEndpoint(string requestUri, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                var requestMessage = new HttpRequestMessage(new HttpMethod("DELETE"), requestUri) { };

                Console.WriteLine("Starting delete endpoint.");

                var response = await client.SendAsync(requestMessage);
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseContent);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Endpoint deleted.");
                }
                else
                {
                    Console.WriteLine("Response status code {0}", response.StatusCode);
                    Console.WriteLine("Endpoint deletion failed: {0}", response.ToString());
                }
            }
        }
    }
    public class WebServiceEndpoint
    {
        public string Description { get; set; }
        public string ThrottleLevel { get; set; }
        public string MaxConcurrentCalls { get; set; }
    }
}