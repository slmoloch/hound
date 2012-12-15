using BusinessEntityMappers;

namespace BusinessEntityLab.Entities
{
	public abstract class Order
	{
		private IBusinessEntityCollection<Line> lines;

		public Order()
		{
			lines = BusinessEntityCollectionFactory.CreateEmptyCollection<Line>();
		}

		/// <summary>
		/// Gets or sets the lines.
		/// </summary>
		/// <value>The lines.</value>
		public IBusinessEntityCollection<Line> Lines
		{
			get { return lines; }
			set { lines = value; }
		}

		/// <summary>
		/// Gets or sets the order id.
		/// </summary>
		/// <value>The order id.</value>
		[CausesLoad, IdentityProperty]
		public abstract int OrderId { get; set; }

		/// <summary>
		/// Gets or sets the purchase order number.
		/// </summary>
		/// <value>The purchase order number.</value>
		[CausesLoad]
		public abstract string PurchaseOrderNumber { get; set; }

		/// <summary>
		/// Gets or sets the bill to party.
		/// </summary>
		/// <value>The bill to party.</value>
		[CausesLoad]
		public abstract Party BillToParty { get; set; }

		/// <summary>
		/// Gets or sets the supplier party.
		/// </summary>
		/// <value>The supplier party.</value>
		[CausesLoad]
		public abstract Party SupplierParty { get; set; }
	}
}