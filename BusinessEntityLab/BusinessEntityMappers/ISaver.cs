namespace BusinessEntityMappers
{
	/// <summary>
	/// Stores business entity
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISaver<T>
	{
		/// <summary>
		/// Saves the nested entities.
		/// </summary>
		/// <param name="entity">The entity.</param>
		void SaveNestedEntities(T entity);

		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		void Update(T entity);

		/// <summary>
		/// Inserts the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		void Insert(T entity);

		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		void Delete(T entity);
	}
}