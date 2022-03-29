using System.Management.Automation;

namespace BSA_Browser_CLI.Filtering
{
    internal class FilterPredicateSimple : IFilterPredicate
    {
        WildcardPattern _pattern;

        public FilterPredicateSimple(string pattern)
        {
            _pattern = new WildcardPattern(
                $"*{WildcardPattern.Escape(pattern).Replace("`*", "*")}*",
                WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
        }

        public bool Match(string value)
        {
            return _pattern.IsMatch(value);
        }
    }
}
