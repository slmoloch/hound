using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Loaders.Attachments.ItemSources;
using BusinessEntityLab.Mappers;
using BusinessEntityMappers;
using Gateways.Line;

namespace BusinessEntityLab.Loaders.Lines
{
	public class LineLoader : IEntityLoader<Line>
	{
		private readonly ILineDataJunction lineDataJunction;
		private readonly int id;

		public LineLoader(int id, ILineDataJunction lineDataJunction)
		{
			this.lineDataJunction = lineDataJunction;
			this.id = id;
		}

		public bool IsExists()
		{
			return lineDataJunction.IsLineExists(id);
		}

		public void Init(Line item)
		{
			item.Attachments = BusinessEntityCollectionFactory.Create(new AttachmentItemsSource(id, lineDataJunction, new AttachmentMapper(lineDataJunction)));
		}

		public void Load(Line item)
		{
			LineRow line = lineDataJunction.SelectLineById(id);

			item.LineId = line.LineId;
			item.LineNumber = line.LineNumber;
			item.ParentId = line.OrderId;
		}

		public int Id
		{
			get { return id; }
		}
	}
}