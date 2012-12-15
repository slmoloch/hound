using System.Collections.Generic;

namespace BusinessEntityMappers
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IBusinessEntityCollection<T> : IEnumerable<T>
	{
		/// <summary>
		/// Selects the with paging.
		/// </summary>
		/// <param name="startRow">The start row.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="sortExpression">The sort expression.</param>
		/// <returns></returns>
		IList<T> SelectWithPaging(int startRow, int pageSize, string sortExpression);
		
		/// <summary>
		/// Selects the item by id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		T SelectById(int id);

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		int Count { get;}

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The line.</param>
		void Add(T item);

		/// <summary>
		/// Removes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		void Remove(T item);
	}
}