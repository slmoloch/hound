using System.Collections.Generic;

namespace Gateways.Attachment
{
	public interface IAttachmentGateway
	{
		IList<int> SelectIdentificatorsInLine(int lineId);
		IList<AttachmentRow> SelectAttachmentsByIdentificators(IList<int> identificators);
		void Insert(int lineId, AttachmentRow row);
		IList<AttachmentRow> SelectAttachmentsInOrder(int orderId);
	}
}