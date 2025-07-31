using System.Globalization;

namespace eID.RO.Service.Responses
{
    public partial class ForeignIdentityInfoResponseType
    {
        public DateTime? BirthDateParsed
        {
            get
            {
                if (!DateTime.TryParse(BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
                {
                    return ParseDate(BirthDate);
                }
                return birthDate;
            }
        }
        static DateTime? ParseDate(string input)
        {
            string[] parts = input.Split('.');
            if (parts.Length != 3)
            {
                parts = input.Split('/');
                if (parts.Length != 3)
                {
                    return null;
                }
            }

            int day = parts[0] == "00" ? 1 : int.Parse(parts[0]);
            int month = parts[1] == "00" ? 1 : int.Parse(parts[1]);
            int year = int.Parse(parts[2]);
            return new DateTime(year, month, day);
        }
    }
}
