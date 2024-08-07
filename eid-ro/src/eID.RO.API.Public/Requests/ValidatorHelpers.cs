﻿using System.Text.RegularExpressions;

namespace eID.RO.API.Public.Requests;

internal static class ValidatorHelpers
{
    /// <summary>
    /// Validates Individual Uid (EGN|LNCH) format.
    /// </summary>
    /// <param name="uid"></param>
    public static bool UidFormatIsValid(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
        {
            throw new ArgumentException($"'{nameof(uid)}' cannot be null or whitespace.", nameof(uid));
        }

        return EgnFormatIsValid(uid) || LnchFormatIsValid(uid);
    }

    public static bool EgnFormatIsValid(string? egn)
    {
        if (string.IsNullOrWhiteSpace(egn) || egn.Length != 10)
        {
            return false;
        }

        _ = int.TryParse(egn.Substring(0, 2), out int controlValueYear);
        _ = int.TryParse(egn.Substring(2, 2), out int controlValueMonth);
        _ = int.TryParse(egn.Substring(4, 2), out int controlValueDate);

        int month = controlValueMonth;
        int baseYear = 1900;
        if (controlValueMonth > 40)
        {
            baseYear = 2000;
            month -= 40;
        }
        else if (controlValueMonth > 20)
        {
            baseYear = 1800;
            month -= 20;
        }
        int year = baseYear + controlValueYear;

        try
        {
            var egnToDate = new DateTime(year, month, controlValueDate);
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }

        int number;
        int checkSum = 0;
        int[] weights = new[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
        for (int i = 0; i < weights.Length; i++)
        {
            number = (int)char.GetNumericValue(egn[i]);
            if (number == -1)
                return false;

            checkSum += weights[i] * number;
        }
        checkSum %= 11;
        checkSum %= 10;
        number = (int)char.GetNumericValue(egn[egn.Length - 1]);

        return checkSum == number;
    }

    public static bool LnchFormatIsValid(string? lnch)
    {
        if (string.IsNullOrWhiteSpace(lnch) || lnch.Length != 10)
        {
            return false;
        }

        int number;
        int checkSum = 0;
        int[] weights = new[] { 21, 19, 17, 13, 11, 9, 7, 3, 1 };
        if (lnch.Length < weights.Length)
        {
            return false;
        }
        for (int i = 0; i < weights.Length; i++)
        {
            number = (int)char.GetNumericValue(lnch[i]);
            if (number == -1)
                return false;

            checkSum += weights[i] * number;
        }
        checkSum %= 10;
        number = (int)char.GetNumericValue(lnch[^1]);

        return checkSum == number;
    }

    public static bool EikFormatIsValid(string eik)
    {
        if (string.IsNullOrWhiteSpace(eik))
        {
            throw new ArgumentException($"'{nameof(eik)}' cannot be null or whitespace.", nameof(eik));
        }

        //If EIK has length of 10, we have to validate it as EGN
        if (eik.Length == 10)
        {
            return EgnFormatIsValid(eik);
        }

        //If EIK has length different of 10, then we only accept length of 9 or 13
        if (eik.Length != 9 && eik.Length != 13)
        {
            return false;
        }

        return true;
    }

    public static bool IsLawfulAge(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
        {
            throw new ArgumentException($"'{nameof(uid)}' cannot be null or whitespace.", nameof(uid));
        }
        if (!EgnFormatIsValid(uid) && !LnchFormatIsValid(uid))
        {
            return false;
        }
        if (LnchFormatIsValid(uid))
        {
            return true;
        }

        var eighteenYearsAgo = DateTime.UtcNow.AddYears(-18);

        _ = int.TryParse(uid.Substring(0, 2), out int controlValueYear);
        _ = int.TryParse(uid.Substring(2, 2), out int controlValueMonth);
        _ = int.TryParse(uid.Substring(4, 2), out int controlValueDate);

        int month = controlValueMonth;
        int baseYear = 1900;
        if (controlValueMonth > 40)
        {
            baseYear = 2000;
            month -= 40;
        }
        else if (controlValueMonth > 20)
        {
            baseYear = 1800;
            month -= 20;
        }
        int year = baseYear + controlValueYear;

        try
        {
            var egnToDate = new DateTime(year, month, controlValueDate);
            return egnToDate <= eighteenYearsAgo;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    public static bool UserIdentifierNameIsValid(string input)
    {
        //Accepted ONLY Bulgarian letters, spaces, apostrophes, and dashes
        var acceptedSymbols = @"АаБбВвГгДдЕеЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЬьЮюЯя-' ".ToCharArray();

        return input.All(acceptedSymbols.Contains);
    }
}
