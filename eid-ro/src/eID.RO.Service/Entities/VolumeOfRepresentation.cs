using System.Xml.Serialization;

namespace eID.RO.Service.Entities;

[XmlRoot("EmpowermentStatementItem")]
public class VolumeOfRepresentation : Contracts.Results.VolumeOfRepresentationResult
{
    public string Code { get; set; }
    public string Name { get; set; }
}

