using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAquisition.Helpers
{
    public static class GenericHelper
    {
        public static bool ValidateLabel(string label, string[] acceptableLabels, bool throwException = true)
        {
            label = CleanLabel(label);
            int result = acceptableLabels.Where(s => !string.IsNullOrEmpty(label) && label.ToLower().Trim().StartsWith(s.ToLower().Trim())).ToArray().Length;
            if (result < 1)
            {
                if (throwException)
                {
                    ExceptionThrower("201 - Configuration error. The field " + acceptableLabels[0] + " is misplaced.");
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsLabel(string label, string[] acceptableLabels, bool throwException = true)
        {
            label = CleanLabel(label);
            int result = acceptableLabels.Where(s => !string.IsNullOrEmpty(label) && label.ToLower().Trim().Contains(s.ToLower().Trim())).ToArray().Length;
            if (result < 1)
            {
                if (throwException)
                {
                    ExceptionThrower("201 - Configuration error. The field " + acceptableLabels[0] + " is misplaced.");
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static void ValidateLabel(string label, string acceptableLabel, bool throwException = true)
        {
            acceptableLabel = CleanLabel(acceptableLabel);

            string[] arr = new string[] { acceptableLabel };
            ValidateLabel(label, arr, throwException);
        }

        private static string CleanLabel(string label)
        {
            char[] remove = new char[] { ':' };
            return String.Concat(label.Split(remove,
                StringSplitOptions.None));
        }

        public static void ExceptionThrower(string message)
        {           
            throw new Exception(message);
        }

        public static string CleanString(string text)
        {
            if (String.IsNullOrEmpty(text) || text.ToLower().Equals("null")) return "";

            text = text.Contains("\n") ? text.Replace("\n", "") : text;
            text = text.Contains("\t") ? text.Replace("\t", "") : text;
            text = text.Contains("\r") ? text.Replace("\r", "") : text;
            text = text.Contains("&nbsp;") ? text.Replace("&nbsp;", "") : text;
            text = text.Contains("&amp;") ? text.Replace("&amp;", "&") : text;

            return text.Trim();
        }

        public static decimal? ConvertToDecimal(string value, string fieldName, bool isMandatory = true)
        {
            bool negative = value.Contains("-");
            decimal number;
            value = GenericHelper.CleanString(value);
            value = value.Replace("R$", "");
            value = value.Replace("$", "");
            value = value.Replace("£", "");
            value = value.Replace("€", "");
            value = value.Replace("%", "");
            value = value.Replace("(", "");
            value = value.Replace(")", "");
            value = value.Replace("-", "");
            value = value.Replace(" ", "");
            value = value.Replace("USD", "");

            //INNO-2078
            //if (value.Count(x => x == '.') > 1) { value = value.Replace(".", ""); }

            bool americanFormat = value.IndexOf(",") < value.IndexOf(".");

            //Realiza a conversão

            if (!String.IsNullOrEmpty(value))
            {
                try
                {
                    if (!americanFormat)
                    {
                        number = decimal.Parse(value, new CultureInfo("pt-BR"));
                    }
                    else
                    {
                        number = decimal.Parse(value, new CultureInfo("en-US"));
                    }
                    if (negative)
                    {
                        number *= -1;
                    }
                    return number;
                }
                catch
                {
                    if (isMandatory) GenericHelper.ExceptionThrower("The field " + fieldName + " is in a incorrect/unexpected format");
                }
            }
            else if (isMandatory)
            {
                GenericHelper.ExceptionThrower("The field " + fieldName + " is mandatory");
            }
            return null;
        }

        public static DateTime? ConvertToDatetime(string value, string fieldName, string[] expectedFormats, bool isMandatory = true)
        {
            value = GenericHelper.CleanString(value);

            if (!string.IsNullOrEmpty(value))
            {
                try
                {

                    return DateTime.ParseExact(value, expectedFormats, CultureInfo.InvariantCulture,
                                                   DateTimeStyles.AssumeUniversal |
                                                   DateTimeStyles.AdjustToUniversal);
                }
                catch
                {
                    if (isMandatory) GenericHelper.ExceptionThrower("The field " + fieldName + " is in a incorrect/unexpected format " + value + ". The expected formats are: " + string.Join(",", expectedFormats));
                }
            }
            else if (isMandatory)
            {
                GenericHelper.ExceptionThrower("The field " + fieldName + " is mandatory");
            }
            return null;
        }

        public static Int32? ConvertToint(string value, string fieldName, bool isMandatory = true, bool roundToInt = false)
        {
            value = GenericHelper.CleanString(value);
            if (!roundToInt) value = value.Split('.').FirstOrDefault();

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return (!roundToInt) ? int.Parse(value) : Convert.ToInt32(Math.Floor((decimal)GenericHelper.ConvertToDecimal(value, fieldName, isMandatory)));
                }
                catch
                {
                    if (isMandatory) GenericHelper.ExceptionThrower("The field " + fieldName + " is in a incorrect/unexpected format.");
                }
            }
            else if (isMandatory)
            {
                GenericHelper.ExceptionThrower("The field " + fieldName + " is mandatory");
            }
            return null;
        }
    }
}
