namespace BusinessEntityMappers
{
	interface ISaveMapper<T> 
		where T : class
	{
		/// <summary>
		/// Stores the specified entity.
		/// </summary>
		/// <param name="entity">The po.</param>
		void Store(T entity);

		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		void Delete(T entity);
	}
}
