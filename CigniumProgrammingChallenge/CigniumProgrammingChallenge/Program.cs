using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CigniumProgrammingChallenge
{
    class Program
    {
        private const string SEARCH_PATTERN_ENGIENS = @"[a-zA-Z ]+: \d+";
        private const char CHARACTER_SPLIT_ELEMENTS = ':';

        static void Main(string[] args)
        {
            var mostTermSearch = string.Empty;
            var mostCountSearch = 0L;

            // Get the dictory to store the data
            var data = GetDictionary(args);

            // Get the information
            foreach (var item in args)
            {
                var termLine = Console.ReadLine();

                // Get word to process
                var term = GetProcessTerm(termLine);

                // Get engiens to process
                var searchElement = GetHigherSearchEngien(termLine);
                data[term] = searchElement.Item1;

                // Determinate the higher search element
                if (mostCountSearch < searchElement.Item2)
                {
                    mostTermSearch = term;
                    mostCountSearch = searchElement.Item2;
                }
            }

            // Print the results
            foreach (var key in data.Keys)
            {
                Console.WriteLine(string.Format("{0} winner: {1}", data[key].Description, key));
            }

            Console.WriteLine(string.Format("Total winner: {0}", mostTermSearch));
        }

        public static Dictionary<string, EngienBE> GetDictionary(IList<string> wordsSearch)
        {
            var response = new Dictionary<string, EngienBE>();

            foreach (var item in wordsSearch)
            {
                response.Add(item, new EngienBE());
            }

            return response;
        }

        public static string GetProcessTerm(string input)
        {
            return input.Split(CHARACTER_SPLIT_ELEMENTS)[0];
        }

        public static Tuple<EngienBE, long> GetHigherSearchEngien(string input)
        {
            var engiens = Regex.Matches(input, SEARCH_PATTERN_ENGIENS);
            var engienBE = new EngienBE();
            var searchSum = 0L;

            foreach (var engien in engiens)
            {
                var elements = engien.ToString().Split(CHARACTER_SPLIT_ELEMENTS);

                if (elements.Count() >= 2)
                {
                    var searchs = Convert.ToInt64(elements[1].Trim());
                    searchSum += searchs;

                    if (searchs > engienBE.CountSearchs)
                    {
                        engienBE.Description = elements[0].Trim();
                        engienBE.CountSearchs = searchs;
                    }
                }
            }

            return Tuple.Create(engienBE, searchSum);
        }
    }

    public class EngienBE
    {
        public string Description { get; set; }

        public long CountSearchs { get; set; }
    }
}

