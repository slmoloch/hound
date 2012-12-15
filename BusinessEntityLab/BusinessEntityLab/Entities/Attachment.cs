using BusinessEntityMappers;

namespace BusinessEntityLab.Entities
{
	public abstract class Attachment
	{
		/// <summary>
		/// Gets or sets the attachment id.
		/// </summary>
		/// <value>The attachment id.</value>
		[CausesLoad, IdentityProperty]
		public abstract int AttachmentId { get; set; }

		/// <summary>
		/// Gets or sets the line number.
		/// </summary>
		/// <value>The line number.</value>
		[CausesLoad]
		public abstract string FileName { get; set; }

		/// <summary>
		/// Gets or sets the line number.
		/// </summary>
		/// <value>The line number.</value>
		[CausesLoad]
		public abstract string Body { get; set; }
	}
}
