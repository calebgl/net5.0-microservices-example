using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{
  [ApiController]
  [Route("items")]
  public class ItemsController : ControllerBase
  {
    // It is declared as static so that it is not reacreated every time someone calls an API method
    private static readonly List<ItemDto> items = new()
    {
      new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
      new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
      new ItemDto(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
    };

    [HttpGet]
    public IEnumerable<ItemDto> Get()
    {
      return items;
    }

    [HttpGet("{id}")]
    public ActionResult<ItemDto> GetById(Guid id)
    {
      var item = items.Where(item => item.Id == id).SingleOrDefault();

      if (item == null)
      {
        return NotFound();
      }

      return item;
    }

    [HttpPost]
    public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
    {
      var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
      items.Add(item);
      // GetById is passed to create a header named location
      // with a value of item.Id to tell the client what route of this new item is 
      return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    // The main difference between ActionResult and IActionResult is that the first one
    // is generic, meaning we can specifiy a type to use
    public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
    {
      var existingItem = items.Where(item => item.Id == id).FirstOrDefault();

      if (existingItem == null)
      {
        return NotFound();
      }

      var updatedItem = existingItem with
      {
        Name = updateItemDto.Name,
        Description = updateItemDto.Description,
        Price = updateItemDto.Price
      };

      var index = items.FindIndex(existingItem => existingItem.Id == id);
      items[index] = updatedItem;

      return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
      var index = items.FindIndex(existingItem => existingItem.Id == id);

      if (index < 0)
      {
        return NotFound();
      }

      items.RemoveAt(index);
      return NoContent();
    }
  }
}
