namespace eID.PJS.LocalLogsSearch.Tests;

    public enum ScaleType
    {
        BitBased,
        ByteBased
    }

    public static class ValueTypeExtensions
    {
        public static string NumberString(this decimal value)
        {
            if (value - Decimal.Truncate(value) > 0)
            {
                return string.Format("{0:0.00}", value);
            }
            else
            {
                return value.ToString("#");
            }
        }

        public static string NumberString(this double value)
        {
            if (value - Math.Truncate(value) > 0)
            {
                return string.Format("{0:0.00}", value);
            }
            else
            {
                return value.ToString("#");
            }
        }

        public static string ToFileSize(this int value)
        {
            return ToFileSize((long)value);
        }

        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        public static string ToFileSize(this long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EiB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PiB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TiB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GiB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MiB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KiB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = Math.Round((readable / 1024), 2);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

        public static KeyValuePair<string, decimal> ScaleWithUom(this decimal value)
        {
            const int scale = 1024;
            var orders = new string[] { "GB", "MB", "KB", "Bytes" };
            var max = (long)Math.Pow(scale, orders.Length - 1);
            foreach (string order in orders)
            {
                if (value > max)
                {
                    return new KeyValuePair<string, decimal>(order, decimal.Divide(value, max));
                }

                max /= scale;
            }
            return new KeyValuePair<string, decimal>("Bytes", 0);
        }

        public static KeyValuePair<string, decimal> ScaleWithUom(this decimal value, ScaleType scaleType)
        {
            int scale = 1024;
            var orders = new string[] { "GB", "MB", "KB", "Bytes" };
            if (scaleType == ScaleType.BitBased)
            {
                scale = 8192;
                orders = new string[] { "Gb", "Mb", "Kb", "bits" };
            }

            var max = (long)Math.Pow(scale, orders.Length - 1);
            foreach (string order in orders)
            {
                if (value > max)
                {
                    return new KeyValuePair<string, decimal>(order, decimal.Divide(value, max));
                }

                max /= scale;
            }
            return new KeyValuePair<string, decimal>("Bytes", 0);
        }

        public static KeyValuePair<string, decimal> ScaleWithUom(this int value)
        {
            return ScaleWithUom((decimal)value);
        }

        public static KeyValuePair<string, decimal> ScaleWithUom(this int value, ScaleType scaleType)
        {
            return ScaleWithUom((decimal)value,scaleType);
        }

        public static KeyValuePair<string, decimal> ScaleWithUom(this long value)
        {
            return ScaleWithUom((decimal)value);
        }

        public static KeyValuePair<string, decimal> ScaleWithUom(this long value, ScaleType scaleType)
        {
            return ScaleWithUom((decimal)value,scaleType);
        }

        public static string Render(this bool data, string textForTrue, string textForFalse)
        {
            if (data)
            {
                return textForTrue;
            }
            else
            {
                return textForFalse;
            }
        }

        public static string AsBase64StringString(this Guid g)
        {
            return System.Convert.ToBase64String(g.ToByteArray());
        }

    }

