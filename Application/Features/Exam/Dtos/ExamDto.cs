﻿using Application.Features.Students.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Dtos
{
    public class ExamDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public Guid CourseId { get; set; }
        public int DurationMinutes { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public List<QuestionDto> Questions { get; set; } = new();

    }

    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = default!;
        public decimal Point { get; set; } = default!;
        //public int Order { get; set; } = default!;

        public string? ImageUrl { get; set; }
      
        public List<AnswerOptionDto> Answers { get; set; } = new();
    }


    public class AnswerOptionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = default!;
        public bool IsCorrect { get; set; }
    }

    public class ExamResultDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public double Score { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public List<StudentAnswerDto> StudentAnswers { get; set; } = new();
    }

    public class ExamResultDetailDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }
        public string ExamTitle { get; set; } = default!;
        public string StudentName { get; set; } = default!;
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public double Score { get; set; }
        public string Status { get; set; } = default!;
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public List<StudentAnswerDetailDto> StudentAnswers { get; set; } = new();
    }

    public class StudentAnswerDetailDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = default!;
        public Guid? SelectedAnswerId { get; set; }
        public string? SelectedAnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public List<AnswerOptionDto> CorrectAnswers { get; set; } = new();
        public decimal QuestionPoints { get; set; }
        public decimal PointsAwarded { get; set; }
    }
}
