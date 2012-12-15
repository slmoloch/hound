using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace Gateways.Attachment
{
	public class AttachmentRow
	{
		private int attachmentId;
		private string fileName;
		private string body;
		private int lineId;

		public AttachmentRow()
		{
		}

		public AttachmentRow(string fileName, string body)
		{
			this.fileName = fileName;
			this.body = body;
		}

		[MapField("AttachmentId"), PrimaryKey, NonUpdatable]
		public int AttachmentId
		{
			get { return attachmentId; }
			set { attachmentId = value; }
		}

		[MapField("FileName")]
		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		[MapField("Body")]
		public string Body
		{
			get { return body; }
			set { body = value; }
		}

		[MapField("LineId")]
		public int LineId
		{
			get { return lineId; }
			set { lineId = value; }
		}
	}
}