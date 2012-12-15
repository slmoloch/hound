using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityMappers;
using Gateways.Attachment;

namespace BusinessEntityLab.Loaders.Attachments
{
	/// <summary>
	/// Attachment loader.
	/// </summary>
	public class AttachmentLoader : IEntityLoader<Attachment>
	{
		private readonly int id;
		private readonly IAttachmentDataJunction attachmentDataJunction;

		/// <summary>
		/// Initializes a new instance of the <see cref="AttachmentLoader"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="attachmentDataJunction">The attachment data.</param>
		public AttachmentLoader(int id, IAttachmentDataJunction attachmentDataJunction)
		{
			this.id = id;
			this.attachmentDataJunction = attachmentDataJunction;
		}

		/// <summary>
		/// Determines whether this instance is exists.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is exists; otherwise, <c>false</c>.
		/// </returns>
		public bool IsExists()
		{
			return attachmentDataJunction.IsAttachmentExists(id);
		}

		/// <summary>
		/// Inits the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Init(Attachment item)
		{
		}

		/// <summary>
		/// Loads the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Load(Attachment item)
		{
			AttachmentRow row = attachmentDataJunction.SelectAttachmentById(id);

			item.Body = row.Body;
			item.FileName = row.FileName;
			item.AttachmentId = row.AttachmentId;
		}

		public int Id
		{
			get { return id; }
		}
	}
}
