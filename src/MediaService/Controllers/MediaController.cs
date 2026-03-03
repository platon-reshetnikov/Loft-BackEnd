using AutoMapper;
using Loft.Common.DTOs;
using MediaService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IMapper _mapper;

        public MediaController(IMediaService mediaService, IMapper mapper) { 
            _mediaService = mediaService;
            _mapper = mapper;
        }

        [HttpPost("upload/{category}")]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                return BadRequest(new { Message = "File is empty or not provided" });
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Unauthorized();

                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized();

                var media = await _mediaService.UploadFileAsync(dto.File, dto.Category, userId, dto.IsPrivate);
                
                if (!media.IsPrivate)
                {
                    return Ok(media.Url);
                }

                return Ok(media.Id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
        }

        [HttpPost("token/{mediaId}")]
        [Authorize]
        public async Task<IActionResult> GenerateToken(Guid mediaId, [FromQuery] int hours = 24)
        {
            var token = await _mediaService.GenerateDownloadTokenAsync(mediaId, TimeSpan.FromHours(hours));
            return Ok(new { downloadUrl = $"/api/media/download?token={token.Token}", expiresAt = token.ExpiresAt });
        }
        
        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string token)
        {
            try
            {
                var (content, contentType, originalName) = await _mediaService.DownloadFileByTokenAsync(token);
                return File(content, contentType, originalName);
            }
            catch
            {
                return Unauthorized();
            }
        }
        
        [HttpDelete("{mediaId}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid mediaId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();


            try
            {
                var success = await _mediaService.DeleteFileAsync(mediaId, userId);
                if (!success)
                    return NotFound(new { message = "File not found" });

                return Ok(new { message = "File deleted successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("by-url")]
        [Authorize]
        public async Task<IActionResult> DeleteByUrl([FromQuery] string fileUrl)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();


            try
            {
                var success = await _mediaService.DeleteFileByUrlAsync(fileUrl, userId);
                if (!success) return NotFound(new { Message = "File not found" });
                return Ok(new { Message = "File deleted successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("my-public-files")]
        [Authorize]
        public IActionResult GetMyPublicFiles()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();


            var files = _mediaService.GetFilesByUser(userId)
                                     .Where(f => !f.IsPrivate)
                                     .ToList();

            var result = _mapper.Map<List<MyPublicMediaFileDTO>>(files);

            return Ok(result);
        }
    }
}
