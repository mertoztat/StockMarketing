using Microsoft.EntityFrameworkCore;
using StockMarkets.Data;
using StockMarkets.DTOs;
using StockMarkets.Interfaces;
using StockMarkets.Models;

namespace StockMarkets.Repository;

public class CommentRepository : ICommentRepository
{
     private readonly ApplicationDBContext _context;
    public CommentRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetAllAsync()
    {
        return await _context.Comments.Include(c => c.AppUser).ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _context.Comments.Include(c => c.AppUser).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Comment> CreateAsync(Comment commentModel)
    {
        await _context.Comments.AddAsync(commentModel);
        await _context.SaveChangesAsync();
        return commentModel;
    }

    public async Task<Comment?> UpdateAsync(int id, UpdateCommentDto updateCommentDto)
    {
        var existingComment = await _context.Comments.FindAsync(id);
        if (existingComment == null) return null;

        _context.Entry(existingComment).CurrentValues.SetValues(updateCommentDto);
        await _context.SaveChangesAsync();
        return existingComment;
    }

    public async Task<Comment?> DeleteAsync(int id)
    {
        var existingComment = await _context.Comments.FindAsync(id);
        if (existingComment == null) return null;

        _context.Comments.Remove(existingComment);
        await _context.SaveChangesAsync();
        return existingComment;
    }

    public Task<bool> CommentExistsAsync(int stockId, string title, string content, int? excludeId = null)
    {
        return _context.Comments.AnyAsync(c =>
            c.StockId == stockId &&
            c.Title == title &&
            c.Content == content &&
            c.Id != excludeId);
    }
}
