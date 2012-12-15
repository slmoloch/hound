using System.Collections.Generic;

namespace BusinessEntityMappers
{
	public class EmptyLoadCollectionStrategy<T> : IBusinessCollectionItemsSource<T>
	{
		public IList<int> SelectPageIdentificators(int startRow, int pageSize, string sortExpression)
		{
			return new List<int>();
		}

		public IList<T> LoadAllEntities()
		{
			return new List<T>();
		}
	}
}