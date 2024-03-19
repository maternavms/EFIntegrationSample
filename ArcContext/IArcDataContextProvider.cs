namespace TestEfMultipleSqlVersions.ArcContext
{
    public interface IArcDataContextProvider
    {
        IArcAgentDbContext GetContext(string instanceName);
    }
}