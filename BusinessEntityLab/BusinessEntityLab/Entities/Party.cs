using BusinessEntityMappers;

namespace BusinessEntityLab.Entities
{
	public abstract class Party
	{
		/// <summary>
		/// Gets or sets the party id.
		/// </summary>
		/// <value>The party id.</value>
		[CausesLoad, IdentityProperty]
		public abstract int PartyId { get; set;}

		/// <summary>
		/// Gets or sets the purchase order number.
		/// </summary>
		/// <value>The purchase order number.</value>
		[CausesLoad]
		public abstract string Name { get; set; }
	}
}
