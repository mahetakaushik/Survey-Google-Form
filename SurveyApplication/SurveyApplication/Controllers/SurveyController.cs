using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApplication.DTOs;
using SurveyApplication.Models;

[ApiController]
[Route("api/[controller]")]
public class SurveyController : ControllerBase
{
    private readonly AppDbContext _context;

    public SurveyController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateSurvey([FromBody] Survey survey)
    {
        try
        {
            if (survey == null)
                return BadRequest("Survey cannot be null.");

            await _context.Surveys.AddAsync(survey);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the survey: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSurvey(int id)
    {
        try
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
                return NotFound($"Survey with id {id} not found.");

            return Ok(survey);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving the survey: {ex.Message}");
        }
    }

    [HttpPost("submit-response")]
    public async Task<IActionResult> SubmitResponse([FromBody] SurveyResponseDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || dto.Answers == null || !dto.Answers.Any())
                return BadRequest("Invalid response data.");

            var surveyExists = await _context.Surveys.AnyAsync(s => s.Id == dto.SurveyId);
            if (!surveyExists)
                return NotFound("Survey not found.");

            var response = new Response
            {
                SurveyId = dto.SurveyId,
                email = dto.Email,
                IsDeleted = false,
                Answers = dto.Answers.Select(a => new ResponseAnswer
                {
                    QuestionId = a.QuestionId,
                    SelectedOptionId = a.SelectedOptionId,
                    TextAnswer = a.TextAnswer
                }).ToList()
            };

            await _context.Responses.AddAsync(response);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Survey response submitted successfully.", ResponseId = response.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while submitting the response: {ex.Message}");
        }
    }

    [HttpPut("update-response/{responseId}")]
    public async Task<IActionResult> UpdateResponse(int responseId, [FromBody] UpdateResponseDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || dto.Answers == null || !dto.Answers.Any())
                return BadRequest("Invalid response data.");

            var existingResponse = await _context.Responses
                .Include(r => r.Answers)
                .FirstOrDefaultAsync(r => r.Id == responseId && !r.IsDeleted && r.email == dto.Email);

            if (existingResponse == null)
                return NotFound("Response not found or email mismatch.");

            // Soft-delete old response
            existingResponse.IsDeleted = true;

            // Create new response
            var newResponse = new Response
            {
                SurveyId = existingResponse.SurveyId,
                email = dto.Email,
                IsDeleted = false,
                Answers = dto.Answers.Select(a => new ResponseAnswer
                {
                    QuestionId = a.QuestionId,
                    SelectedOptionId = a.SelectedOptionId,
                    TextAnswer = a.TextAnswer
                }).ToList()
            };

            await _context.Responses.AddAsync(newResponse);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Response updated by creating a new entry.",
                NewResponseId = newResponse.Id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the response: {ex.Message}");
        }
    }

    [HttpGet("responses-by-email")]
    public async Task<IActionResult> GetResponsesByEmail([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var responses = await _context.Responses
                .Where(r => r.email == email)
                .Include(r => r.Answers)
                .ThenInclude(a => a.Question)
                .ToListAsync();

            return Ok(responses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving responses: {ex.Message}");
        }
    }
}
