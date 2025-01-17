using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCare.Models
{
    public static class FuzzyMatch
    {
        public static int GetLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source)) return target?.Length ?? 0;
            if (string.IsNullOrEmpty(target)) return source?.Length ?? 0;

            int lenSource = source.Length;
            int lenTarget = target.Length;
            int[,] distance = new int[lenSource + 1, lenTarget + 1];

            for (int i = 0; i <= lenSource; distance[i, 0] = i++) ;
            for (int j = 0; j <= lenTarget; distance[0, j] = j++) ;

            for (int i = 1; i <= lenSource; i++)
            {
                for (int j = 1; j <= lenTarget; j++)
                {
                    int cost = source[i - 1] == target[j - 1] ? 0 : 1;

                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }
            return distance[lenSource, lenTarget];
        }

        // Fuzzy match using Levenshtein distance with a threshold
        public static bool IsFuzzyMatch(string source, string target, int maxDistance)
        {
            int distance = GetLevenshteinDistance(source, target);
            return distance <= maxDistance;
        }
    }

}
