using System.Text.RegularExpressions;

namespace BSA_Browser_CLI.Filtering
{
    internal class FilterPredicateRegex : IFilterPredicate
    {
        Regex _pattern;

        public FilterPredicateRegex(string pattern)
        {
            _pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public bool Match(string value)
        {
            return _pattern.IsMatch(value);
        }
    }
}
