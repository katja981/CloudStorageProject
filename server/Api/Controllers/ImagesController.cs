using System.Security.Claims;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly ObjectStorageService _objectStorageService;
    private readonly AppDbContext _dbContext;
    
    public ImagesController(ObjectStorageService objectStorageService, AppDbContext dbContext)
    {
        _objectStorageService = objectStorageService;
        _dbContext = dbContext;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return  Unauthorized();
        }

        var key = Guid.NewGuid().ToString("N");

        await using var stream = file.OpenReadStream();
        await _objectStorageService.UploadAsync(key, stream);

        var image = new ImageObject
        {
            ObjectKey = key,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UploadedAt = DateTimeOffset.UtcNow,
            UploadedByUserId = userId
        };

        _dbContext.ImageObjects.Add(image);
        await  _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetImage), new { key = key }, new
        {
            key,
            image.FileName,
            image.UploadedAt
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var images = await _dbContext.ImageObjects
            .OrderByDescending(i => i.UploadedAt)
            .Select(i => new
            {
                i.ObjectKey,
                i.FileName,
                i.UploadedAt,
                i.UploadedByUserId
            })
            .ToListAsync();
        return Ok(images);
    }

    [HttpGet("{key}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImage(string key)
    {
        var image = await _dbContext.ImageObjects
            .FirstOrDefaultAsync(i => i.ObjectKey == key);

        if (image == null)
        {
            return NotFound();
        }

        var stream = await _objectStorageService.DownloadAsync(key);
        return File(stream, image.ContentType);
    }

    [HttpDelete("{key}")]
    [Authorize]
    public async Task<IActionResult> DeleteImage(string key)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var image = await _dbContext.ImageObjects
            .FirstOrDefaultAsync(i => i.ObjectKey == key);

        if (image == null)
        {
            return  NotFound();
        }

        if (image.UploadedByUserId != userId)
        {
            return Forbid();
        }

        await _objectStorageService.DeleteAsync(key);

        _dbContext.ImageObjects.Remove(image);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}