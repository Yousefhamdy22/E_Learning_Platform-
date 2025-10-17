using Domain.Common;
using System;


namespace Domain.Entities.Exams
{
    public class AnswerOption : AuditableEntity
    {
       
        #region Properties
        public string Text { get; private set; } = default!;
        public bool IsCorrect { get; private set; }
        #endregion

        #region Foreign Keys
        public Guid QuestionId { get; private set; }
        public Question Question { get; private set; } = default!;
        #endregion

        #region Constructor
        private AnswerOption() { }

        public AnswerOption(Guid questionId, string text, bool isCorrect)
        {
            QuestionId = questionId;
            SetText(text);
            SetIsCorrect(isCorrect);
        }
        #endregion

        #region Behaviors
        public void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Answer option text cannot be empty");
            Text = text.Trim();
        }

        public void SetIsCorrect(bool isCorrect)
        {
            IsCorrect = isCorrect;
        }
        #endregion
    }
}
