namespace BusinessEntityMappers
{
	/// <summary>
	/// This interface should implement all business entities
	/// </summary>
	public interface IStoreEntity
	{
		/// <summary>
		/// Gets the store status.
		/// </summary>
		/// <value>The store status.</value>
		StoreStatus StoreStatus { get; }

		/// <summary>
		/// Gets or sets the storage identificator.
		/// </summary>
		/// <value>The id.</value>
		int Id { get; set; }
	}
}