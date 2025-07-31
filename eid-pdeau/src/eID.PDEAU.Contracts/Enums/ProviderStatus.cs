namespace eID.PDEAU.Contracts.Enums;

/// <summary>
/// Provider request statuses.
/// The Work flow is:
///          / Active
/// InReview - Denied
///          \ ReturnedForCorrection
/// ReturnedForCorrection - InReview
///                       \ Denied
/// Active - Denied
/// </summary>
public enum ProviderStatus
{
    None = 0,
    InReview = 1,
    Active = 2,
    Denied = 3,
    ReturnedForCorrection = 4,
}
