using System.Configuration;
using System.Data.SqlClient;
using Gateways;
using Gateways.Attachment;
using Gateways.Line;
using Gateways.Order;
using Gateways.Party;

namespace Tests
{
	public class TestStorage
	{
		public TestStorage()
		{
			InitStorage();
		}

		private int orderId;
		private int firstLineId;
		private int secondLineId;
		private int firstAttachmentId;
		private	int secondOrderId;

		private void InitStorage()
		{
			using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
			{
				conn.Open();

				SqlCommand command = new SqlCommand(@"
							Delete from [AttachmentBridge]; 
							Delete from [Attachment]; 
							Delete from [Line]; 
							Delete from [Order]; 
							Delete from [Party];", conn);

				command.ExecuteNonQuery();
			}

			using (GatewayManager manager = new GatewayManager())
			{
				PartyRow partyRow = new PartyRow("party1");
				manager.Party.Insert(partyRow);

				OrderRow orderInserted = new OrderRow("lama", partyRow.Id, null);
				manager.Order.Insert(orderInserted);
				orderId = orderInserted.OrderId;

				LineRow firstLine = new LineRow("lama1", orderInserted.OrderId);
				manager.Line.Insert(firstLine);
				firstLineId = firstLine.LineId;

				LineRow secondLine = new LineRow("lama2", orderInserted.OrderId);
				manager.Line.Insert(secondLine);
				secondLineId = secondLine.LineId;

				manager.Line.Insert(new LineRow("lama3", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama4", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama5", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama6", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama7", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama8", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama9", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama0", orderInserted.OrderId));

				manager.Line.Insert(new LineRow("lama1", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama2", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama3", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama4", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama5", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama6", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama7", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama8", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama9", orderInserted.OrderId));
				manager.Line.Insert(new LineRow("lama0", orderInserted.OrderId));

				AttachmentRow attachmentRow = new AttachmentRow("fileName1", "body1");
				manager.Attachment.Insert(firstLineId, attachmentRow);
				firstAttachmentId = attachmentRow.AttachmentId;

				manager.Attachment.Insert(firstLineId, new AttachmentRow("fileName2", "body2"));
				manager.Attachment.Insert(secondLineId, new AttachmentRow("fileName3", "body3"));
				manager.Attachment.Insert(secondLineId, new AttachmentRow("fileName4", "body4"));

				OrderRow orderInserted2= new OrderRow("order2", null, null);
				manager.Order.Insert(orderInserted2);
				secondOrderId = orderInserted2.OrderId;
			}
		}

		public int OrderId
		{
			get { return orderId; }
		}

		public int SecondOrderId
		{
			get { return secondOrderId; }
		}

		public int FirstLineId
		{
			get { return firstLineId; }
		}

		public int FirstAttachmentId
		{
			get { return firstAttachmentId; }
		}
	}
}
