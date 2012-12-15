namespace BusinessEntityMappers
{
	public interface ILoaderFactory<T>
	{
		IEntityLoader<T> GetEntityLoader(int id);
	}
}