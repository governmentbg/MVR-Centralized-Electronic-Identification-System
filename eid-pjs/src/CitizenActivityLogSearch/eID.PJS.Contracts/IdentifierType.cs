namespace eID.PJS.Contracts;

public enum IdentifierType
{
    /// <summary>
    /// Default value
    /// </summary>
    NotSpecified = 0,

    /// <summary>
    /// ЛНЧ (личен номер на чужденец)
    /// </summary>
    LNCh = 1,

    /// <summary>
    /// ЕГН (единен граждански номер)
    /// </summary>
    EGN = 2,
}
