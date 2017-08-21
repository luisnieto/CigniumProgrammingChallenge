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
        private const string seGoogle = "Google";
        private const string seBing = "MSN Search";

        static void Main(string[] args)
        {
            var mostTermSearch = string.Empty;

            // Create the basic struture data
            var rankingData = new RankingSearchsBE(seGoogle, seBing);

            // Get the dictory to store the data
            rankingData.InitializeTerm(args.ToList());

            // Get the information
            foreach (var item in args)
            {
                var termLine = Console.ReadLine();
                rankingData.AddDataSearch(termLine);
            }

            // Print the results
            foreach (var engien in rankingData.Engiens)
            {
                var termsWinner = engien.Terms.Where(x => x.Winner).Select(x => x.Description);
                var termsDescription = termsWinner.Any()
                    ? string.Join(", ", termsWinner)
                    : "none";

                Console.WriteLine(string.Format("{0} winner: {1}", engien.Description, termsDescription));
            }

            Console.WriteLine(string.Format("Total winner: {0}", rankingData.GetMostSearchTerm()));
        }
    }

    public class RankingSearchsBE
    {
        // constants
        private const string SEARCH_PATTERN_ENGIENS = @"[a-zA-Z ]+: \d+";
        private const char CHARACTER_SPLIT_ELEMENTS = ':';

        // constructor
        public RankingSearchsBE(params string[] searchEngiens)
        {
            this.Engiens = new List<EngienBE>();

            foreach (var searchEngien in searchEngiens)
            {
                Engiens.Add(new EngienBE(searchEngien));
            }
        }

        // properties
        public List<EngienBE> Engiens { get; }

        // methods
        public void InitializeTerm(List<string> terms)
        {
            foreach (var engien in Engiens)
            {
                engien.Terms.Clear();

                foreach (var term in terms)
                {
                    engien.Terms.Add(new TermBE() { Description = term });
                }
            }
        }

        public void AddDataSearch(string input)
        {
            var termDescripcion = GetProcessTerm(input);
            var engienSections = Regex.Matches(input, SEARCH_PATTERN_ENGIENS);

            foreach (var engienData in engienSections)
            {
                var elements = engienData.ToString().Split(CHARACTER_SPLIT_ELEMENTS);

                if (elements.Count() >= 2)
                {
                    var engienDescription = elements[0].Trim();
                    var searchs = Convert.ToInt64(elements[1].Trim());

                    var engien = this.Engiens.FirstOrDefault(x => x.Description.Equals(engienDescription, StringComparison.OrdinalIgnoreCase));

                    if (engien != null)
                    {
                        var term = engien.Terms.FirstOrDefault(x => x.Description.Equals(termDescripcion, StringComparison.OrdinalIgnoreCase));

                        if (term != null)
                        {
                            term.AmountSearch = searchs;
                        }
                    }
                }
            }

            CalculateTermWinner(termDescripcion);
        }

        public string GetMostSearchTerm()
        {
            var termDescription = string.Empty;

            if (!this.Engiens.Any())
            {
                return termDescription;
            }

            var terms = this.Engiens.First().Terms.Select(x => new TermBE() { Description = x.Description }).ToList();

            foreach (var engien in this.Engiens)
            {
                foreach (var term in engien.Terms)
                {
                    terms.First(x => x.Description.Equals(term.Description)).AmountSearch += term.AmountSearch;
                }
            }

            var maxAmountSearch = terms.Max(x => x.AmountSearch);
            termDescription = terms.First(x => x.AmountSearch == maxAmountSearch).Description;

            return termDescription;
        }

        // private methods
        private void CalculateTermWinner(string termDescription)
        {
            if (!this.Engiens.Any())
            {
                return;
            }

            var maxAmountSearch = 0L;
            var descriptionEngieWinner = string.Empty;

            foreach (var engien in this.Engiens)
            {
                var term = engien.Terms.FirstOrDefault(x => x.Description.Equals(termDescription, StringComparison.OrdinalIgnoreCase));

                if (term != null && term.AmountSearch > maxAmountSearch)
                {
                    maxAmountSearch = term.AmountSearch;
                    descriptionEngieWinner = engien.Description;
                }
            }

            if (!string.IsNullOrEmpty(descriptionEngieWinner))
            {
                this.Engiens.First(x => x.Description.Equals(descriptionEngieWinner)).Terms.First(x => x.Description.Equals(termDescription, StringComparison.OrdinalIgnoreCase)).Winner = true;
            }
        }

        private string GetProcessTerm(string input)
        {
            return input.Split(CHARACTER_SPLIT_ELEMENTS)[0];
        }
    }

    public class EngienBE
    {
        public EngienBE(string description)
        {
            this.Description = description;
            this.Terms = new List<TermBE>();
        }

        public string Description { get; }

        public List<TermBE> Terms { get; set; }
    }

    public class TermBE
    {
        public string Description { get; set; }

        public long AmountSearch { get; set; }

        public bool Winner { get; set; }
    }
}

