using System.Collections.Generic;

namespace Gateways.Party
{
	public interface IPartyGateway
	{
		void Insert(PartyRow partyRow);
		PartyRow SelectById(int partyId);
		void Update(PartyRow partyRow);
		IList<PartyRow> SelectAllPartiesInOrder(int orderId);
	}
}