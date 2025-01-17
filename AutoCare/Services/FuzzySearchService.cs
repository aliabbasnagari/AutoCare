using AutoCare.Models;

namespace AutoCare.Services
{
    public class FuzzySearchService
    {
        private readonly IQueryable<Item> _items;

        public FuzzySearchService(IQueryable<Item> items)
        {
            _items = items;
        }

        public IEnumerable<Item> FuzzySearch(string searchTerm, double similarityThreshold = 0.8)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Item>();

            // Convert to lowercase for case-insensitive comparison
            searchTerm = searchTerm.ToLower().Trim();

            // Split search term into individual words
            var searchWords = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Get all items and perform fuzzy matching
            return _items.AsEnumerable()
                .Select(item => new
                {
                    Item = item,
                    Score = CalculateSearchRelevanceScore(item, searchWords)
                })
                .Where(x => x.Score >= similarityThreshold)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Item);
        }

        private double CalculateSearchRelevanceScore(Item item, string[] searchWords)
        {
            var nameScore = GetBestMatchScore(item.Name.ToLower(), searchWords);
            var descriptionScore = item.Description != null
                ? GetBestMatchScore(item.Description.ToLower(), searchWords) * 0.9 // Description matches are weighted less
                : 0;

            // Check tags and labels if they exist
            var tagScore = !string.IsNullOrEmpty(item.Tags)
                ? GetBestMatchScore(item.Tags.ToLower(), searchWords) * 0.7
                : 0;

            var labelScore = !string.IsNullOrEmpty(item.Labels)
                ? GetBestMatchScore(item.Labels.ToLower(), searchWords) * 0.8
                : 0;

            // Return the best score among all fields

            Console.WriteLine("||->> " + item.Name + " - " + (nameScore + descriptionScore + tagScore + labelScore));
            return (nameScore + descriptionScore + tagScore + labelScore);
            //return Math.Max(Math.Max(nameScore, descriptionScore), Math.Max(tagScore, labelScore));
        }

        private double GetBestMatchScore(string text, string[] searchWords)
        {
            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            double bestScore = 0;
            int iters = 1;
            foreach (var searchWord in searchWords)
            {
                foreach (var word in words)
                {
                    bestScore += CalculateSimilarity(word, searchWord);
                    iters++;
                }
            }

            return bestScore;
        }

        private double CalculateSimilarity(string source, string target)
        {
            // Handle exact matches immediately
            if (source == target) return 1.0;

            // Calculate Levenshtein distance
            var distance = ComputeLevenshteinDistance(source, target);

            // Convert distance to similarity score
            var maxLength = Math.Max(source.Length, target.Length);
            var similarity = 1.0 - ((double)distance / maxLength);

            return similarity;
        }

        private int ComputeLevenshteinDistance(string source, string target)
        {
            var sourceLength = source.Length;
            var targetLength = target.Length;
            var matrix = new int[sourceLength + 1, targetLength + 1];

            // Initialize first row and column
            for (var i = 0; i <= sourceLength; i++)
                matrix[i, 0] = i;
            for (var j = 0; j <= targetLength; j++)
                matrix[0, j] = j;

            // Fill in the rest of the matrix
            for (var i = 1; i <= sourceLength; i++)
            {
                for (var j = 1; j <= targetLength; j++)
                {
                    var cost = (source[i - 1] == target[j - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }

            return matrix[sourceLength, targetLength];
        }
    }

}