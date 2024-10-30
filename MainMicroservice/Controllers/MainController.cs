using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private static bool _connected = false;
        private readonly HttpClient _httpClient;
        private readonly string _getServiceUrl = "http://localhost:5001/api/get"; // URL for GET Microservice

        public MainController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("connect")]
        public IActionResult Connect()
        {
            _connected = true;
            return Ok(new { status = "connected" });
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            if (_connected)
            {
                return Ok(new { status = "Connected to GET Microservice" });
            }
            else
            {
                return Ok(new { status = "Not connected" });
            }
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            if (!_connected)
            {
                return BadRequest(new { error = "Not connected to GET Microservice" });
            }

            var content = new StringContent(JsonSerializer.Serialize(new { message }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_getServiceUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Message sent", response = await response.Content.ReadAsStringAsync() });
            }
            else
            {
                return StatusCode((int)response.StatusCode, new { error = "Failed to send message" });
            }
        }
    }
}
