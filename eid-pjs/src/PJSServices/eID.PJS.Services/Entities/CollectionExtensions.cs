using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;

namespace eID.PJS.Services
{
    #nullable disable
    public static class CollectionExtensions
    {

        public static string ToDelimitedText<T>(this IEnumerable<T> list, string separator)
        {
            var sb = new StringBuilder();

            var items = list as T[] ?? list.ToArray();
            if (list != null && items.Length > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    sb.Append(ObjectToString(items[i]));

                    if (i < items.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText<T>(this HashSet<T> list, string separator)
        {
            var sb = new StringBuilder();

            var items = list as T[] ?? list.ToArray();
            if (list != null && items.Length > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    sb.Append(ObjectToString(items[i]));

                    if (i < items.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IEnumerable<DateTime> list, string separator, string dateFormat = null)
        {
            var sb = new StringBuilder();


            var enumerable = list as DateTime[] ?? list.ToArray();
            if (list != null && enumerable.Length > 0)
            {
                for (int i = 0; i < enumerable.Length; i++)
                {
                    if (!string.IsNullOrEmpty(dateFormat))
                    {
                        sb.Append(enumerable[i].ToString(dateFormat));
                    }
                    else
                    {
                        sb.Append(enumerable[i].DateTimeToString());
                    }

                    if (i < enumerable.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IEnumerable<double> list, string separator)
        {
            var sb = new StringBuilder();

            var enumerable = list as double[] ?? list.ToArray();
            if (list != null && enumerable.Length > 0)
            {
                for (int i = 0; i < enumerable.Length; i++)
                {
                    sb.Append(enumerable[i]);

                    if (i < enumerable.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IEnumerable<decimal> list, string separator)
        {
            var sb = new StringBuilder();

            var enumerable = list as decimal[] ?? list.ToArray();
            if (list != null && enumerable.Length > 0)
            {
                for (int i = 0; i < enumerable.Length; i++)
                {
                    sb.Append(enumerable[i]);

                    if (i < enumerable.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IEnumerable<int> list, string separator)
        {
            var sb = new StringBuilder();

            var enumerable = list as int[] ?? list.ToArray();
            if (list != null && enumerable.Length > 0)
            {
                for (int i = 0; i < enumerable.Length; i++)
                {
                    sb.Append(enumerable[i]);

                    if (i < enumerable.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IEnumerable<string> list, string separator)
        {
            var sb = new StringBuilder();

            var enumerable = list as string[] ?? list.ToArray();
            if (list != null && enumerable.Length > 0)
            {
                for (int i = 0; i < enumerable.Length; i++)
                {
                    sb.Append(enumerable[i]);

                    if (i < enumerable.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IEnumerable<KeyValuePair<string, string>> items, string itemSeparator, string keyValueSeparator)
        {
            var sb = new StringBuilder();

            var keyValuePairs = items as KeyValuePair<string, string>[] ?? items.ToArray();
            if (items != null && keyValuePairs.Any())
            {
                for (int i = 0; i < keyValuePairs.Length; i++)
                {
                    sb.Append(ToDelimitedText(keyValuePairs[i], keyValueSeparator));

                    if (i < keyValuePairs.Length - 1)
                    {
                        sb.Append(itemSeparator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText<T>(this IEnumerable<KeyValuePair<string, T>> items, string itemSeparator, string keyValueSeparator)
        {
            var sb = new StringBuilder();

            var keyValuePairs = items as KeyValuePair<string, T>[] ?? items.ToArray();
            if (items != null && keyValuePairs.Any())
            {
                for (int i = 0; i < keyValuePairs.Length; i++)
                {
                    sb.Append(ToDelimitedText<T>(keyValuePairs[i], keyValueSeparator));

                    if (i < keyValuePairs.Length - 1)
                    {
                        sb.Append(itemSeparator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IEnumerable<KeyValuePair<string, object>> items, string itemSeparator, string keyValueSeparator)
        {
            var sb = new StringBuilder();

            var keyValuePairs = items as KeyValuePair<string, object>[] ?? items.ToArray();
            if (items != null && keyValuePairs.Any())
            {
                for (int i = 0; i < keyValuePairs.Length; i++)
                {
                    sb.Append(ToDelimitedText(keyValuePairs[i], keyValueSeparator));

                    if (i < keyValuePairs.Length - 1)
                    {
                        sb.Append(itemSeparator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this KeyValuePair<string, object> item, string separator)
        {
            return string.Format("{0}{1}{2}", item.Key, separator, ObjectToString(item.Value));
        }

        public static string ToDelimitedText(this KeyValuePair<string, string> item, string separator)
        {
            return string.Format("{0}{1}{2}", item.Key, separator, item.Value);
        }

        public static string ToDelimitedText<T>(this KeyValuePair<string, T> item, string separator)
        {
            return string.Format("{0}{1}{2}", item.Key, separator, ObjectToString(item.Value));
        }

        public static string ToDelimitedText(this IDictionary<string, string> dict, string itemSeparator, string keyValueSeparator)
        {
            var sb = new StringBuilder();

            if (dict != null && dict.Count > 0)
            {
                int i = 0;
                foreach (var item in dict)
                {
                    sb.Append(item.Key);
                    sb.Append(keyValueSeparator);
                    sb.Append(item.Value);

                    if (i < dict.Count - 1)
                    {
                        sb.Append(itemSeparator);
                    }

                    i++;
                }

                return sb.ToString();
            }

            return null;
        }

        public static string ToDelimitedText<T>(this IDictionary<string, T> dict, string itemSeparator, string keyValueSeparator)
        {
            var sb = new StringBuilder();

            if (dict != null && dict.Count > 0)
            {
                int i = 0;
                foreach (var item in dict)
                {
                    sb.Append(item.Key);
                    sb.Append(keyValueSeparator);
                    sb.Append(ObjectToString(item.Value));

                    if (i < dict.Count - 1)
                    {
                        sb.Append(itemSeparator);
                    }

                    i++;
                }

                return sb.ToString();
            }

            return null;
        }

        public static string ToDelimitedText(this IDictionary<string, object> dict, string itemSeparator, string keyValueSeparator)
        {
            var sb = new StringBuilder();

            if (dict != null && dict.Count > 0)
            {
                int i = 0;
                foreach (var item in dict)
                {
                    sb.Append(item.Key);
                    sb.Append(keyValueSeparator);
                    sb.Append(ObjectToString(item.Value));

                    if (i < dict.Count - 1)
                    {
                        sb.Append(itemSeparator);
                    }

                    i++;
                }

                return sb.ToString();
            }

            return null;
        }

        public static string ToDelimitedText(this Enum list, string separator)
        {
            var items = Enum.GetNames(list.GetType());

            return items.ToDelimitedText(separator);
        }

        public static string ToDelimitedText(this string[] list, string separator)
        {
            StringBuilder sb = new StringBuilder();

            if (list != null && list.Length > 0)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    sb.Append(list[i]);

                    if (i < list.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IList<object> list, string separator)
        {
            StringBuilder sb = new StringBuilder();

            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    sb.Append(ObjectToString(list[i]));

                    if (i < list.Count - 1)
                    {
                        sb.Append(separator);
                    }
                }
            }

            return sb.ToString();
        }

        public static string ToDelimitedText(this IDictionary items, string itemSeparator, string keyValueSeparator)
        {
            var sb = new StringBuilder();

            if (items != null && items.Count > 0)
            {
                int i = 1;
                foreach (var tmpItem in items)
                {
                    var item = (DictionaryEntry)tmpItem;

                    sb.Append(item.Key);
                    sb.Append(keyValueSeparator);
                    sb.Append(item.Value);

                    if (i < items.Count)
                    {
                        sb.Append(itemSeparator);
                    }

                    i++;
                }
            }

            return sb.ToString();
        }

        public static string ToJsonText(this IEnumerable<KeyValuePair<string, string>> items)
        {
            return ToDelimitedText(items, ",", ":");
        }

        public static string ToJsonPair(this KeyValuePair<string, string> item)
        {
            return ToDelimitedText(item, ":");
        }

        public static string ToEqualPair(this KeyValuePair<string, string> item)
        {
            return ToDelimitedText(item, "=");
        }

        public static string ToDelimitedText(this NameValueCollection dict, string itemSeparator, string keyValueSeparator)
        {
            var sb = new StringBuilder();

            if (dict != null && dict.Count > 0)
            {
                int i = 0;
                foreach (string item in dict.Keys)
                {
                    sb.Append(item);
                    sb.Append(keyValueSeparator);
                    sb.Append(dict[item]);

                    if (i < dict.Count - 1)
                    {
                        sb.Append(itemSeparator);
                    }

                    i++;
                }

                return sb.ToString();
            }

            return null;
        }

        /// <summary>
        /// Serialize DateTime value in format yyyy-MM-ddTHH:mm:ss.lll 
        /// </summary>
        /// <param name="value">DateTime value</param>
        /// <returns>string in format yyyy-MM-ddTHH:mm:ss.lll</returns>
        public static string DateTimeToString(this DateTime value)
        {
            string tz = value.Kind == DateTimeKind.Utc ? "Z" : "";


            return string.Format("{0}-{1}-{2}T{3}:{4}:{5}.{6}{7}",
                                 value.Year.ToString("0000"),
                                 value.Month.ToString("00"),
                                 value.Day.ToString("00"),
                                 value.Hour.ToString("00"),
                                 value.Minute.ToString("00"),
                                 value.Second.ToString("00"),
                                 value.Millisecond.ToString("000"), tz);


        }

        public static string ObjectToString(object value, CultureInfo culture = null)
        {
            if (culture == null)
                culture = CultureInfo.InvariantCulture;

            if (value is DateTime && culture == CultureInfo.InvariantCulture)
            {
                return ((DateTime)value).DateTimeToString();
            }

            if (value is DateTime)
            {
                return ((DateTime)value).ToString(culture.DateTimeFormat);
            }


            var type = value as Type;
            if (type != null)
            {
                Type t = type;
                return t.AssemblyQualifiedName;
            }

            return string.Format(culture, "{0}", value);

        }

        public static SortedList<TKey, TValue> ToSortedList<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector) where TKey : notnull
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var sortedList = new SortedList<TKey, TValue>();

            foreach (var item in source)
            {
                var key = keySelector(item);
                sortedList[key] = item;
            }

            return sortedList;
        }


        public static List<(TValue Value, ListLocation Location)> CompareByPositionWith<TKey, TValue>(this SortedList<TKey, TValue> source, SortedList<TKey, TValue> other) where TKey : notnull
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var differences = new List<(TValue, ListLocation)>();

            int sourceIndex = 0;
            int otherIndex = 0;

            while (sourceIndex < source.Count || otherIndex < other.Count)
            {
                if (sourceIndex < source.Count && otherIndex < other.Count)
                {
                    var sourceKey = source.Keys[sourceIndex];
                    var otherKey = other.Keys[otherIndex];

                    int keyComparison = Comparer<TKey>.Default.Compare(sourceKey, otherKey);

                    if (keyComparison == 0)
                    {
                        // Keys match, check values
                        var sourceValue = source.Values[sourceIndex];
                        var otherValue = other.Values[otherIndex];

                        if (!EqualityComparer<TValue>.Default.Equals(sourceValue, otherValue))
                        {
                            differences.Add((sourceValue, ListLocation.Both));
                        }

                        sourceIndex++;
                        otherIndex++;
                    }
                    else if (keyComparison < 0)
                    {
                        differences.Add((source.Values[sourceIndex], ListLocation.Left));
                        sourceIndex++;
                    }
                    else
                    {
                        differences.Add((other.Values[otherIndex], ListLocation.Right));
                        otherIndex++;
                    }
                }
                else if (sourceIndex < source.Count)
                {
                    differences.Add((source.Values[sourceIndex], ListLocation.Left));
                    sourceIndex++;
                }
                else
                {
                    differences.Add((other.Values[otherIndex], ListLocation.Right));
                    otherIndex++;
                }
            }

            return differences;
        }

        public static List<(TValue Value, ListLocation Location)> CompareByPositionWith<TKey, TValue>(this SortedList<TKey, TValue> source, SortedList<TKey, TValue> other, Func<TValue, TKey> keySelector) where TKey : notnull
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var differences = new List<(TValue, ListLocation)>();

            int sourceIndex = 0;
            int otherIndex = 0;

            while (sourceIndex < source.Count || otherIndex < other.Count)
            {
                if (sourceIndex < source.Count && otherIndex < other.Count)
                {
                    var sourceKey = keySelector(source.Values[sourceIndex]);
                    var otherKey = keySelector(other.Values[otherIndex]);

                    int keyComparison = Comparer<TKey>.Default.Compare(sourceKey, otherKey);

                    if (keyComparison == 0)
                    {
                        // Keys match, check values
                        var sourceValue = source.Values[sourceIndex];
                        var otherValue = other.Values[otherIndex];

                        if (!EqualityComparer<TValue>.Default.Equals(sourceValue, otherValue))
                        {
                            differences.Add((sourceValue, ListLocation.Both));
                        }

                        sourceIndex++;
                        otherIndex++;
                    }
                    else if (keyComparison < 0)
                    {
                        differences.Add((source.Values[sourceIndex], ListLocation.Left));
                        sourceIndex++;
                    }
                    else
                    {
                        differences.Add((other.Values[otherIndex], ListLocation.Right));
                        otherIndex++;
                    }
                }
                else if (sourceIndex < source.Count)
                {
                    differences.Add((source.Values[sourceIndex], ListLocation.Left));
                    sourceIndex++;
                }
                else
                {
                    differences.Add((other.Values[otherIndex], ListLocation.Right));
                    otherIndex++;
                }
            }

            return differences;
        }


        public static List<(TValue Value, ListLocation Location)> CompareTo<TKey, TValue>(this SortedList<TKey, TValue> left, SortedList<TKey, TValue> right, Func<TValue, TKey> keySelector) where TKey : notnull
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var leftDifferences = Compare(left, right, keySelector, ListLocation.Left);
            var rightDifferences = Compare(right, left, keySelector, ListLocation.Right);

            return leftDifferences.Concat(rightDifferences).ToList();
        }

        private static List<(TValue Value, ListLocation Location)> Compare<TKey, TValue>( SortedList<TKey, TValue> left, SortedList<TKey, TValue> right, Func<TValue, TKey> keySelector, ListLocation location) where TKey : notnull
        {
            var differences = new List<(TValue, ListLocation)>();

            foreach (var kvp in left)
            {
                var key = keySelector(kvp.Value);

                if (!right.ContainsKey(key))
                {
                    differences.Add((kvp.Value, location));
                }
            }

            return differences;
        }
        public enum ListLocation
        {
            Left,
            Right,
            Both
        }
    }
}
