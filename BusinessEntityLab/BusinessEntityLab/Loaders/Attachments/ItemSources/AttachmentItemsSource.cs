using System.Collections.Generic;
using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Mappers;
using BusinessEntityMappers;

namespace BusinessEntityLab.Loaders.Attachments.ItemSources
{
	public class AttachmentItemsSource : IBusinessCollectionItemsSource<Attachment>
	{
		private readonly int lineId;
		private readonly IAttachmentDataJunction attachmentDataJunction;
		private readonly AttachmentMapper mapper;

		public AttachmentItemsSource(int lineId, IAttachmentDataJunction attachmentDataJunction, AttachmentMapper mapper)
		{
			this.lineId = lineId;
			this.attachmentDataJunction = attachmentDataJunction;
			this.mapper = mapper;
		}

		public IList<int> SelectPageIdentificators(int startRow, int pageSize, string sortExpression)
		{
			throw new System.NotImplementedException();
		}

		public IList<Attachment> LoadAllEntities()
		{
			IList<int> ids = attachmentDataJunction.SelectAttachmentIdentificatorsInLine(lineId);

			IList<Attachment> lines = new List<Attachment>();

			foreach (int identificator in ids)
			{
				lines.Add(mapper.Load(identificator));
			}

			return lines;
		}
	}
}
