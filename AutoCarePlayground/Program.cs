using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Services;
using FuzzySharp;
using FuzzySharp.PreProcess;
using FuzzyString;
using System.Collections.ObjectModel;
using System.Diagnostics;

class Program
{
    static void Test1()
    {
        ObservableCollection<Item> Items = new ObservableCollection<Item>(TestData.GetItems());
        List<List<Item>> results = new List<List<Item>>();
        List<string> names = new List<string>();

        string[] targets = {
            "Item 1",
            "Item 2",
            "Item 3 Long description test for an inventory atom",
            "Item 4",
            "Item 5"
        };


        string source = "atom inventry Item 2";

        List<FuzzyStringComparisonOptions> options = new List<FuzzyStringComparisonOptions>
        {
            FuzzyStringComparisonOptions.UseJaccardDistance,
        };

        FuzzyStringComparisonTolerance tolerance = FuzzyStringComparisonTolerance.Weak;

        foreach (var t in targets)
        {
            bool result = source.ApproximatelyEquals(t, options, tolerance);
            Console.WriteLine("{0,-10} == {1,-10} >> {2,5}", source.Length > 10 ? source.Substring(0, 10) : source, t.Length > 10 ? t.Substring(0, 10) : t, result);
        }

        foreach (var t in targets)
        {
            Console.WriteLine();
            // Console.WriteLine("LevenshteinDistance: " + source.LevenshteinDistance(t));
            //Console.WriteLine("NormalizedLevenshteinDistance: " + source.NormalizedLevenshteinDistance(t));
            //Console.WriteLine("JaroDistance: " + source.JaroDistance(t));
            //Console.WriteLine("JaroWinklerDistance: " + source.JaroWinklerDistance(t));
            Console.WriteLine("JaccardDistance: " + source.JaccardDistance(t));
            Console.WriteLine("JaccardIndex: " + source.JaccardIndex(t));
            Console.WriteLine("LCSq:" + source.LongestCommonSubsequence(t));
            Console.WriteLine("LCSt:" + source.LongestCommonSubstring(t));
            Console.WriteLine("OverlapCoefficient:" + source.OverlapCoefficient(t));
            Console.WriteLine("RatcliffObershelpSimilarity:" + source.RatcliffObershelpSimilarity(t));
            Console.WriteLine("SorensenDiceDistance:" + source.SorensenDiceDistance(t));
            Console.WriteLine("TanimotoCoefficient:" + source.TanimotoCoefficient(t));
        }


        Console.WriteLine();


        // Description of Item 4, A long test description. This is a test long description. Hello is this large or long description.

        var searchText = "lang descition hallo Item 2 3";
        PreprocessMode m = PreprocessMode.Full;
        foreach (var item in Items)
        {
            var t = item.Name + " " + item.Description;
            Console.WriteLine("FR: " + Fuzz.PartialTokenAbbreviationRatio(searchText, t));
        }


        Console.WriteLine();
        results.Add(ItemSearcher.SearchItems(Items.ToList(), searchText));
        names.Add("ItemSearcher");


        var s6 = new LSearch();
        s6.AddItems(Items);
        results.Add(s6.SearchItems(searchText));
        results.Add(s6.FuzzySearch(searchText));
        results.Add(s6.ASearchItems(searchText));
        names.Add("LSearch");
        names.Add("LSearch");
        names.Add("LSearch");

        var s8 = new FuzzySearchService(Items.AsQueryable());
        results.Add(s8.FuzzySearch(searchText).ToList());
        names.Add("FuzzySearchService");


        for (int i = 0; i < results.Count; i++)
        {
            Console.WriteLine($"{names[i]} > {i + 1}");
            foreach (var item in results[i])
            {
                Console.WriteLine($"R: {item.Name} - {item.Description}");
            }
            Console.WriteLine("-----------------------------------------------------------------------");
        }
    }
    // The Main method is the entry point of the application
    static void Main(string[] args)
    {
        long avg = 0;
        for (int j = 0; j < 10; j++)
        {
            CategoryManager cm = new CategoryManager();
            Category? cati = null;
            Category? cat = null;
            for (int i = 0; i < 1000; i++)
            {
                cat = cm.AddCategory($"Categories {i}", cat);
                if (i == 2)
                {
                    cati = cat;
                }
            }


            long beforeMemory = GC.GetTotalMemory(true);
            cm.RemoveCategory(cati, true);
            long afterMemory = GC.GetTotalMemory(true);

            Console.Write($"B: {beforeMemory} b");
            Console.Write($", A: {afterMemory} b");
            Console.WriteLine($", F: {(beforeMemory - afterMemory)} b");
            avg += (beforeMemory - afterMemory);
        }
        avg /= 10;
        Console.WriteLine($"AVG Free: {avg} b - {avg/ 1024} Kb");

        avg = 0;
        for (int j = 0; j < 10; j++)
        {
            CategoryManager cm = new CategoryManager();
            Category? cati = null;
            Category? cat = null;
            for (int i = 0; i < 1000; i++)
            {
                cat = cm.AddCategory($"Categories {i}", cat);
                if (i == 2)
                {
                    cati = cat;
                }
            }


            long beforeMemory = GC.GetTotalMemory(true);
            cm.RemoveCategory(cati, false);
            long afterMemory = GC.GetTotalMemory(true);

            Console.Write($"B: {beforeMemory} b");
            Console.Write($", A: {afterMemory} b");
            Console.WriteLine($", F: {(beforeMemory - afterMemory)} b");
            avg += (beforeMemory - afterMemory);
        }
        avg /= 10;
        Console.WriteLine($"AVG Free: {avg} b - {avg / 1024} Kb");
    }
}