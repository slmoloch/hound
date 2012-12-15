using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Savers;
using BusinessEntityMappers;

namespace BusinessEntityLab.Mappers
{
	class PartySaveMapper : BusinessEntitySaveMapper<Party>
	{
		public PartySaveMapper(IPartySaveJunction junction) : base(new PartySaver(junction))
		{
		}
	}
}
