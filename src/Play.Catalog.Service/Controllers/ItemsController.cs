using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly ItemsRepository repository = new();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync(CancellationToken ct)
        {
            var items = (await repository.GetAllAsync(ct))
                        .Select(item => item.AsDto());
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetById(Guid id, CancellationToken ct)
        {
            var item = await repository.GetAsync(id, ct);

            if (item is null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        //POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> Post(CreateItemDto dto, CancellationToken ct)
        {
            var item = new Item
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateAsync(item, ct);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateItemDto dto, CancellationToken ct)
        {
            var existingItem = await repository.GetAsync(id, ct);

            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.Name = dto.Name;
            existingItem.Description = dto.Description;
            existingItem.Price = dto.Price;

            await repository.UpdateAsync(existingItem, ct);

            return NoContent();
        }

        //DELELE /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var item = await repository.GetAsync(id, ct);

            if (item is null)
            {
                return NotFound();
            }

            await repository.RemoveAsync(id, ct);

            return NoContent();
        }

    }


}