using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.DTOs
{
    // ─── Request DTOs ────────────────────────────────────────────────────────────

    public class CreateStudentDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Course is required.")]
        [MaxLength(100, ErrorMessage = "Course cannot exceed 100 characters.")]
        public string Course { get; set; } = string.Empty;
    }

    public class UpdateStudentDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Course is required.")]
        [MaxLength(100)]
        public string Course { get; set; } = string.Empty;
    }

    // ─── Response DTOs ───────────────────────────────────────────────────────────

    public class StudentResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Course { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
