using System.Collections.Generic;

namespace BusinessEntityMappers
{
	public interface IBusinessCollectionItemsSource<T>
	{
		IList<int> SelectPageIdentificators(int startRow, int pageSize, string sortExpression);
		IList<T> LoadAllEntities();
	}
}