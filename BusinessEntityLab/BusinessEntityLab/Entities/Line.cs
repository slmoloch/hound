using BusinessEntityMappers;

namespace BusinessEntityLab.Entities
{
	public abstract class Line
	{
		private IBusinessEntityCollection<Attachment> attachments;

		public Line()
		{
			attachments = BusinessEntityCollectionFactory.CreateEmptyCollection<Attachment>();
		}

		/// <summary>
		/// Gets or sets the parent id.
		/// </summary>
		/// <value>The parent id.</value>
		[CausesLoad, IdentityProperty]
		public abstract int LineId { get; set; }

		/// <summary>
		/// Gets or sets the line number.
		/// </summary>
		/// <value>The line number.</value>
		[CausesLoad]
		public abstract string LineNumber { get; set; }

		/// <summary>
		/// Gets or sets the parent id.
		/// </summary>
		/// <value>The parent id.</value>
		[CausesLoad]
		public abstract int ParentId { get; set; }

		/// <summary>
		/// Gets or sets the lines.
		/// </summary>
		/// <value>The lines.</value>
		public IBusinessEntityCollection<Attachment> Attachments
		{
			get { return attachments; }
			set { attachments = value; }
		}
	}
}