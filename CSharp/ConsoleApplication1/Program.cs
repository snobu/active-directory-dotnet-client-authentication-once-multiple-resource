using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        const string AADTenant = "xxx.onmicrosoft.com";
        const string NativeClientId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
        const string subscriptionId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

        static void Main(string[] args)
        {
            Test();
            Console.ReadKey();
        }

        private static async void Test()
        {
            var resultForGraph = await GetGraphAccessResult();
            var resultForManagement = await GetManagementAccessResult(resultForGraph.RefreshToken);
            InvokeManagementApi(resultForManagement.AccessToken);
        }

        private static async Task<AuthenticationResult> GetGraphAccessResult()
        {
            var authenticationContext = new AuthenticationContext(string.Format("https://login.microsoftonline.com/{0}/", AADTenant));
            var result = authenticationContext.AcquireToken("https://graph.microsoft.com/", NativeClientId, new Uri("http://myapplication.com"));

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;
            Console.WriteLine("Token\n\n" + token + "\n\n=====================\n\n\n\n\n");
            return result;
        }

        private static async Task<AuthenticationResult> GetManagementAccessResult(string refreshToken)
        {
            var authenticationContext = new AuthenticationContext(string.Format("https://login.microsoftonline.com/{0}/", SubscriptionTenant));
            // Before going any further read this post -
            // http://www.cloudidentity.com/blog/2015/08/13/adal-3-didnt-return-refresh-tokens-for-5-months-and-nobody-noticed/
            var result = authenticationContext.AcquireTokenByRefreshToken(refreshToken, NativeClientId, "https://management.azure.com/");

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;
            Console.WriteLine("Token\n\n" + token + "\n\n=====================\n\n\n\n\n");
            return result;
        }

        private static void InvokeManagementApi(string accessToken)
        {
            // invoke RESTful API
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.Authorization, String.Format("Bearer {0}", accessToken));
                    wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    var json = wc.DownloadString(string.Format("https://management.azure.com/subscriptions/{0}/providers/microsoft.insights/eventtypes/management/values?api-version=2014-04-01&$filter=eventTimestamp ge '2017-01-15T00:00:00.00Z'", subscriptionId));
                    Console.WriteLine("Result JSON: \n\n" + json);
                }
            }
            catch (WebException ex)
            {
                using (var streamreader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    var result = streamreader.ReadToEnd();
                    Console.WriteLine(result);
                }
                Console.WriteLine(ex);
            }
        }
    }
}
