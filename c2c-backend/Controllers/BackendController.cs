using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Sockets;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.Options;

namespace c2c_backend.Controllers
{
    [Route("")]
    public class BackendController : Controller
    {
        private readonly IHostingEnvironment _env;
        private string _address;
        private int _port;

        public BackendController(IHostingEnvironment env, IOptions<CloudFoundryApplicationOptions> appOptions)
        {
            _env = env;
            _address = getIpAddress();
            _port = appOptions.Value.Port;
        }

        // GET api/values
        [Route("")]
        [Route("/ping")]
        [HttpGet]
        public string Get()
        {
            return "Remote hello from " + _env.ApplicationName + " (IP: " + _address + ":" + _port + ")";
        }

        private string getIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    // Just nab the first IP address for demo purposes
                    return ip.ToString();
                }
            }
            return _address = "Unknown";
        }
        /*
         * Connect to a service directly (external to foundation)
         */
        [HttpGet]
        [Route("/remote")]
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
