using Hachodromo.API.Data;
using Hachodromo.API.Helpers;
using Hachodromo.Shared.DTOs;
using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Controllers
{

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public ItemsController(DataContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Item>>> Get([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Items
                .Include(i => i.ItemImages)
                .Include(i => i.ItemCategories)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return Ok(await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync());
        }

        [HttpGet("totalPages")]
        [AllowAnonymous]
        public async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Items.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]

        public async Task<ActionResult<Item>> Getasync(int id)
        {
            var item = await _context.Items
                .Include(i => i.ItemImages)
                .Include(i => i.ItemCategories!)
                .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Item>> PostAsync(ItemDto itemDto)
        {
            try
            {
                Item newItem = new Item
                {
                    Description = itemDto.Description,
                    Name = itemDto.Name,
                    Price = itemDto.Price,
                    Stock = itemDto.Stock,
                    ItemCategories = new List<ItemCategory>(),
                    ItemImages = new List<ItemImage>()
                };

                foreach (var ItemImage in itemDto.ItemImages!)
                {
                    var photoItem = Convert.FromBase64String(ItemImage);
                    newItem.ItemImages.Add(new ItemImage { Image = await _fileStorage.SaveFileAsync(photoItem, ".jpg", "items") });
                }
                foreach (var itemCategoryId in itemDto.ItemCategoryIds!)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == itemCategoryId);
                    newItem.ItemCategories.Add(new ItemCategory { Category = category! });
                }

                _context.Items.Add(newItem);
                await _context.SaveChangesAsync();
                return Ok(itemDto);

            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un Item con ese nombre");
                }
                return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(ItemDto itemDto)
        {
            try
            {
                var item = await _context.Items
                    .Include(i => i.ItemCategories!)
                    .Include(i => i.ItemImages!)
                    .FirstOrDefaultAsync(i => i.Id == itemDto.Id);

                if (item == null)
                {
                    return NotFound();
                }

                item.Name = itemDto.Name;
                item.Description = itemDto.Description;
                item.Price = itemDto.Price;
                item.Stock = itemDto.Stock;

                // Reemplazar categorías
                item.ItemCategories = itemDto.ItemCategoryIds!
                    .Select(id => new ItemCategory { CategoryId = id, ItemId = item.Id }).ToList();

                await _context.SaveChangesAsync();
                return Ok(itemDto);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un item con el mismo nombre.");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            _context.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("addImages")]
        public async Task<ActionResult> PostAddImagesAsync(ImageDto imageDto)
        {
            var item = await _context.Items
                .Include(x => x.ItemImages)
                .FirstOrDefaultAsync(x => x.Id == imageDto.ItemId);
            if (item == null)
            {
                return NotFound();
            }

            if (item.ItemImages is null)
            {
                item.ItemImages = new List<ItemImage>();
            }

            for (int i = 0; i < imageDto.Images.Count; i++)
            {
                if (!imageDto.Images[i].StartsWith("https://hachodromoproject.blob.core.windows.net/items"))
                {
                    var photoItem = Convert.FromBase64String(imageDto.Images[i]);
                    imageDto.Images[i] = await _fileStorage.SaveFileAsync(photoItem, ".jpg", "items");
                    item.ItemImages!.Add(new ItemImage { Image = imageDto.Images[i] });
                }
            }

            _context.Update(item);
            await _context.SaveChangesAsync();
            return Ok(imageDto);
        }

        [HttpPost("removeLastImage")]
        public async Task<ActionResult> PostRemoveLastImageAsync(ImageDto imageDto)
        {
            var item = await _context.Items
                .Include(x => x.ItemImages)
                .FirstOrDefaultAsync(x => x.Id == imageDto.ItemId);
            if (item == null)
            {
                return NotFound();
            }

            if (item.ItemImages is null || item.ItemImages.Count == 0)
            {
                return Ok();
            }

            var lastImage = item.ItemImages.LastOrDefault();
            await _fileStorage.RemoveFileAsync(lastImage!.Image, "items");
            item.ItemImages.Remove(lastImage);
            _context.Update(item);
            await _context.SaveChangesAsync();
            imageDto.Images = item.ItemImages.Select(x => x.Image).ToList();
            return Ok(imageDto);
        }

    }
}
