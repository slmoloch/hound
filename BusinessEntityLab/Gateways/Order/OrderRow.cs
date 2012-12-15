using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace Gateways.Order
{
	[TableName("Order")]
	public class OrderRow
	{
		public OrderRow() { }

		public OrderRow(string purchaseOrderNumber, int? billToPartyId, int? supplierPartyId)
			: this(0, purchaseOrderNumber, billToPartyId, supplierPartyId)
		{ }

		public OrderRow(int orderId, string purchaseOrderNumber, int? billToPartyId, int? supplierPartyId)
		{
			this.orderId = orderId;
			this.purchaseOrderNumber = purchaseOrderNumber;
			this.billToPartyId = billToPartyId;
			this.supplierPartyId = supplierPartyId;
		}

		private int orderId;
		private string purchaseOrderNumber;
		private int? billToPartyId;
		private int? supplierPartyId;


		[MapField("OrderId"), PrimaryKey, NonUpdatable]
		public int OrderId
		{
			get { return orderId; }
			set { orderId = value; }
		}

		[MapField("Number")]
		public string PurchaseOrderNumber
		{
			get { return purchaseOrderNumber; }
			set { purchaseOrderNumber = value; }
		}

		[MapField("BillToPartyId"), Nullable]
		public int? BillToPartyId
		{
			get { return billToPartyId; }
			set { billToPartyId = value; }
		}

		[MapField("SupplierPartyId"), Nullable]
		public int? SupplierPartyId
		{
			get { return supplierPartyId; }
			set { supplierPartyId = value; }
		}
	}
}