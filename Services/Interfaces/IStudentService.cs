using StudentManagementSystem.DTOs;

namespace StudentManagementSystem.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync();
        Task<StudentResponseDto?> GetStudentByIdAsync(int id);
        Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto dto);
        Task<StudentResponseDto?> UpdateStudentAsync(int id, UpdateStudentDto dto);
        Task<bool> DeleteStudentAsync(int id);
    }
}
