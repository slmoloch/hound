using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityMappers;
using Gateways.Party;

namespace BusinessEntityLab.Savers
{
	public class PartySaver : ISaver<Party>
	{
		private readonly IPartySaveJunction junction;

		public PartySaver(IPartySaveJunction junction)
		{
			this.junction = junction;
		}

		public void SaveNestedEntities(Party entity)
		{
		}

		public void Update(Party entity)
		{
			junction.UpdateParty(new PartyRow(entity.PartyId, entity.Name));
		}

		public void Insert(Party entity)
		{
			PartyRow row = new PartyRow(entity.Name);
			junction.InsertParty(row);
			entity.PartyId = row.Id;
		}

		public void Delete(Party entity)
		{
		}
	}
}
