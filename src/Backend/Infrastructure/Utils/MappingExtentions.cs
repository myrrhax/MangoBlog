﻿using System.Reflection.Metadata;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Infrastructure.MongoModels;
using Infrastructure.Utils;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Infrastructure;

internal static class MappingExtentions
{
    public static Article MapToEntity(this ArticleDocument document)
    {
        return new Article(document.Id,
            document.Title,
            document.Content.ToDictionary(),
            document.CreatorId,
            document.Tags,
            document.Likes,
            document.Dislikes,
            document.CreationDate,
            document.CoverImageId);
    }

    public static ArticleDocument ToDocument(this Article article)
    {
        string documentId = ObjectId.TryParse(article.Id, out ObjectId id) 
            ? id.ToString()
            : string.Empty;

        return new ArticleDocument
        {
            Id = documentId,
            Title = article.Title,
            CreatorId = article.CreatorId,
            CreationDate = article.CreationDate,
            Tags = article.Tags.ToList(),
            Content = BsonDocument.Parse(JsonConvert.SerializeObject(article.Content)),
            Likes = article.Likes,
            Dislikes = article.Dislikes,
            CoverImageId = article.CoverImageId
        };
    }

    public static IntegrationPublicationInfoDocument MapToDocument(this IntegrationPublishInfo entity)
        => new IntegrationPublicationInfoDocument()
        {
            IntegrationType = entity.IntegrationType,
            PublishStatuses = entity.PublishStatuses,
        };

    public static MediaFileTuple MapToMediaTuple(this (Guid Id, MediaFileType Type) entity)
        => new MediaFileTuple() { MediaId = entity.Id, Type = entity.Type };

    public static Publication MapToEntity(this PublicationDocument document)
        => new Publication()
        {
            PublicationId = document.PublicationId.ToString(),
            UserId = document.UserId,
            Content = document.Content,
            MediaFiles = document.Media.Select(media => (media.MediaId, media.Type)).ToList(),
            CreationDate = document.CreationDate,
            PublicationTime = document.PublicationTime,
            IntegrationPublishInfos = document.IntegrationPublishInfos.Select(document => document.MapToEntity()).ToList(),
        };

    public static IntegrationPublishInfo MapToEntity(this IntegrationPublicationInfoDocument document)
        => new IntegrationPublishInfo()
        {
            IntegrationType = document.IntegrationType,
            PublishStatuses = document.PublishStatuses,
        };
}
