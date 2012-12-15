using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Savers;
using BusinessEntityMappers;

namespace BusinessEntityLab.Mappers
{
	class LineSaveMapper : BusinessEntitySaveMapper<Line>
	{
		public LineSaveMapper(ILineSaveJunction saveJunction)
			: base(new LineSaver(saveJunction))
		{
		}
	}
}
