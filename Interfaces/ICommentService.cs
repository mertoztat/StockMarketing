using StockMarkets.DTOs;
using StockMarkets.Models;

namespace StockMarkets.Interfaces;

public interface ICommentService
{
    Task<List<Comment>> GetAllAsync();

    /// <exception cref="Exceptions.NotFoundException">No comment with this id.</exception>
    Task<Comment> GetByIdAsync(int id);

    /// <exception cref="Exceptions.UnauthenticatedException">The user id is empty or does not belong to an existing user.</exception>
    /// <exception cref="Exceptions.NotFoundException">The stock does not exist.</exception>
    /// <exception cref="Exceptions.ConflictException">The same title and content already exist for this stock.</exception>
    Task<Comment> CreateAsync(string userId, int stockId, CreateCommentDto createDto);

    /// <exception cref="Exceptions.NotFoundException">No comment with this id.</exception>
    /// <exception cref="Exceptions.ConflictException">The same title and content already exist for this stock.</exception>
    Task<Comment> UpdateAsync(int id, UpdateCommentDto updateDto);

    /// <exception cref="Exceptions.NotFoundException">No comment with this id.</exception>
    Task DeleteAsync(int id);
}
