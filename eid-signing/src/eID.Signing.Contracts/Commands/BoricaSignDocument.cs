using eID.Signing.Contracts.Enums;
using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface BoricaARSSignDocument : CorrelatedBy<Guid>
{
    public IEnumerable<BoricaContentToSign> Contents { get; set; }
}
public interface BoricaSignDocument : CorrelatedBy<Guid>
{
    public string Uid { get; set; }
    public IEnumerable<BoricaContentToSign> Contents { get; set; }
}
public class BoricaContentToSign
{
    public string ConfirmText { get; set; } = "Confirm sign";

    public string ContentFormat { get; set; } = "BINARY_BASE64";
    /// <summary>
    /// Not needed for ARSSigning
    /// </summary>
    public string MediaType { get; set; }

    /// <summary>
    /// Hash of the document content
    /// </summary>
    public string Data { get; set; }
    /// <summary>
    /// File name with extension
    /// </summary>
    public string FileName { get; set; }

    public string PadesVisualSignature { get; set; } = "true";

    public BoricaSignaturePosition SignaturePosition { get; set; }
    public SignatureType SignatureType { get; set; }
}

public class BoricaSignaturePosition
{
    public int ImageHeight { get; set; }

    public int ImageWidth { get; set; }

    public int ImageXAxis { get; set; }

    public int ImageYAxis { get; set; }

    public int PageNumber { get; set; }
}
