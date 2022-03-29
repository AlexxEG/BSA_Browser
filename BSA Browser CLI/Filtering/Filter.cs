namespace BSA_Browser_CLI.Filtering
{
    internal struct Filter
    {
        public FilteringTypes Type { get; set; }
        public string Pattern { get; set; }

        public Filter(FilteringTypes type, string pattern)
        {
            Type = type;
            Pattern = pattern;
        }
    }
}
