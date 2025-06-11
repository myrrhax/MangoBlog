﻿using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
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

    public async Task<MediaFile?> GetMediaFile(Guid fileId)
    {
        return await _context.MediaFiles.FindAsync(fileId);
    }

    public async Task<Result<List<MediaFile>>> GetMediaFiles(List<Guid> ids)
    {
        var entities = await _context.MediaFiles
            .Where(entity => ids.Contains(entity.Id))
            .ToListAsync();

        return entities.Count == ids.Count
            ? Result.Success(entities)
            : Result.Failure<List<MediaFile>>(new SomeMediasAreAbsent());
    }

    public async Task<(Stream, MediaFileType)?> LoadFile(Guid fileId)
    {
        MediaFile? file = await GetMediaFile(fileId);

        if (file is null || !File.Exists(file.FilePath))
            return null;

        Stream fs = File.OpenRead(file.FilePath);
        return (fs, file.FileType);
    }

    // ToDo перенести загрузку в Background Task-у через канал и отправлять уведомления о загрузке
    public async Task<Result<MediaFile>> LoadFileToServer(Stream fileStream,
        string extention,
        Guid creatorId,
        bool isAvatar)
    {
        DateTime creationDate = DateTime.UtcNow;
        string pathToDirectory = Path.Combine(uploadPath,
            creationDate.Year.ToString(),
            creationDate.Month.ToString(),
            creationDate.Day.ToString());

        Guid fileId = Guid.NewGuid();
        string fileName = fileId.ToString() + extention;
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
                if (isAvatar)
                {
                    image.Mutate(img => img.Resize(new ResizeOptions
                    {
                        Size = new Size(AvatarWidth, AvatarHeight),
                        Mode = ResizeMode.Crop
                    }));
                }

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
                Id = fileId,
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
