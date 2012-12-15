using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Savers;
using BusinessEntityMappers;

namespace BusinessEntityLab.Mappers
{
	public class OrderSaveMapper : BusinessEntitySaveMapper<Order>
	{
		public OrderSaveMapper(IOrderSaveJunction junction)
			: base(new OrderSaver(junction))
		{
		}
	}
}
