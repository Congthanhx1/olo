using DigitalStore.Application.Services;
using DigitalStore.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalStore.API.Controllers;

[ApiController]
[Route("api/downloads")]
[Authorize]
public class DownloadsController : ControllerBase
{
    private readonly DownloadService _download;
    private readonly ILogger<DownloadsController> _logger;

    public DownloadsController(DownloadService download, ILogger<DownloadsController> logger)
    {
        _download = download;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/downloads/{productId}/tool
    /// Trả về file nếu user đã mua, 403 nếu chưa.
    /// </summary>
    [HttpGet("{productId:int}/tool")]
    public async Task<IActionResult> DownloadTool(int productId, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        try
        {
            var result = await _download.GetDownloadAsync(userId, productId, ct);

            _logger.LogInformation("DOWNLOAD: UserId={UserId} | ProductId={ProductId} | IP={IP}",
                userId, productId, HttpContext.Connection.RemoteIpAddress);

            return File(result.FileStream, result.ContentType,
                        fileDownloadName: result.FileName,
                        enableRangeProcessing: true); // hỗ trợ resume download
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }
}
