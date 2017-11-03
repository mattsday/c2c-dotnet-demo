using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pivotal.Discovery.Client;

namespace c2c_frontend.Controllers
{
    [Route("")]
    public class FrontendController : Controller
    {
        private DiscoveryHttpClientHandler _handler;
        private string _backendBaseUrl;

        public FrontendController(IDiscoveryClient client, IConfiguration configuration)
        {
            string backendName = configuration.GetSection("c2c:backend").Get<string>();

            // The .NET Eureka handler automatically replaces the backend app name, so just provide it here:
            _backendBaseUrl = "http://" + backendName;
            _handler = new DiscoveryHttpClientHandler(client);
        }

        /*
         * Attempt to connect to backend
         */
        [HttpGet]
        [Route("/ping")]
        public async Task<IActionResult> ping()
        {
            Console.WriteLine("Trying to get " + _backendBaseUrl + "/");
            using (var client = new HttpClient(_handler, false))
            {
                try
                {
                    Console.WriteLine(client.BaseAddress);
                    var response = await client.GetAsync(_backendBaseUrl + "/");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Got! " + _backendBaseUrl + "/");
                    return Ok(stringResult);
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("Got! " + _backendBaseUrl + "/");

                    return Ok("Could not reach url " + _backendBaseUrl + "/ (networking problem ?)");
                }
            }
        }

        /*
         * Connect to a service indirectly via backend (external to foundation)
         */
        [HttpGet]
        [Route("/remote/backend")]
        public async Task<IActionResult> connectRemoteBackend(string url)
        {
            using (var client = new HttpClient(_handler, false))
            {
                try
                {
                    var response = await client.GetAsync(_backendBaseUrl + "/remote?url=" + url);
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    return Ok(stringResult);
                }
                catch (HttpRequestException)
                {
                    return Ok("Could not reach url " + url + "(networking problem ?)");
                }
            }
        }

        /*
         * Connect to a service directly (external to foundation)
         */
        [HttpGet]
        [Route("/remote/frontend")]
        public async Task<IActionResult> connectRemoteFrontend(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    return Ok(stringResult);
                }
                catch (HttpRequestException)
                {
                    return Ok("Could not reach url " + url + "(networking problem ?)");
                }
            }
        }

    }
}
