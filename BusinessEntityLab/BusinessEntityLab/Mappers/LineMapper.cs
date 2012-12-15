using BusinessEntityLab.Entities;
using BusinessEntityMappers;

namespace BusinessEntityLab.Mappers
{
	public class LineMapper : BusinessEntityMapper<Line>
	{
		public LineMapper(ILoaderFactory<Line> factory)
			: base(factory)
		{}
	}
}
