#nullable disable

namespace eID.PJS.Services.Signing
{
    public class ChecksumData
    {
        public string Checksum { get; set; }
        public string[] EventIds { get; set; }
        public string SystemId { get; set; }
    }
}
