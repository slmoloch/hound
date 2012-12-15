using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityMappers;
using Gateways.Party;

namespace BusinessEntityLab.Loaders.Parties
{
	internal class PartyLoader : IEntityLoader<Party>
	{
		private readonly int id;
		private readonly IPartyDataJunction partyDataJunction;

		public PartyLoader(int id, IPartyDataJunction partyDataJunction)
		{
			this.id = id;
			this.partyDataJunction = partyDataJunction;
		}

		public bool IsExists()
		{
			return partyDataJunction.IsPartyExists(id);
		}

		public void Init(Party item)
		{
		}

		public void Load(Party item)
		{
			PartyRow partyRow = partyDataJunction.SelectPartyById(id);
			item.Name = partyRow.Name;
		}

		public int Id
		{
			get { return id; }
		}
	}
}