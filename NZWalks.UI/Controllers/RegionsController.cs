using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Text;
using System.Text.Json;

namespace NZWalks.UI.Controllers
{
	public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public RegionsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory=httpClientFactory;
            this.configuration=configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
			List<RegionDto> responce = new List<RegionDto>();

            try
            {
                // Get all regions from web api
                var client = httpClientFactory.CreateClient();

                var httpResponceMessage = await client.GetAsync($"{configuration["ApiHttpString"]}regions");

                httpResponceMessage.EnsureSuccessStatusCode();

                responce.AddRange(await httpResponceMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
            }
            catch (Exception ex)
            {
                // log exception
               
            }

            return View(responce);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel model)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{configuration["ApiHttpString"]}regions"),
				Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"),
            };

            var httpResponceMessage = await client.SendAsync(httpRequestMessage);
            httpResponceMessage.EnsureSuccessStatusCode();

            var responce = await httpResponceMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (responce is not null) 
            {
                return RedirectToAction("Index", "Regions");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = httpClientFactory.CreateClient();

            var responce = await client.GetFromJsonAsync<RegionDto>($"{configuration["ApiHttpString"]}regions/{id}");

            if (responce is not null)
            {
                return View(responce);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto request)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{configuration["ApiHttpString"]}regions/{request.Id}"),
                Method= HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            var httpResponceMessage = await client.SendAsync(httpRequestMessage);

            httpResponceMessage.EnsureSuccessStatusCode();

            var responce = await httpResponceMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (responce is not null)
            {
                return RedirectToAction("Edit", "Regions");
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto request)
        {
            var client = httpClientFactory.CreateClient();

            var httpResponceMessage = await client.DeleteAsync($"{configuration["ApiHttpString"]}regions/{request.Id}");

            httpResponceMessage.EnsureSuccessStatusCode();

            var responce = await httpResponceMessage.Content.ReadFromJsonAsync<RegionDto>();

            if(responce is not null)
            {
				return RedirectToAction("Index", "Regions");
			}

            return View();  

		}
    }
} 
