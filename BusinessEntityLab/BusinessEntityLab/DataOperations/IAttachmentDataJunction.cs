using System.Collections.Generic;
using Gateways.Attachment;

namespace BusinessEntityLab.DataOperations
{
	/// <summary>
	/// 
	/// </summary>
	public interface IAttachmentDataJunction
	{
		IList<int> SelectAttachmentIdentificatorsInLine(int lineId);
		AttachmentRow SelectAttachmentById(int attachmentId);
		bool IsAttachmentExists(int attachmentId);
	}
}
