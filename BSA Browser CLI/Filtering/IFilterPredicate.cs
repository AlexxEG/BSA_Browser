namespace BSA_Browser_CLI.Filtering
{
    internal interface IFilterPredicate
    {
        bool Match(string value);
    }
}
