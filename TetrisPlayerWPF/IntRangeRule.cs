using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TetrisPlayerWPF
{
    public class IntRangeRule : ValidationRule
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public IntRangeRule()
        {
            //既定値  
            MinValue = int.MinValue;
            MaxValue = int.MaxValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null) return new ValidationResult(false, "値がNullです");

            int inputNum;
            if (!int.TryParse(value.ToString(), out inputNum))
            {
                return new ValidationResult(false, "値の形式が不正です");
            }

            if (inputNum < MinValue || MaxValue < inputNum)
            {
                return new ValidationResult(false, "値が範囲外です");
            }

            return ValidationResult.ValidResult;
        }
    }
}
