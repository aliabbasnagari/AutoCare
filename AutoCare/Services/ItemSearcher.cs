using AutoCare.Models;

namespace AutoCare.Services
{
    public class ItemSearcher
    {
        public static List<Item> SearchItems(List<Item> items, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new List<Item>();

            searchTerm = searchTerm.Trim().ToLower();
            var scoredItems = new List<(Item item, double score)>();

            foreach (var item in items)
            {
                double score = 0;
                score += KmpMatch(item.Name.ToLower(), searchTerm) * 10;
                score += KmpMatch(item.Description?.ToLower() ?? string.Empty, searchTerm) * 10;

                score += LcsMatch(item.Name.ToLower(), searchTerm);
                score += LcsMatch(item.Description?.ToLower() ?? string.Empty, searchTerm);

                score += LevenshteinScore(item.Name.ToLower(), searchTerm);
                score += LevenshteinScore(item.Description?.ToLower() ?? string.Empty, searchTerm);

                scoredItems.Add((item, score));
            }

            return scoredItems.OrderByDescending(x => x.score).Select(x => x.item).ToList();
        }

        private static double KmpMatch(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return 0;
            int[] lps = BuildKmpTable(pattern);
            int i = 0, j = 0;

            while (i < text.Length)
            {
                if (pattern[j] == text[i])
                {
                    i++;
                    j++;
                    if (j == pattern.Length) return 1;
                }
                else if (j > 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
            return 0;
        }

        private static int[] BuildKmpTable(string pattern)
        {
            int[] lps = new int[pattern.Length];
            int length = 0;
            int i = 1;

            while (i < pattern.Length)
            {
                if (pattern[i] == pattern[length])
                {
                    length++;
                    lps[i++] = length;
                }
                else if (length != 0)
                {
                    length = lps[length - 1];
                }
                else
                {
                    lps[i++] = 0;
                }
            }
            return lps;
        }

        private static double LcsMatch(string text, string pattern)
        {
            int[,] dp = new int[text.Length + 1, pattern.Length + 1];

            for (int i = 1; i <= text.Length; i++)
            {
                for (int j = 1; j <= pattern.Length; j++)
                {
                    dp[i, j] = text[i - 1] == pattern[j - 1]
                        ? dp[i - 1, j - 1] + 1
                        : Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
            return dp[text.Length, pattern.Length];
        }

        private static double LevenshteinScore(string text, string pattern)
        {
            int[,] dp = new int[text.Length + 1, pattern.Length + 1];

            for (int i = 0; i <= text.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= pattern.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= text.Length; i++)
            {
                for (int j = 1; j <= pattern.Length; j++)
                {
                    int cost = text[i - 1] == pattern[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }
            return 1.0 / (1 + dp[text.Length, pattern.Length]);
        }
    }

    /*
    public class ItemSearcher
    {
        public static List<Item> SearchItems(List<Item> items, string searchTerm)
        {
            searchTerm = searchTerm.Trim().ToLower();
            Debug.WriteLine("|---------------------------|");
            var scoredItems = new List<(Item, double)>();

            foreach (var item in items)
            {
                Debug.WriteLine(FuzzyMatch.GetLevenshteinDistance(searchTerm, item.Name));
                Debug.WriteLine(FuzzyMatch.GetLevenshteinDistance(searchTerm, item.Description));

                Debug.WriteLine(searchTerm.LongestCommonSubsequence(item.Name));
                Debug.WriteLine(searchTerm.LongestCommonSubsequence(item.Description));

                Debug.WriteLine(searchTerm.LevenshteinDistance(item.Name));
                Debug.WriteLine(searchTerm.LevenshteinDistance(item.Description));

                Debug.WriteLine(searchTerm.DiceCoefficient(item.Name));
                Debug.WriteLine(searchTerm.DiceCoefficient(item.Description));

                Debug.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~");

                double score = 0;

                double psc = 0;
                // Score based on KMP (exact match)
                psc = KmpMatch(item.Name.ToLower(), searchTerm);
                Debug.WriteLine($"KMP Name: {psc}");
                score += psc;

                psc = KmpMatch(item.Description.ToLower(), searchTerm);
                Debug.WriteLine($"KMP Des: {psc}");
                score += psc;

                // Score based on LCS (partial match)
                psc = LcsMatch(item.Name.ToLower(), searchTerm);
                Debug.WriteLine($"LCS Name: {psc}");
                score += psc;

                psc = LcsMatch(item.Description.ToLower(), searchTerm);
                Debug.WriteLine($"LCS Des: {psc}");
                score += psc;

                // Score based on Levenshtein distance (approximate match)
                psc = LevenshteinDistance(item.Name.ToLower(), searchTerm);
                Debug.WriteLine($"LD Name: {psc}");
                score += psc;

                psc = LevenshteinDistance(item.Description.ToLower(), searchTerm);
                Debug.WriteLine($"LD Des: {psc}");
                score += psc;

                //score = Sigmoid(score);
                Debug.WriteLine(item.Name + " <-----> " + score);
                scoredItems.Add((item, score));
            }

            // Sort items based on the score in descending order
            return scoredItems.OrderByDescending(x => x.Item2).Select(x => x.Item1).ToList();
        }

        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        // KMP String Matching Algorithm (returns 1 if matched, 0 if not)
        private static double KmpMatch(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return 0;
            int[] lps = BuildKmpTable(pattern);
            int i = 0, j = 0;
            while (i < text.Length)
            {
                if (pattern[j] == text[i])
                {
                    j++;
                    i++;
                    if (j == pattern.Length)
                    {
                        return 10; // Exact match score
                    }
                }
                else if (j > 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
            return 0;
        }

        // Build KMP Table
        private static int[] BuildKmpTable(string pattern)
        {
            int[] lps = new int[pattern.Length];
            int j = 0;
            for (int i = 1; i < pattern.Length; i++)
            {
                while (j > 0 && pattern[i] != pattern[j])
                {
                    j = lps[j - 1];
                }

                if (pattern[i] == pattern[j])
                {
                    j++;
                    lps[i] = j;
                }
                else
                {
                    lps[i] = 0;
                }
            }
            return lps;
        }

        // Longest Common Subsequence (LCS) Match
        private static double LcsMatch(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return 0;

            int m = text.Length;
            int n = pattern.Length;
            int[,] dp = new int[m + 1, n + 1];

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (text[i - 1] == pattern[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            return dp[m, n];
        }

        // Levenshtein Distance Match
        private static double LevenshteinDistance(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return 0;

            int m = text.Length;
            int n = pattern.Length;
            int[,] dp = new int[m + 1, n + 1];

            for (int i = 0; i <= m; i++) dp[i, 0] = i;
            for (int j = 0; j <= n; j++) dp[0, j] = j;

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    int cost = (text[i - 1] == pattern[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }

            return 1.0 / (1 + dp[m, n]);
        }
    }*/
}


