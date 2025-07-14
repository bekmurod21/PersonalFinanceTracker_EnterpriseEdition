using System.Text.RegularExpressions;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Models.Validations;

public static class PasswordValidatoion
{
    public static (bool IsValid, string Message) IsStrong(this string password)
    {
        string pattern = @"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z]).{8,}$";

        bool isValid = Regex.IsMatch(password, pattern);

        if (!isValid)
            return (false, "Password must contain at least 1 digit, 1 uppercase character, 1 lowercase character, and be 8 or more characters long.");

        return (true, "Valid Password");
    }
}
