using Microsoft.AspNetCore.Identity;
using StockMarkets.DTOs;
using StockMarkets.Exceptions;
using StockMarkets.Interfaces;
using StockMarkets.Mappers;
using StockMarkets.Models;

namespace StockMarkets.Service;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IStockRepository _stockRepository;
    private readonly UserManager<AppUser> _userManager;

    public CommentService(ICommentRepository commentRepository, IStockRepository stockRepository,
        UserManager<AppUser> userManager)
    {
        _commentRepository = commentRepository;
        _stockRepository = stockRepository;
        _userManager = userManager;
    }

    public Task<List<Comment>> GetAllAsync()
    {
        return _commentRepository.GetAllAsync();
    }

    public async Task<Comment> GetByIdAsync(int id)
    {
        return await _commentRepository.GetByIdAsync(id)
               ?? throw new NotFoundException("Comment not found.");
    }

    public async Task<Comment> CreateAsync(string userId, int stockId, CreateCommentDto createDto)
    {
        // Resolve the author first: an unknown caller must never get as far as touching stock data.
        var user = string.IsNullOrEmpty(userId) ? null : await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UnauthenticatedException("User not found.");

        if (!await _stockRepository.StockExist(stockId))
            throw new NotFoundException("Stock does not exist.");

        if (await _commentRepository.CommentExistsAsync(stockId, createDto.Title, createDto.Content))
            throw new ConflictException(ErrorCodes.DuplicateComment,
                "The same comment already exists for this stock.");

        var comment = createDto.ToCommentFromCreate(stockId);
        comment.AppUserId = user.Id;
        // Also set the navigation so the returned comment can report who wrote it (CreatedBy).
        comment.AppUser = user;

        return await _commentRepository.CreateAsync(comment);
    }

    public async Task<Comment> UpdateAsync(int id, UpdateCommentDto updateDto)
    {
        var existing = await GetByIdAsync(id);

        if (await _commentRepository.CommentExistsAsync(existing.StockId, updateDto.Title, updateDto.Content, id))
            throw new ConflictException(ErrorCodes.DuplicateComment,
                "The same comment already exists for this stock.");

        return await _commentRepository.UpdateAsync(id, updateDto)
               ?? throw new NotFoundException("Comment not found.");
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _commentRepository.DeleteAsync(id);
        if (deleted is null)
            throw new NotFoundException("Comment not found.");
    }
}
