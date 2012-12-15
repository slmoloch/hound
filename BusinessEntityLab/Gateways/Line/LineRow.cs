using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace Gateways.Line
{
	[TableName("Line")]
	public class LineRow
	{
		#region Constructors

		public LineRow() {}

		public LineRow(string lineNumber, int orderId)
		{
			this.lineNumber = lineNumber;
			this.orderId = orderId;
		}

		public LineRow(int lineId, string lineNumber, int orderId)
			: this(lineNumber, orderId)
		{
			this.lineId = lineId;
		}

		#endregion

		#region Private fields

		private int lineId;
		private int orderId;
		private string lineNumber;

		#endregion

		[MapField("LineId"), PrimaryKey, NonUpdatable]
		public int LineId
		{
			get { return lineId; }
			set { lineId = value; }
		}

		[MapField("Number")]
		public string LineNumber
		{
			get { return lineNumber; }
			set { lineNumber = value; }
		}

		[MapField("OrderId")]
		public int OrderId
		{
			get { return orderId; }
			set { orderId = value; }
		}
	}
}