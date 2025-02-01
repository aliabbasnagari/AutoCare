using System.Text;

namespace AutoCare.Models
{
    public static class StringExtensions
    {
        // Levenshtein Distance
        public static int Levenshtein(this string str1, string str2)
        {
            int lenStr1 = str1.Length;
            int lenStr2 = str2.Length;

            int[,] matrix = new int[lenStr1 + 1, lenStr2 + 1];

            for (int i = 0; i <= lenStr1; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= lenStr2; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= lenStr1; i++)
            {
                for (int j = 1; j <= lenStr2; j++)
                {
                    int cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;

                    matrix[i, j] = new[] {
                    matrix[i - 1, j] + 1,    // Deletion
                    matrix[i, j - 1] + 1,    // Insertion
                    matrix[i - 1, j - 1] + cost // Substitution
                }.Min();
                }
            }

            return matrix[lenStr1, lenStr2];
        }

        // Longest Common Subsequence (LCS)
        public static string LCS(this string str1, string str2)
        {
            int m = str1.Length;
            int n = str2.Length;

            int[,] dp = new int[m + 1, n + 1];

            // Build the LCS table
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            // Reconstruct the LCS string
            StringBuilder lcs = new StringBuilder();
            int x = m, y = n;
            while (x > 0 && y > 0)
            {
                if (str1[x - 1] == str2[y - 1])
                {
                    lcs.Insert(0, str1[x - 1]);
                    x--;
                    y--;
                }
                else if (dp[x - 1, y] > dp[x, y - 1])
                {
                    x--;
                }
                else
                {
                    y--;
                }
            }

            return lcs.ToString();
        }
    }

}
