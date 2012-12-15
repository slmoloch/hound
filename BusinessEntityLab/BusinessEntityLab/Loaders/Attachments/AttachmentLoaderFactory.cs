using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityMappers;

namespace BusinessEntityLab.Loaders.Attachments
{
	public class AttachmentLoaderFactory : ILoaderFactory<Attachment>
	{
		private readonly IAttachmentDataJunction attachmentDataJunction;

		public AttachmentLoaderFactory(IAttachmentDataJunction attachmentDataJunction)
		{
			this.attachmentDataJunction = attachmentDataJunction;
		}

		public IEntityLoader<Attachment> GetEntityLoader(int id)
		{
			return new AttachmentLoader(id, attachmentDataJunction);
		}
	}
}
