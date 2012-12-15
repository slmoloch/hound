using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Loaders.Attachments;
using BusinessEntityMappers;

namespace BusinessEntityLab.Mappers
{
	public class AttachmentMapper : BusinessEntityMapper<Attachment>
	{
		public AttachmentMapper(IAttachmentDataJunction attachmentDataJunction)
			: base(new AttachmentLoaderFactory(attachmentDataJunction))
		{
		}
	}
}
