using System.Collections.Generic;
using Gateways.Order;

namespace BusinessEntityLab.DataOperations
{
	/// <summary>
	/// Order data operations
	/// </summary>
	public interface IOrderDataJunction : IPartyDataJunction, ILineDataJunction
	{
		/// <summary>
		/// Determines whether [is order exists] [the specified order id].
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <returns>
		/// 	<c>true</c> if [is order exists] [the specified order id]; otherwise, <c>false</c>.
		/// </returns>
		bool IsOrderExists(int orderId);

		/// <summary>
		/// Selects the order by id.
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <returns></returns>
		OrderRow SelectOrderById(int orderId);

		/// <summary>
		/// Selects the line identificators on page.
		/// </summary>
		/// <param name="startRow">The start row.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="sortExpression">The sort expression.</param>
		/// <param name="orderId">The parent id.</param>
		/// <returns></returns>
		IList<int> SelectLineIdentificatorsOnPage(int startRow, int pageSize, string sortExpression, int orderId);

		/// <summary>
		/// Selects the line identificators in order.
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <returns></returns>
		IList<int> SelectLineIdentificatorsInOrder(int orderId);
	}
}