using StockMarkets.DTOs;
using StockMarkets.Models;

namespace StockMarkets.Mappers;

public static class CommentMapper
{
    public static CommentDto ToCommentDto(this Comment commentModel)
    {
        return new CommentDto
        {
            Id = commentModel.Id,
            Content = commentModel.Content,
            Title = commentModel.Title,
            CreatedAt = commentModel.CreatedAt,
            StockId = commentModel.StockId,
        };
    }
    
    public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int stockId)
    {
        return new Comment
        {
            StockId = stockId,
            Title = commentDto.Title,
            Content = commentDto.Content,
        };
    }
    
    public static void ToCommentFromUpdate (this UpdateCommentDto updateDto, Comment commentModel)
    {
        commentModel.Title = updateDto.Title;
        commentModel.Content = updateDto.Content;
    }
}