using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Mappers;
using BusinessEntityMappers;
using Gateways.Order;

namespace BusinessEntityLab.Savers
{
	public class OrderSaver : ISaver<Order>
	{
		private readonly IOrderSaveJunction junction;


		public OrderSaver(IOrderSaveJunction junction)
		{
			this.junction = junction;
		}

		public void Insert(Order po)
		{
			StoreParties(po);
			InsertOrderRow(po);
			StoreLines(po);
		}

		public void Delete(Order entity)
		{
			IMappedCollection<Line> lines = (IMappedCollection<Line>)entity.Lines;

			LineSaveMapper lineMapper = new LineSaveMapper(junction);
			PartySaveMapper partyMapper = new PartySaveMapper(junction);

			lines.Store(
				delegate(Line line) { lineMapper.Delete(line); },
				null,
				null);

			foreach (Line line in entity.Lines)
			{
				lineMapper.Delete(line);
			}

			junction.DeleteOrder(entity.OrderId);


			if (entity.BillToParty != null)
			{
				partyMapper.Delete(entity.BillToParty);
			}

			if (entity.SupplierParty != null)
			{
				partyMapper.Delete(entity.SupplierParty);
			}
		}

		public void Update(Order po)
		{
			StoreParties(po);
			junction.UpdateOrder(GetOrderRow(po));
			StoreLines(po);
		}

		public void SaveNestedEntities(Order po)
		{
			StoreLines(po);
			StoreParties(po);
		}

		private void InsertOrderRow(Order po)
		{
			OrderRow orderRow = GetOrderRow(po);
			junction.InsertOrder(orderRow);
			po.OrderId = orderRow.OrderId;
		}

		private static OrderRow GetOrderRow(Order po)
		{
			int? billToPartyId = GetPartyId(po.BillToParty);
			int? supplierPartyId = GetPartyId(po.SupplierParty);

			return new OrderRow(po.PurchaseOrderNumber, billToPartyId, supplierPartyId);
		}

		private static int? GetPartyId(Party party)
		{
			int? partyId = null;

			if (party != null)
			{
				partyId = party.PartyId;
			}

			return partyId;
		}

		private void StoreLines(Order po)
		{
			IMappedCollection<Line> lines = (IMappedCollection<Line>)po.Lines;

			LineSaveMapper lineMapper = new LineSaveMapper(junction);
			
			lines.Store(
				delegate(Line line)
				{
					lineMapper.Delete(line);
				},
				delegate(Line line)
				{
					line.ParentId = po.OrderId;
					lineMapper.Store(line);
				},
				delegate(Line line)
				{
					lineMapper.Store(line);
				});
		}

		private void StoreParties(Order po)
		{
			PartySaveMapper partyMapper = new PartySaveMapper(junction);

			if (po.BillToParty != null)
			{
				partyMapper.Store(po.BillToParty);
			}

			if (po.SupplierParty != null)
			{
				partyMapper.Store(po.SupplierParty);
			}
		}
	}
}