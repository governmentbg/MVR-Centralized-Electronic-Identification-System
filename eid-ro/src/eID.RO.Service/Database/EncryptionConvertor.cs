using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eID.RO.Service.Database;

public class EncryptionConvertor : ValueConverter<string, string>
{
    public EncryptionConvertor(ConverterMappingHints mappingHints = null)
        : base(x => EncryptionHelper.Encrypt(x), x => EncryptionHelper.Decrypt(x), mappingHints)
    { }
}
