using BusinessEntityLab.Entities;
using BusinessEntityMappers;

namespace BusinessEntityLab.Mappers
{
	public class OrderMapper : BusinessEntityMapper<Order>
	{
		public OrderMapper(ILoaderFactory<Order> factory)
			: base(factory)
		{
		}
	}
}
