using BusinessEntityLab.Entities;
using BusinessEntityMappers;

namespace BusinessEntityLab.Mappers
{
	public class PartyMapper : BusinessEntityMapper<Party>
	{
		public PartyMapper(ILoaderFactory<Party> factory)
			: base(factory)
		{
		}
	}
}
