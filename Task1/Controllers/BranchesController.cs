using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task1.Contracts;
using Task1.Data;
using Task1.Models.Branch;

namespace Task1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public BranchesController(IMapper mapper, IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://api.weatherapi.com/v1/current.json");
        }

        // GET: api/Branches
        [HttpGet]
        //[Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<GetBranchDto>>> GetBranches()
        {
            var branches = await _branchRepository.GetAllAsync();
            var records = _mapper.Map<List<GetBranchDto>>(branches);
            return Ok(records);
        }

        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(int id)
        {
            var branch = await _branchRepository.GetDetails(id);

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }
        [HttpGet("{id}/weather")]
        public async Task<IActionResult> GetForecast(int id)
        {
            try
            {
                var branch = await _branchRepository.GetDetails(id);
                var city = branch.City;
                HttpResponseMessage response = await _client.GetAsync($"?key=e08165481634422b8df72421242702&q={city.CityName}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Failed to get data");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        // PUT: api/Branches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBranch(int id, UpdateBranchDto updateBranch)
        {
            if (id != updateBranch.Id)
            {
                return BadRequest();
            }

            var branch = await _branchRepository.GetAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            _mapper.Map(updateBranch, branch);

            try
            {
                await _branchRepository.UpdateAsync(branch);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BranchExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Branches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Branch>> PostBranch(CreateBranchDto branchdto)
        {
            var branch = _mapper.Map<Branch>(branchdto);
            await _branchRepository.AddAsync(branch);

            return CreatedAtAction("GetBranch", new { id = branch.Id }, branch);
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _branchRepository.GetAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            await _branchRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _branchRepository.Exists(id);
        }
    }
}
