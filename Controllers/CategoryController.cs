using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using piggyzen.api.Data;
using piggyzen.api.Models;
using piggyzen.api.Dtos.Category;

namespace piggyzen.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly PiggyzenContext _context;

        public CategoryController(PiggyzenContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllCategoriesDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new GetAllCategoriesDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCategoryByIdDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Subcategories)
                .Where(c => c.Id == id)
                .Select(c => new GetCategoryByIdDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null,
                    Subcategories = c.Subcategories.Select(sc => new GetAllCategoriesDto
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        ParentCategoryId = sc.ParentCategoryId
                    }).ToList()
                })
                .SingleOrDefaultAsync();

            if (category == null)
            {
                return NotFound($"Kategorin med id {id} hittades inte.");
            }

            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<GetCategoryByIdDto>> CreateCategory(CreateCategoryDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                Name = createDto.Name,
                ParentCategoryId = createDto.ParentCategoryId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = new GetCategoryByIdDto
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, result);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("Id mismatch.");
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound($"Kategorin med id {id} hittades inte.");
            }

            existingCategory.Name = updateDto.Name;
            existingCategory.ParentCategoryId = updateDto.ParentCategoryId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CategoryExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound($"Kategorin med id {id} hittades inte.");
            }

            _context.Categories.Remove(category);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Ett fel inträffade vid borttagning av kategorin.");
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
    }
}