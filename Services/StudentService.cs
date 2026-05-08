using StudentManagementSystem.DTOs;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories.Interfaces;
using StudentManagementSystem.Services.Interfaces;

namespace StudentManagementSystem.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IStudentRepository studentRepository, ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync()
        {
            _logger.LogInformation("Fetching all students.");
            var students = await _studentRepository.GetAllAsync();
            return students.Select(MapToResponseDto);
        }

        public async Task<StudentResponseDto?> GetStudentByIdAsync(int id)
        {
            _logger.LogInformation("Fetching student with ID: {StudentId}", id);
            var student = await _studentRepository.GetByIdAsync(id);
            return student is null ? null : MapToResponseDto(student);
        }

        public async Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto dto)
        {
            _logger.LogInformation("Creating student with email: {Email}", dto.Email);

            // Check for duplicate email
            var existing = await _studentRepository.GetByEmailAsync(dto.Email);
            if (existing is not null)
                throw new InvalidOperationException($"A student with email '{dto.Email}' already exists.");

            var student = new Student
            {
                Name     = dto.Name.Trim(),
                Email    = dto.Email.Trim().ToLower(),
                Age      = dto.Age,
                Course   = dto.Course.Trim(),
                CreatedDate = DateTime.UtcNow
            };

            var created = await _studentRepository.CreateAsync(student);
            _logger.LogInformation("Student created successfully with ID: {StudentId}", created.Id);
            return MapToResponseDto(created);
        }

        public async Task<StudentResponseDto?> UpdateStudentAsync(int id, UpdateStudentDto dto)
        {
            _logger.LogInformation("Updating student with ID: {StudentId}", id);

            var student = await _studentRepository.GetByIdAsync(id);
            if (student is null) return null;

            // If email changed, ensure uniqueness
            if (!string.Equals(student.Email, dto.Email.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var emailOwner = await _studentRepository.GetByEmailAsync(dto.Email);
                if (emailOwner is not null && emailOwner.Id != id)
                    throw new InvalidOperationException($"Email '{dto.Email}' is already in use by another student.");
            }

            student.Name   = dto.Name.Trim();
            student.Email  = dto.Email.Trim().ToLower();
            student.Age    = dto.Age;
            student.Course = dto.Course.Trim();

            var updated = await _studentRepository.UpdateAsync(student);
            _logger.LogInformation("Student ID: {StudentId} updated successfully.", id);
            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            _logger.LogInformation("Deleting student with ID: {StudentId}", id);
            var result = await _studentRepository.DeleteAsync(id);
            if (result)
                _logger.LogInformation("Student ID: {StudentId} deleted successfully.", id);
            else
                _logger.LogWarning("Student ID: {StudentId} not found for deletion.", id);
            return result;
        }

        // ─── Mapper ───────────────────────────────────────────────────────────────
        private static StudentResponseDto MapToResponseDto(Student s) => new()
        {
            Id          = s.Id,
            Name        = s.Name,
            Email       = s.Email,
            Age         = s.Age,
            Course      = s.Course,
            CreatedDate = s.CreatedDate
        };
    }
}
