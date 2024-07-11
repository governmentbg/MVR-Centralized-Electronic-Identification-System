using Quartz;

namespace eID.POD.Service.Validators;

public static class ValidatorHelpers
{
    public static bool IsValidCronExpression(string cronExpression)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
        {
            return false;
        }

        return CronExpression.IsValidExpression(cronExpression);
    }

    public static bool IsValidAbsoluteUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }
        // https://github.com/dotnet/runtime/issues/22718
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
        {
            return false;
        }
        return uri.IsAbsoluteUri;
    }
}
