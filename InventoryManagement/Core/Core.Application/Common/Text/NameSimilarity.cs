// Core.Application/Common/Text/NameSimilarity.cs
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Application.Common.Text
{
    public static class NameSimilarity
    {
        private static readonly Regex NonAlphaNum = new(@"[^a-z0-9\s]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MultiSpace  = new(@"\s+", RegexOptions.Compiled);

        public static string Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var lower  = s.ToLowerInvariant();
            var noPunc = NonAlphaNum.Replace(lower, " ");
            var spaced = MultiSpace.Replace(noPunc, " ").Trim();
            return spaced.Replace(" ", ""); // remove all spaces
        }

        // Jaro-Winkler ~[0..1], higher = more similar
        public static double JaroWinkler(string a, string b)
        {
            if (a == b) return 1d;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0d;

            int matchDistance = Math.Max(a.Length, b.Length) / 2 - 1;
            bool[] aMatch = new bool[a.Length];
            bool[] bMatch = new bool[b.Length];
            int matches = 0, transpositions = 0;

            for (int i = 0; i < a.Length; i++)
            {
                int start = Math.Max(0, i - matchDistance);
                int end   = Math.Min(i + matchDistance + 1, b.Length);
                for (int j = start; j < end; j++)
                {
                    if (bMatch[j] || a[i] != b[j]) continue;
                    aMatch[i] = bMatch[j] = true;
                    matches++;
                    break;
                }
            }
            if (matches == 0) return 0d;

            for (int i = 0, k = 0; i < a.Length; i++)
            {
                if (!aMatch[i]) continue;
                while (!bMatch[k]) k++;
                if (a[i] != b[k]) transpositions++;
                k++;
            }

            double jaro = (matches / (double)a.Length +
                           matches / (double)b.Length +
                           (matches - transpositions / 2.0) / matches) / 3.0;

            int prefix = 0;
            for (; prefix < Math.Min(4, Math.Min(a.Length, b.Length)); prefix++)
                if (a[prefix] != b[prefix]) break;

            return jaro + prefix * 0.1 * (1 - jaro);
        }
    }
}
