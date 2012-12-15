using System.Collections.Generic;
using BLToolkit.Data;

namespace Gateways.Attachment
{
	public class AttachmentGateway : IAttachmentGateway
	{
		private readonly DbManager manager;
		private readonly QueryLog log;

		public AttachmentGateway(DbManager manager, QueryLog log)
		{
			this.manager = manager;
			this.log = log;
		}

		public IList<int> SelectIdentificatorsInLine(int lineId)
		{
			log.SelectAttachmentIdentificatorsInLine();

			return
				manager.SetCommand(
					"select AttachmentId from AttachmentBridge where LineId = @LineId",
					manager.Parameter("@LineId", lineId)).
					ExecuteScalarList<int>();
		}

		public IList<AttachmentRow> SelectAttachmentsByIdentificators(IList<int> identificators)
		{
			log.SelectAttachmentsByIdentificators();

			string sql = "Select *, @LineId as LineId from Attachment Where AttachmentId in (";

			foreach (int i in identificators)
			{
				sql += i + ",";
			}

			sql += "-1)";

			return manager
				.SetCommand(sql, manager.Parameter("LineId"))
				.ExecuteList<AttachmentRow>();
		}

		public void Insert(int lineId, AttachmentRow row)
		{
			log.AttachmentInsert();

			row.AttachmentId = manager
					.SetCommand("Insert into [Attachment] (FileName, Body) Values (@FileName, @Body); select @@Identity;", manager.CreateParameters(row))
					.ExecuteScalar<int>();

			manager.SetCommand("Insert into [AttachmentBridge] (AttachmentId, ItemId) values (@AttachmentId, @LineId)",
			                   manager.Parameter("AttachmentId", row.AttachmentId), manager.Parameter("LineId", lineId))
												 .ExecuteNonQuery();
		}

		public IList<AttachmentRow> SelectAttachmentsInOrder(int orderId)
		{
			log.SelectAllAttachmentsInOrder();

			return manager
				.SetCommand("Select * ,	(Select ItemId from AttachmentBridge as b where a.AttachmentId = b.AttachmentId) as LineId From [Attachment] as a")
				.ExecuteList<AttachmentRow>();
		}
	}
}
