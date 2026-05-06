using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Category.API.Services;
using System.Security.Claims;

namespace SpendSmart.Category.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public record CreateCategoryRequest(string Name, string Icon, string Color, string Type);
        public record UpdateCategoryRequest(string Name, string Icon, string Color);

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            int? userId = null;
            try { userId = GetUserId(); } catch { }

            var category = new Entities.Category
            {
                UserId = userId,
                Name = request.Name,
                Icon = request.Icon,
                Color = request.Color,
                Type = request.Type.ToUpper(),
                IsDefault = userId == null // If no user, it's a system default
            };

            await _categoryService.CreateCategoryAsync(category);
            return Ok(category);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetByUser()
        {
            return Ok(await _categoryService.GetCategoriesByUserAsync(GetUserId()));
        }

        [AllowAnonymous]
        [HttpGet("defaults")]
        public async Task<IActionResult> GetDefaults()
        {
            return Ok(await _categoryService.GetDefaultCategoriesAsync());
        }

        [HttpGet("allForUser")]
        public async Task<IActionResult> GetAllForUser()
        {
            return Ok(await _categoryService.GetAllForUserAsync(GetUserId()));
        }

        [HttpGet("byType")]
        public async Task<IActionResult> GetByType([FromQuery] string type)
        {
            return Ok(await _categoryService.GetByTypeAsync(GetUserId(), type));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            var success = await _categoryService.UpdateCategoryAsync(id, GetUserId(), request.Name, request.Icon, request.Color);
            return success ? Ok() : BadRequest();
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var success = await _categoryService.DeactivateCategoryAsync(id, GetUserId());
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id, GetUserId());
            return success ? Ok() : BadRequest();
        }
    }
}
