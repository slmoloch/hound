using System.Collections.Generic;

namespace BusinessEntityMappers
{
	/// <summary>
	/// Base interface for mappers.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMapper<T> 
		where T : class
	{
		/// <summary>
		/// Loads the entity by id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		T Load(int id);

		/// <summary>
		/// Creates new instance of entity.
		/// </summary>
		/// <returns></returns>
		T Create();
	}
}