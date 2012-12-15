using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Loaders.Parties;
using BusinessEntityMappers;

namespace BusinessEntityLab.Loaders.Parties
{
	public class PartyLoaderFactory : ILoaderFactory<Party>
	{
		private readonly IPartyDataJunction manager;

		public PartyLoaderFactory(IPartyDataJunction manager)
		{
			this.manager = manager;
		}

		public IEntityLoader<Party> GetEntityLoader(int id)
		{
			return new PartyLoader(id, manager);
		}
	}
}