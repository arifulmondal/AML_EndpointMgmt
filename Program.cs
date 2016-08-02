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
            const string endpointName = "newendpoint1";
            const string workspaceId = "yourWorkspaceId";//From workspace settings
            const string workspaceToken = "yourWorkspaceAuthToken";//From Settings, Auth Token
            const string webserviceId = "yourWebServiceId";//From Azure portal or help page url of the service after /webservices/

            string endpointUrl = getEndpointUrl(workspaceId, webserviceId, endpointName);
            string endpointsUrl = getEndpointsUrl(workspaceId, webserviceId);//to get endpoints list

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

            //Get endpoints
            //Get list of endpoint
            Console.WriteLine("Get endpoints? y/n");
            string responseGet = Console.ReadLine();
            if (responseGet == "y")
            {
                ListEndpoints(endpointsUrl, workspaceToken).Wait();
            }

            //Get endpoint
            Console.WriteLine("Get endpoint? y/n");
            string responseGetEndpoint = Console.ReadLine();
            if (responseGet == "y")
            {
                GetEndpoint(endpointUrl, workspaceToken).Wait();
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

                Console.WriteLine("Starting create endpoint");

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
            //For different Azure regions, you need to append the management.azureml.net with the Azure region. 
            //For South East Asia append asiasoutheast. That makes the url https://asiasoutheast.management.azureml.net. 
            //For Western Europe, https://europewest.management.azureml.net. 
            //For US, leave as is.
            string endpointUrl = "https://management.azureml.net/workspaces/" + workspaceId + "/webservices/" + webserviceId + "/endpoints/" + endpointName;
            return endpointUrl;
        }
        static string getEndpointsUrl(string workspaceId, string webserviceId)
        {
            //Format is: https://management.azureml.net/workspaces/<workspaceId>/webservices/<webserviceid>/endpoints
            //You can get the workspaceId and webesrviceId from Scoring service's RRS help page url (remove apihelp and id after endpoints)
            string endpointsUrl = "https://management.azureml.net/workspaces/" + workspaceId + "/webservices/" + webserviceId + "/endpoints";
            return endpointsUrl;
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
        /**********************List endpoints*************************************************/
        static async Task ListEndpoints(string requestUri, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), requestUri) { };

                Console.WriteLine("Starting getting list of endpoints.");

                var response = await client.SendAsync(requestMessage);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {

                    Console.WriteLine(responseContent.ToString());
                }
                else
                {
                    Console.WriteLine("Response status code {0}", response.StatusCode);
                    Console.WriteLine("ListEndpoints failed: {0}", response.ToString());
                }
            }
        }
        /**********************Get endpoint*************************************************/
        static async Task GetEndpoint(string requestUri, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), requestUri) { };

                Console.WriteLine("Starting get endpoint.");

                var response = await client.SendAsync(requestMessage);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {

                    Console.WriteLine(responseContent.ToString());
                }
                else
                {
                    Console.WriteLine("Response status code {0}", response.StatusCode);
                    Console.WriteLine("Get endpoint failed: {0}", response.ToString());
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
