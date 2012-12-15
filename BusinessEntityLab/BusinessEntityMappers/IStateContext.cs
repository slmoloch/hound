namespace BusinessEntityMappers
{
	/// <summary>
	/// State context manages states of business entity.
	/// </summary>
	public interface IStateContext
	{
		/// <summary>
		/// Connects this instance.
		/// </summary>
		void Connect();

		/// <summary>
		/// Stores this instance.
		/// </summary>
		void Store();
	}
}