using AutoCare.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.NGram;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Diagnostics;
using System.IO;

namespace AutoCare.Services
{
    public class EdgeNGramAnalyzer : Analyzer
    {
        private readonly int minGramSize;
        private readonly int maxGramSize;

        public EdgeNGramAnalyzer(int minGramSize = 4, int maxGramSize = 25)
        {
            this.minGramSize = minGramSize;
            this.maxGramSize = maxGramSize;
        }

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader textReader)
        {
            var tokenizer = new NGramTokenizer(LuceneVersion.LUCENE_48, textReader, minGramSize, maxGramSize);
            var tokenStream = new StandardFilter(LuceneVersion.LUCENE_48, tokenizer);
            var tokenStream2 = new LowerCaseFilter(LuceneVersion.LUCENE_48, tokenStream); // Optional, to make search case-insensitive
            return new TokenStreamComponents(tokenizer, tokenStream2);
        }
    }
    public class LSearch
    {
        // private readonly string _indexDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LuceneIndex");
        private LuceneVersion _version = LuceneVersion.LUCENE_48;
        private readonly Analyzer _analyzer;
        private readonly RAMDirectory _directory;
        private readonly IndexWriter _writer;

        public LSearch()
        {
            _analyzer = new StandardAnalyzer(_version);
            _directory = new RAMDirectory();
            var conf = new IndexWriterConfig(_version, _analyzer);
            _writer = new IndexWriter(_directory, conf);
        }

        public void AddItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                var doc = new Document();
                doc.Add(new StringField("Id", item.Id.ToString(), Field.Store.YES));
                doc.Add(new TextField("Name", item.Name, Field.Store.YES));
                doc.Add(new TextField("Description", item.Description, Field.Store.YES));
                doc.Add(new TextField("Labels", item.Labels, Field.Store.YES));
                doc.Add(new TextField("Tags", item.Tags, Field.Store.YES));
                doc.Add(new StringField("Location", item.Location, Field.Store.YES));
                doc.Add(new TextField("All", item.ConcatString(), Field.Store.YES));
                _writer.AddDocument(doc);
            }
            _writer.Commit();
        }


        public List<Item> SearchItems(string queryText)
        {
            if (string.IsNullOrEmpty(queryText)) return new List<Item>();
            var dReader = DirectoryReader.Open(_directory);
            var iSearcher = new IndexSearcher(dReader);

            // Parsing the query
            string[] fields = { "Name", "Description", "Tags", "Labels", "Location" };
            var qParser = new MultiFieldQueryParser(_version, fields, _analyzer);
            var query = qParser.Parse(queryText);

            // Searching the index
            var hits = iSearcher.Search(query, 10).ScoreDocs;

            var items = new List<Item>();
            foreach (var hit in hits)
            {
                var doc = iSearcher.Doc(hit.Doc);
                var item = new Item
                {
                    Id = int.Parse(doc.Get("Id")),
                    Name = doc.Get("Name"),
                    Description = doc.Get("Description"),
                    Labels = doc.Get("Labels"),
                    Tags = doc.Get("Tags"),
                    Location = doc.Get("Location")
                };
                items.Add(item);
            }

            return items;
        }

        public List<Item> ASearchItems(string queryText)
        {
            if (string.IsNullOrEmpty(queryText)) return new List<Item>();
            
            var directory = DirectoryReader.Open(_directory);
            var searcher = new IndexSearcher(directory);

            var fuzzyQuery1 = new FuzzyQuery(new Term("Name", queryText), 2); // max edit distance of 2
            var fuzzyQuery2 = new FuzzyQuery(new Term("Description", queryText), 2); // max edit distance of 2
            var fuzzyQuery3 = new FuzzyQuery(new Term("All", queryText), 2); // max edit distance of 2


            var bq = new BooleanQuery();
            bq.Add(fuzzyQuery1, Occur.SHOULD); // Add the fuzzy query
            bq.Add(fuzzyQuery2, Occur.SHOULD); // Add the fuzzy query
            bq.Add(fuzzyQuery3, Occur.SHOULD); // Add the fuzzy query


            // Searching the index
            var hits = searcher.Search(bq, 10).ScoreDocs;
            var items = new List<Item>();
            foreach (var hit in hits)
            {
                var doc = searcher.Doc(hit.Doc);
                var item = new Item
                {
                    Id = int.Parse(doc.Get("Id")),
                    Name = doc.Get("Name"),
                    Description = doc.Get("Description"),
                    Labels = doc.Get("Labels"),
                    Tags = doc.Get("Tags"),
                    Location = doc.Get("Location")
                };
                items.Add(item);
            }

            return items;
        }

        public List<Item> FuzzySearch(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery)) return new List<Item>();

            var dReader = DirectoryReader.Open(_directory);
            var iSearcher = new IndexSearcher(dReader);

            // Tokenize the search query
            var tokens = TokenizeSearchQuery(searchQuery);
            foreach (var tk in tokens)
            {
                Debug.WriteLine("Token: "+ tk);
            }

            // Define fuzziness (default is 2, adjust as needed)
            var fuzziness = 2;  // Allow up to 2 character differences
            var booleanQuery = new BooleanQuery();
            foreach (var token in tokens)
            {
                booleanQuery.Add(new FuzzyQuery(new Term("Name", token), fuzziness), Occur.SHOULD);
                booleanQuery.Add(new FuzzyQuery(new Term("Description", token), fuzziness), Occur.SHOULD);
            }

            // Search for documents matching the query
            var topDocs = iSearcher.Search(booleanQuery, 10); // Get top 10 results
            Console.WriteLine($"Found {topDocs.TotalHits} results:");

            // Display the results
            List<Item> items = new List<Item>();
            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var doc = iSearcher.Doc(scoreDoc.Doc);
                var item = new Item
                {
                    Id = int.Parse(doc.Get("Id")),
                    Name = doc.Get("Name"),
                    Description = doc.Get("Description"),
                    Labels = doc.Get("Labels"),
                    Tags = doc.Get("Tags"),
                    Location = doc.Get("Location")
                };
                items.Add(item);
            }
            dReader.Dispose();
            return items;
        }

        public List<string> TokenizeSearchQuery(string query)
        {
            var tokens = new List<string>();

            using (var reader = new System.IO.StringReader(query))
            {
                var tokenStream = _analyzer.GetTokenStream(null, reader);
                tokenStream.Reset();
                while (tokenStream.IncrementToken())
                {
                    var term = tokenStream.GetAttribute<ICharTermAttribute>();
                    tokens.Add(term.ToString());
                }
                tokenStream.End();
                tokenStream.Dispose();
            }

            return tokens;
        }
    }
}
