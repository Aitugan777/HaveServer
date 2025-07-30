using AitukCore.Contracts;
using AitukServer.Data;
using AitukServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Web.Http.Cors;

namespace AitukServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClothPropertiesController : ControllerBase
    {
        private readonly ClothPropertiesRepository _repository;

        public ClothPropertiesController(ClothPropertiesRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("sizes")]
        public async Task<ActionResult<List<SizeContract>>> GetSizes()
        {
            var sizes = await _repository.GetSizesAsync();
            if (sizes == null || sizes.Count == 0)
                return NotFound("Sizes not found.");

            return Ok(sizes);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryContract>>> GetCategories()
        {
            var categories = await _repository.GetCategoriesAsync();
            if (categories == null || categories.Count == 0)
                return NotFound("Categories not found.");

            return Ok(categories);
        }

        [HttpGet("genders")]
        public async Task<ActionResult<List<GenderContract>>> GetGenders()
        {
            var genders = await _repository.GetGendersAsync();
            if (genders == null || genders.Count == 0)
                return NotFound("Genders not found.");

            return Ok(genders);
        }

        [HttpGet("colors")]
        public async Task<ActionResult<List<ColorContract>>> GetColors()
        {
            var colors = await _repository.GetColorsAsync();
            if (colors == null || colors.Count == 0)
                return NotFound("Colors not found.");

            return Ok(colors);
        }
    }

}
