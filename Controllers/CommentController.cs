using Microsoft.AspNetCore.Mvc;
using StockMarkets.DTOs;
using StockMarkets.Extensions;
using StockMarkets.Interfaces;
using StockMarkets.Mappers;

namespace StockMarkets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: api/Comment
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentService.GetAllAsync();
            return Ok(comments.Select(c => c.ToCommentDto()).ToList());
        }

        // GET: api/Comment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById([FromRoute] int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            return Ok(comment.ToCommentDto());
        }

        // POST api/Comment/5
        [HttpPost("{stockId}")]
        public async Task<IActionResult> CreateComment([FromRoute] int stockId, [FromBody] CreateCommentDto createDto)
        {
            var comment = await _commentService.CreateAsync(User.GetUserId(), stockId, createDto);
            return CreatedAtAction(
                nameof(GetCommentById),
                new { id = comment.Id },
                comment.ToCommentDto());
        }

        // PUT api/Comment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto updateDto)
        {
            var updated = await _commentService.UpdateAsync(id, updateDto);
            return Ok(updated.ToCommentDto());
        }

        // DELETE api/Comment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            await _commentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
