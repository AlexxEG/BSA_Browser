using System.Management.Automation;

namespace BSA_Browser_CLI.Filtering
{
    internal class FilterPredicateSimpleExclude : IFilterPredicate
    {
        WildcardPattern _pattern;

        public FilterPredicateSimpleExclude(string pattern)
        {
            _pattern = new WildcardPattern(
                $"*{WildcardPattern.Escape(pattern).Replace("`*", "*")}*",
                WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
        }

        public bool Match(string value)
        {
            // Return true if pattern DOESN'T match
            return _pattern.IsMatch(value) == false;
        }
    }
}
