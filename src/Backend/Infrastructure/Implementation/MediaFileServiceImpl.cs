using System.Security.Cryptography;
using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Implementation;

internal class MediaFileServiceImpl : IMediaFileService
{
    public const int AvatarWidth = 800;
    public const int AvatarHeight = 600;
    private static string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    private static string[] allowedImageTypes = { ".jpg", ".jpeg", ".png", ".bmp", };
    private static string[] allowedVideoTypes = { ".mp4", ".avi", ".mov", ".mkv", ".webm" };
    private static string[] validExtentions = allowedVideoTypes.Concat(allowedImageTypes).ToArray();
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MediaFileServiceImpl> _logger;

    public MediaFileServiceImpl(ApplicationDbContext context, ILogger<MediaFileServiceImpl> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task<MediaFile?> GetMediaFile(string url)
    {
        throw new NotImplementedException();
    }

    public Task<Stream?> LoadFile(string url)
    {
        throw new NotImplementedException();
    }

    // ToDo перенести загрузку в Background Task-у через канал и отправлять уведомления о загрузке
    public async Task<Result<MediaFile>> LoadFileToServer(Stream fileStream, 
        string extention, 
        string url, 
        Guid creatorId, 
        bool isAvatar)
    {
        DateTime creationDate = DateTime.UtcNow;
        string pathToDirectory = Path.Combine(uploadPath,
            creationDate.Year.ToString(),
            creationDate.Month.ToString(),
            creationDate.Day.ToString());
        string hashName = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        string fileName = hashName + extention;
        string filePath = Path.Combine(pathToDirectory, fileName);

        try
        {
            if (!Directory.Exists(pathToDirectory))
            {
                Directory.CreateDirectory(pathToDirectory);
            }
            if (allowedImageTypes.Contains(extention)) // load image and crop if it is an avatar
            {
                using var image = await Image.LoadAsync(fileStream);
                image.Mutate(img => img.Resize(new ResizeOptions
                {
                    Size = new Size(AvatarWidth, AvatarHeight),
                    Mode = ResizeMode.Crop
                }));

                await using FileStream outputStream = File.OpenWrite(filePath);
                await image.SaveAsJpegAsync(outputStream);
            }
            else if (allowedVideoTypes.Contains(extention))
            {
                await using FileStream outputStream = File.OpenWrite(filePath);
                await fileStream.CopyToAsync(outputStream);
            }
            else
            {
                return Result.Failure<MediaFile>(new InvalidFileExtention(extention, validExtentions));
            }

            var mediaFile = new MediaFile
            {
                Id = Guid.NewGuid(),
                Url = url + hashName,
                FilePath = filePath,
                LoadTime = creationDate,
                IsAvatar = isAvatar
            };

            await _context.MediaFiles.AddAsync(mediaFile);
            await _context.SaveChangesAsync();

            return Result.Success(mediaFile);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError($"Failed to add entity to database: {ex.Message}");
            return Result.Failure<MediaFile>(new DatabaseInteractionError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to upload file: {ex.Message}");
            return Result.Failure<MediaFile>(new FailedToLoadFile(ex.Message));
        }
    }
}
