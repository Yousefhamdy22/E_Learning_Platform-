using Application.Features.Course.Dtos;
using Application.Features.Exam.Dtos;
using Application.Features.Lecture.Dtos;
using Application.Features.Students.Dtos;
using Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours.Interfaces
{
    public interface IStudentService
    {
      
        #region Curd
     
        //Task<Result<StudentDto>> CreateStudentAsync(CreateStudentDto dto, CancellationToken ct);
        //Task<Result<StudentDto>> UpdateStudentAsync(Guid id, StudentDto dto);
        Task<IEnumerable<StudentDto>> GetStudentsByCourseAsync(Guid courseId, CancellationToken ct);
        Task<StudentDto> GetStudentAsync(Guid studentId, CancellationToken ct);
        #endregion

       
        #region Enrollment

        //Task<StudentWithEnrollmentsDto> GetWithEnrollmentsAsync(Guid studentId, CancellationToken ct);
        //Task<Result<Success>> EnrollInCourseAsync(Guid studentId, Guid courseId, CancellationToken ct);
        #endregion
       

        #region Courses & Lectures


        Task<IReadOnlyList<CourseDto>> GetCoursesAsync(Guid studentId); // only enrolled
        Task<IReadOnlyList<LectureDto>> GetLecturesAsync(Guid studentId, Guid courseId);
        #endregion


        #region Exams


        //Task<IReadOnlyList<ZoomDto>> GetExamsAsync(Guid studentId, Guid courseId); // only available exams
        //Task<Result<ExamSessionDto>> StartExamAsync(Guid studentId, Guid examId);
        //Task<Result<Success>> SubmitExamAnswersAsync(Guid studentId, Guid examId, List<StudentAnswerDto> answers);
        //Task<ExamResultDto> GetExamResultAsync(Guid studentId, Guid examId);

        #endregion
    }
}
