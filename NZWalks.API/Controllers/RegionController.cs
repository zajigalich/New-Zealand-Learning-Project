using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(IRegionRepository regionRepository, 
            IMapper mapper,
            ILogger<RegionsController> logger)
        {
            this.regionRepository=regionRepository;
            this.mapper=mapper;
            this.logger=logger;
        }


        // GET ALL REGIONS
        // GET: /api/regions
        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            /*logger.LogWarning("This is a warning log");
            logger.LogError("This is a error log");*/

            // Get Data from DB - Domain Models
            var regionsDomainModel = await regionRepository.GetAllAsync();

            logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regionsDomainModel)}");

            return Ok(mapper.Map<List<RegionDto>>(regionsDomainModel));
        }

        // GET SINGLE REGION (Get Region by id)
        // GET: /api/regions/{id}
        [HttpGet("{id:Guid}")]  //or [Route("{id:Guid}")] on the next line instead of ("{id:Guid}") here
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.GetByIdAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }


        // CREATE REGION
        // POST: /api/regions/
        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionDto)
        {
            var regionDomainModel = mapper.Map<Region>(addRegionDto);

            await regionRepository.CreateAsync(regionDomainModel);

            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById), new { regionDto.Id }, regionDto);
        }

        // UPDATE REGION
        // PUT: /api/regions/{id}
        [HttpPut("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionDto)
        {

            if (ModelState.IsValid)
            {
                var regionDomainModel = mapper.Map<Region>(updateRegionDto);

                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                return Ok(mapper.Map<RegionDto>(regionDomainModel));
            }

            return BadRequest(ModelState);
        }

        // DELETE REGION BY ID
        // DELETE: /api/regions/{id}
        [HttpDelete("{id:Guid}")]
        //[Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<RegionDto>(regionDomainModel)); // or return Ok();
        }
    }
}