using eID.PIVR.Contracts.Enums;

namespace eID.PIVR.Contracts.Results;

public interface DateOfProhibitionResult
{
    DateTime? Date { get; }
    /// <summary>
    /// Prohibition type: Full or Partial
    /// </summary>
    ProhibitionType TypeOfProhibition { get; }
    string DescriptionOfProhibition { get; }
}
