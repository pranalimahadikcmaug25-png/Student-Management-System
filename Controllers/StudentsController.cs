using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.DTOs;
using StudentManagementSystem.Services.Interfaces;

namespace StudentManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger         = logger;
        }

        // ─── GET /api/students ────────────────────────────────────────────────────
        /// <summary>Get all students.</summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(ApiResponse<IEnumerable<StudentResponseDto>>.SuccessResult(
                students, $"{students.Count()} student(s) retrieved."));
        }

        // ─── GET /api/students/{id} ───────────────────────────────────────────────
        /// <summary>Get a student by ID.</summary>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student is null)
            {
                _logger.LogWarning("Student with ID {StudentId} not found.", id);
                return NotFound(ApiResponse<StudentResponseDto>.FailResult(
                    $"Student with ID {id} not found."));
            }
            return Ok(ApiResponse<StudentResponseDto>.SuccessResult(student));
        }

        // ─── POST /api/students ───────────────────────────────────────────────────
        /// <summary>Create a new student.</summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(ApiResponse<object>.FailResult("Validation failed.", errors));
            }

            var created = await _studentService.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<StudentResponseDto>.SuccessResult(created, "Student created successfully."));
        }

        // ─── PUT /api/students/{id} ───────────────────────────────────────────────
        /// <summary>Update an existing student.</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(ApiResponse<object>.FailResult("Validation failed.", errors));
            }

            var updated = await _studentService.UpdateStudentAsync(id, dto);
            if (updated is null)
            {
                return NotFound(ApiResponse<StudentResponseDto>.FailResult(
                    $"Student with ID {id} not found."));
            }

            return Ok(ApiResponse<StudentResponseDto>.SuccessResult(updated, "Student updated successfully."));
        }

        // ─── DELETE /api/students/{id} ────────────────────────────────────────────
        /// <summary>Delete a student by ID.</summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.FailResult(
                    $"Student with ID {id} not found."));
            }

            return Ok(ApiResponse<object>.SuccessResult(null!, "Student deleted successfully."));
        }
    }
}
