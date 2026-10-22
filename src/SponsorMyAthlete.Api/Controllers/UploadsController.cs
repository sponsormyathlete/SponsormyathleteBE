using Microsoft.AspNetCore.Mvc;
using SponsorMyAthlete.Application.Users;

namespace SponsorMyAthlete.Api.Controllers;

[Route("api/uploads")]
public class UploadsController(IUserSyncService userSyncService, IWebHostEnvironment env) : ApiControllerBase(userSyncService)
{
    private static readonly Dictionary<string, string> AllowedContentTypes = new()
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
    };

    private const long MaxBytes = 5 * 1024 * 1024;

    [HttpPost("photo")]
    public async Task<ActionResult<UploadedFileDto>> UploadPhoto(IFormFile file, CancellationToken ct)
    {
        await GetCurrentUserIdAsync(ct);

        if (file.Length == 0)
            return BadRequest("No file provided.");
        if (file.Length > MaxBytes)
            return BadRequest("File too large (max 5MB).");
        if (!AllowedContentTypes.TryGetValue(file.ContentType, out var extension))
            return BadRequest("Only JPEG, PNG, or WebP images are allowed.");

        var webRoot = string.IsNullOrEmpty(env.WebRootPath)
            ? Path.Combine(env.ContentRootPath, "wwwroot")
            : env.WebRootPath;
        var uploadsDir = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsDir, fileName);
        await using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream, ct);

        var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        return Ok(new UploadedFileDto(url));
    }
}

public record UploadedFileDto(string Url);
