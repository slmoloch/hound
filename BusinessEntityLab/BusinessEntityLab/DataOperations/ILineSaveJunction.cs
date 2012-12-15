using Gateways.Line;

namespace BusinessEntityLab.DataOperations
{
	public interface ILineSaveJunction : IAttachmentSaveJunction
	{
		/// <summary>
		/// Updates the line.
		/// </summary>
		/// <param name="row">The row.</param>
		void UpdateLine(LineRow row);

		/// <summary>
		/// Inserts the line.
		/// </summary>
		/// <param name="row">The row.</param>
		void InsertLine(LineRow row);

		/// <summary>
		/// Deletes the line.
		/// </summary>
		/// <param name="lineId">The line id.</param>
		void DeleteLine(int lineId);
	}
}
