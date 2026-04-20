namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Common
{
    public interface IDatabaseMapper<TDomain, TEntity>
        where TEntity : class
    {
        TEntity ToEntity(TDomain domain);
        TDomain? ToDomain(TEntity entity);
    }
}