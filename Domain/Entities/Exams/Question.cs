using Domain.Common;
using Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Exams
{
    public class Question : AuditableEntity
    {
      

        #region Properties
        public string Text { get; private set; } = default!;
        public string? ImageUrl { get; private set; } = default!;
        public decimal? Points { get; private set; }

      
        #endregion

        #region MTOM


        private readonly List<ExamQuestions> _examQuestions = new();
        public IReadOnlyList<ExamQuestions> ExamQuestions => _examQuestions.AsReadOnly();
        #endregion

        #region Navigation
        private readonly List<AnswerOption> _answerOptions = new();
        public IReadOnlyCollection<AnswerOption> AnswerOptions => _answerOptions.AsReadOnly();
        #endregion

        #region Constructor
        private Question() { }

        private Question( string text, decimal points)
        {
           
            SetText(text);
            SetPoints(points);

        }
        #endregion

        #region Factory
        public static Result<Question> Create( string text, decimal points)
        {
          

            if (string.IsNullOrWhiteSpace(text))
                return Result<Question>.FromError(
                    Error.Validation("Question.Text.Empty", "Text is required"));

            if (points <= 0)
                return Result<Question>.FromError(
                    Error.Validation("Question.Points.Invalid", "Points must be greater than zero"));


            var question = new Question(text, points);

            return Result<Question>.FromValue(question);
        }
        #endregion

        #region Behaviors
        public void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Question text cannot be empty");
            Text = text.Trim();
        }
        public void UpdateImage(string imageUrl)
        {
            ImageUrl = imageUrl;
        }
        public void SetPoints(decimal points)
        {
            if (points <= 0)
                throw new ArgumentException("Points must be greater than zero");
            Points = points;
        }

       
        public AnswerOption AddAnswerOption(string text, bool isCorrect)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Option text cannot be empty");

            var option = new AnswerOption(this.Id, text, isCorrect);
            _answerOptions.Add(option);
            return option;
        }

        public Result<Success> ValidateMultipleChoice()
        {
            //if (_answerOptions.Count < 2)
            //    throw new InvalidOperationException("A multiple-choice question must have at least two options");

            //if (!_answerOptions.Any(o => o.IsCorrect))
            //    throw new InvalidOperationException("At least one answer option must be marked correct");

            var correctAnswers = _answerOptions.Count(a => a.IsCorrect);

            if (correctAnswers == 0)
                return Result<Success>.FromError(Error.Failure("At least one correct answer is required"));

            if (_answerOptions.Count < 2)
                return Result<Success>.FromError(Error.Failure("At least two answer options are required"));

            return Result<Success>.FromValue(new Success());
        }
        #endregion
    }
}
