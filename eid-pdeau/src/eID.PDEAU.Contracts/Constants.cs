namespace eID.PDEAU.Contracts;

public static class Constants
{
    public const string Payload = "Payload";

    public const int ServiceScopeNameMaxLength = 1024;
    public static class ProviderStatusHistory
    {
        public const int CommentMaxLength = 1024;
    }

    public static class ProviderOffices
    {
        public const int NameMaxLength = 100;
        public const int AddressMaxLength = 255;
    }

    public static class Providers
    {
        public const int GeneralInformationMaxLength = 500;
    }

    public static class HeaderNames
    {
        /// <summary>Gets the <c>Request-Id</c> HTTP header name.</summary>
        public const string RequestId = "Request-Id";
    }

    public static class ProviderService
    {
        public const int DenialReasonMaxLength = 1024;
    }

    public static class AdministratorAction
    {
        public const int AdministratorUidMaxLength = 13;
        public const int AdministratorFullNameMaxLength = 256;
        public const int CommentMaxLength = 1024;
    }
}
