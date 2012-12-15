using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Loaders.Orders;
using BusinessEntityMappers;

namespace BusinessEntityLab.Loaders.Orders
{
	public delegate IOrderDataJunction CreateDataJunctionDelegate();

	public class OrderLoaderFactory : ILoaderFactory<Order>
	{
		private readonly CreateDataJunctionDelegate createJunction;

		public OrderLoaderFactory(CreateDataJunctionDelegate createJunction)
		{
			this.createJunction = createJunction;
		}

		public IEntityLoader<Order> GetEntityLoader(int id)
		{
			return new OrderLoader(id, createJunction());
		}
	}


}