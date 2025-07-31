namespace eID.PDEAU.Service.Entities;

internal static class DBConstraints
{
    public static class ProviderDetails
    {
        public const int NameMaxLength = 512;
        public const int WebSiteUrlMaxLength = 128;
        public const int WorkingTimeStartMaxLength = 16; // 10:00:00 -> 8 symbols
        public const int WorkingTimeEndMaxLength = 16;
    }

    public static class ProviderService
    {
        public const int NameMaxLength = 1024;
        public const int DescriptionMaxLength = 4096;
        public const int DenialReasonMaxLength = 1024;
        public const int ReviewerFullNameMaxLength = 200;
    }

    public static class ProviderSection
    {
        public const int NameMaxLength = 128;
    }

    public static class ServiceScope
    {
        public const int NameMaxLength = Contracts.Constants.ServiceScopeNameMaxLength;
    }

    public static class DefaultServiceScope
    {
        /// <summary>
        /// We used the same length as <see cref="Contracts.Constants.ServiceScopeNameMaxLength"/> because the value will be copied in ServiceScope table
        /// </summary>
        public const int NameMaxLength = Contracts.Constants.ServiceScopeNameMaxLength;
    }


    public static class ProviderFile
    {
        /// <summary>
        /// We used the same length as <see cref="Contracts.Constants.ServiceScopeNameMaxLength"/> because the value will be copied in ServiceScope table
        /// </summary>
        public const int UploaderNameMaxLength = 200;
        public const int FileName = 2048;
        public const int FilePath = 2048;
    }

    public static class ProviderStatusHistory
    {
        public const int ModifierNameMaxLength = 256;
        public const int CommentMaxLength = 1024;
    }

    public const int IdentificationNumberMaxLength = 16;
    public const int UICMaxLength = 24;
    public const int UidMaxLength = 13;

    public const int HeadquartersLength = 200;
    public const int AddressLength = 200;
}
