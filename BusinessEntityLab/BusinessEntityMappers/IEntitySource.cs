namespace BusinessEntityMappers
{
	public interface IEntitySource<T>
	{
		T Create();
	}
}