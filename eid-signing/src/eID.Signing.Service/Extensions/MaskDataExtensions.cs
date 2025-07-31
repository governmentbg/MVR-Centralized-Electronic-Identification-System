namespace eID.Signing.Service.Extensions;

public static class MaskDataExtensions
{
    /// <summary>
    /// Mask email
    /// </summary>
    /// <param name="email">Email to be masked</param>
    /// <returns>Examples aaa@bbb.ccc to be masked as: a*a@bbb.ccc, Hello-World@dsd.aaa -> H******orld@dsd.aaa</returns>
    public static string MaskEmail(this string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return email;
        }

        var atIndex = email.IndexOf('@');
		var domain = email.AsSpan(atIndex);
        if (atIndex == 2)
        {
			return string.Concat(email[0].ToString(), email[1..atIndex].Mask(100), domain);
        }
		if (atIndex < 2)
		{
			return string.Concat(email[..atIndex].Mask(100), domain);
		}

        var partToMask = email[1..atIndex];
        return string.Concat(email[0].ToString(), partToMask.Mask(), domain);
    }

    /// <summary>
    /// Mask part of <paramref name="s"/> with *
    /// </summary>
    /// <param name="s">Text to be masked</param>
    /// <param name="partToMaskInPercent">Percent of text to be masked</param>
    /// <returns></returns>
    public static string Mask(this string s, double partToMaskInPercent = 60)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        var chars = s.ToCharArray();
        // Get 60 % of length
        var lentgth = (int)(chars.Length * (partToMaskInPercent / 100));
        // Fill first 60 % with mask char
        for (var i = 0; i < lentgth; i++)
        {
            chars[i] = '*';
        }

        return new string(chars);
    }
}
