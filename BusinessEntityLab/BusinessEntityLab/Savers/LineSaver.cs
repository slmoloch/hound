using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityMappers;
using Gateways.Line;

namespace BusinessEntityLab.Savers
{
	public class LineSaver : ISaver<Line>
	{
		private readonly ILineSaveJunction junction;

		public LineSaver(ILineSaveJunction junction)
		{
			this.junction = junction;
		}

		public void SaveNestedEntities(Line entity)
		{
		}

		public void Update(Line entity)
		{
			junction.UpdateLine(new LineRow(entity.ParentId, entity.LineNumber, entity.ParentId));
		}

		public void Insert(Line entity)
		{
			LineRow lineRow = new LineRow(entity.LineNumber, entity.ParentId);

			junction.InsertLine(lineRow);

			entity.LineId = lineRow.LineId;
		}

		public void Delete(Line entity)
		{
			junction.DeleteLine(entity.LineId);
		}
	}
}