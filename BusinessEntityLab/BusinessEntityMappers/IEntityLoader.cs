namespace BusinessEntityMappers
{
	/// <summary>
	/// Entity loader.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEntityLoader<T>
	{
		/// <summary>
		/// Determines whether this instance is exists.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is exists; otherwise, <c>false</c>.
		/// </returns>
		bool IsExists();

		/// <summary>
		/// Inits the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		void Init(T item);

		/// <summary>
		/// Loads the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		void Load(T item);

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		int Id { get; }
	}
}