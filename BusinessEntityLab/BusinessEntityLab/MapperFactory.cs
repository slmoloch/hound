using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Loaders.Lines;
using BusinessEntityLab.Loaders.Orders;
using BusinessEntityLab.Loaders.Parties;
using BusinessEntityLab.Mappers;
using Gateways;

namespace BusinessEntityLab
{
	public class MapperFactory
	{
		public static LineMapper GetLineMapper(GatewayManager manager)
		{
			return new LineMapper(new LineLoaderFactory(null));
		}

		public static OrderMapper GetLazyOrderMapper(GatewayManager manager)
		{			
			return new OrderMapper(new OrderLoaderFactory(delegate { return new OrderLazyDataJunction(manager); }));
		}

		public static OrderMapper GetPreLoadedOrderMapper(GatewayManager manager)
		{
			return new OrderMapper(new OrderLoaderFactory(delegate { return new OrderPreloadedDataJunction(manager); }));
		}

		public static PartyMapper GetPreLoadedPartyMapper(GatewayManager manager)
		{
			return new PartyMapper(new PartyLoaderFactory(null));
		}

		public static OrderSaveMapper GetSaveOrderMapper(GatewayManager manager)
		{
            return new OrderSaveMapper(new OrderSaveJunction(manager));
		}
	}
}