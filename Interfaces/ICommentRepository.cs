using StockMarkets.DTOs;
using StockMarkets.Models;

namespace StockMarkets.Interfaces;

public interface ICommentRepository
{
    Task<List<Comment>> GetAllAsync();
    Task<Comment?> GetByIdAsync(int id);
    Task<Comment> CreateAsync(Comment commentModel);
    Task<Comment?> UpdateAsync(int id, UpdateCommentDto updateCommentDto);
    Task<Comment?> DeleteAsync(int id);
    Task<bool> CommentExistsAsync(int stockId, string title, string content, int? excludeId = null);
}