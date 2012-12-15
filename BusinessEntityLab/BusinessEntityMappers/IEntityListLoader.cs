using System.Collections.Generic;

namespace BusinessEntityMappers
{
	public interface IEntityListLoader<T>
	{
		IList<IEntityLoader<T>> GetEntityLoaders(object criteria);
	}
}