using System.Collections.Generic;
using BLToolkit.Data;
using BLToolkit.DataAccess;
using Gateways.Order;

namespace Gateways.Party
{
	public class PartyGateway : IPartyGateway
	{
		private readonly DbManager manager;
		private readonly QueryLog log;

		public PartyGateway(DbManager manager, QueryLog log)
		{
			this.manager = manager;
			this.log = log;
		}

		public void Insert(PartyRow partyRow)
		{
			log.PartyInsert();

			partyRow.Id = manager
					.SetCommand("Insert into [Party] (Name) Values (@Name); select @@Identity;", manager.CreateParameters(partyRow))
					.ExecuteScalar<int>();
		}
		
		public void Update(PartyRow partyRow)
		{
			log.PartyUpdate();

			partyRow.Id = manager
					.SetCommand("Update [Party] Set Name = @Name where PartyId = @PartyId", 
					manager.CreateParameters(partyRow))
					.ExecuteNonQuery();
		}

		public IList<PartyRow> SelectAllPartiesInOrder(int orderId)
		{
			log.SelectAllPartiesInOrder();

			IList<PartyRow> parties = new List<PartyRow>();

			OrderRow order = new SqlQuery<OrderRow>(manager).SelectByKey(orderId);

			if(order.BillToPartyId != null)
			{
				parties.Add(Select(order.BillToPartyId.Value));
			}

			if (order.SupplierPartyId != null)
			{
				parties.Add(Select(order.SupplierPartyId.Value));
			}

			return parties;
		}

		public PartyRow SelectById(int partyId)
		{
			log.PartySelect();

			return Select(partyId);
		}

		private PartyRow Select(int partyId)
		{
			return new SqlQuery<PartyRow>(manager).SelectByKey(partyId);
		}
	}
}
