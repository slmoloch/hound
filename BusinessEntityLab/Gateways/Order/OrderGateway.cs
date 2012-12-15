using BLToolkit.Data;
using BLToolkit.DataAccess;

namespace Gateways.Order
{
	internal class OrderGateway 
		: IOrderGateway
	{
		private readonly DbManager manager;
		private readonly QueryLog log;

		public OrderGateway(DbManager manager, QueryLog log)
		{
			this.manager = manager;
			this.log = log;
		}

		public OrderRow SelectById(int id)
		{
			log.SelectOrderById();

			return new SqlQuery<OrderRow>(manager).SelectByKey(id);
		}

		public void Update(OrderRow order)
		{
			log.OrderUpdate();

			new SqlQuery<OrderRow>(manager).Update(order);
		}

		public void Insert(OrderRow order)
		{
			log.OrderInsert();

			order.OrderId = manager
				.SetCommand("Insert into [order] (Number, BillToPartyId, SupplierPartyId) Values (@Number, @BillToPartyId, @SupplierPartyId); select @@Identity;", manager.CreateParameters(order))
				.ExecuteScalar<int>();
		}

		public void Delete(int id)
		{
			log.OrderDelete();

			manager.SetCommand("Delete from [order] where orderId = @orderId", manager.Parameter("@orderId", id)).
					ExecuteNonQuery();
		}
	}
}