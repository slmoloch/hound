namespace BusinessEntityMappers
{
	public delegate void StoreHandler<T>(T entity);

	public interface IMappedCollection<T>
	{
		void Store(
			StoreHandler<T> removeHandler, 
			StoreHandler<T> insertHandler, 
			StoreHandler<T> updateHandler);
	}
}