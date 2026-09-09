
using System.ComponentModel.DataAnnotations;

namespace week_26_27_task.Validations
{
    public class FutureDateTimeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not DateTime dateTime)
                return false;

            return dateTime > DateTime.Now;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a future date and time.";
        }
    }
}
