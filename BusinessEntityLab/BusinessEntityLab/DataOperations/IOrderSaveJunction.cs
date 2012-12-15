using Gateways.Order;

namespace BusinessEntityLab.DataOperations
{
	public interface IOrderSaveJunction: ILineSaveJunction, IPartySaveJunction
	{
		/// <summary>
		/// Deletes the order.
		/// </summary>
		/// <param name="orderId">The order id.</param>
		void DeleteOrder(int orderId);

		/// <summary>
		/// Updates the order.
		/// </summary>
		/// <param name="orderRow">The order row.</param>
		void UpdateOrder(OrderRow orderRow);

		/// <summary>
		/// Inserts the order.
		/// </summary>
		/// <param name="orderRow">The order row.</param>
		void InsertOrder(OrderRow orderRow);
	}
}
