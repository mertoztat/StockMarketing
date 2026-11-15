using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockMarkets.DTOs;
using StockMarkets.Extensions;
using StockMarkets.Interfaces;
using StockMarkets.Mappers;
using StockMarkets.Models;

namespace StockMarkets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository,
            UserManager<AppUser> userManager)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
            _userManager = userManager;
        }

        // GET: api/Comment
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentRepository.GetAllAsync();
            var commentDtos = comments.Select(c => c.ToCommentDto()).ToList();
            return Ok(commentDtos);
        }

        // GET: api/Comment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById([FromRoute] int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        // POST api/Comment/5
        [HttpPost("{stockId}")]
        public async Task<IActionResult> CreateComment([FromRoute] int stockId, [FromBody] CreateCommentDto createDto)
        {
            if (!await _stockRepository.StockExist(stockId))
            {
                return NotFound("Stock does not exist.");
            }

            if (await _commentRepository.CommentExistsAsync(stockId, createDto.Title, createDto.Content))
            {
                return Conflict("The same comment already exists for this stock.");
            }

            string userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var commentModel = createDto.ToCommentFromCreate(stockId);
            commentModel.AppUserId = appUser.Id;
            var comment = await _commentRepository.CreateAsync(commentModel);
            return CreatedAtAction(
                nameof(GetCommentById),
                new { id = comment.Id },
                comment.ToCommentDto());
        }

        // PUT api/Comment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto updateDto)
        {
            var existingComment = await _commentRepository.GetByIdAsync(id);
            if (existingComment == null)
            {
                return NotFound("Comment not found.");
            }

            if (await _commentRepository.CommentExistsAsync(existingComment.StockId, updateDto.Title, updateDto.Content, id))
            {
                return Conflict("The same comment already exists for this stock.");
            }

            var updatedComment = await _commentRepository.UpdateAsync(id, updateDto);
            if (updatedComment == null)
            {
                return NotFound("Comment not found.");
            }

            return Ok(updatedComment.ToCommentDto());
        }

        // DELETE api/Comment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            var deletedComment = await _commentRepository.DeleteAsync(id);
            if (deletedComment == null)
            {
                return NotFound("Comment not found.");
            }

            return NoContent();
        }
    }
}
