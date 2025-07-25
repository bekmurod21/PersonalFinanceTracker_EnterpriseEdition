﻿namespace PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;

public static class PasswordHelper
{
    public static string Hash(this string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public static bool Verify(string password,string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}