using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Loaders.Lines;
using BusinessEntityLab.Loaders.Lines.ItemSources;
using BusinessEntityLab.Loaders.Parties;
using BusinessEntityLab.Mappers;
using BusinessEntityMappers;
using Gateways.Order;

namespace BusinessEntityLab.Loaders.Orders
{
	public class OrderLoader : IEntityLoader<Order>
	{
		private readonly IOrderDataJunction dataJunction;
		private readonly int id;

		public OrderLoader(int id, IOrderDataJunction dataJunction)
		{
			this.id = id;
			this.dataJunction = dataJunction;
		}

		public bool IsExists()
		{
			return dataJunction.IsOrderExists(id);
		}

		public void Init(Order item)
		{
			item.Lines = BusinessEntityCollectionFactory.Create(new LineItemsSource(id, dataJunction, new LineMapper(new LineLoaderFactory(dataJunction))));
		}

		public void Load(Order item)
		{
			OrderRow orderRow = dataJunction.SelectOrderById(id);

			item.PurchaseOrderNumber = orderRow.PurchaseOrderNumber;
			item.BillToParty = LoadParty(orderRow.BillToPartyId);
			item.SupplierParty = LoadParty(orderRow.SupplierPartyId);
			item.OrderId = orderRow.OrderId;
		}

		public int Id
		{
			get { return id; }
		}

		private Party LoadParty(int? partyId)
		{
			return partyId.HasValue ? new PartyMapper(new PartyLoaderFactory(dataJunction)).Load(partyId.Value) : null;
		}
	}
}