
using System.ComponentModel.DataAnnotations;

namespace week_26_27_task.Validations
{
    public class LetterOnlyAttribute : ValidationAttribute
    {
        public override bool IsValid(object? val)
        {
            if (val is not string str)
                return false;

             str = str.Trim();

            if (string.IsNullOrEmpty(str) || str.Length < 2)
                return false; 

            return str.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must contain only words of letters.";
        }
    }
}
