using System.Collections.Generic;
using Gateways;
using Gateways.Attachment;
using Gateways.Line;
using Gateways.Order;
using Gateways.Party;

namespace BusinessEntityLab.DataOperations
{
    public class OrderPreloadedDataJunction : IOrderDataJunction
	{
		private readonly GatewayManager manager;
		private OrderRow orderRow;
		private IList<LineRow> lines;
		private IList<AttachmentRow> attachments;
		private IList<PartyRow> parties;

		public OrderPreloadedDataJunction(GatewayManager manager)
		{
			this.manager = manager;
		}

		public bool IsOrderExists(int orderId)
		{
			orderRow = manager.Order.SelectById(orderId);
			if (orderRow == null) return false;

			lines = manager.Line.SelectInOrder(orderId);
			attachments = manager.Attachment.SelectAttachmentsInOrder(orderId);
			parties = manager.Party.SelectAllPartiesInOrder(orderId);

			return true;
		}

	    public bool IsPartyExists(int partyId)
	    {
	        return SelectPartyById(partyId) != null;
	    }

	    public bool IsLineExists(int lineId)
		{
			return true;
		}

	    public bool IsAttachmentExists(int attachmentId)
	    {
	        return true;
	    }

	    public LineRow SelectLineById(int lineId)
		{
			foreach (LineRow row in lines)
			{
				if (row.LineId == lineId)
				{
					return row;
				}
			}

			return null;
		}

	    public OrderRow SelectOrderById(int orderId)
	    {
	        return orderRow;
	    }

	    public PartyRow SelectPartyById(int partyId)
	    {
	        foreach (PartyRow row in parties)
	        {
	            if (row.Id == partyId)
	            {
	                return row;
	            }
	        }

	        return null;
	    }

	    public IList<int> SelectAttachmentIdentificatorsInLine(int lineId)
	    {
	        IList<int> ids = new List<int>();

	        foreach (AttachmentRow row in attachments)
	        {
	            if (row.LineId == lineId)
	            {
	                ids.Add(row.AttachmentId);
	            }
	        }

	        return ids;
	    }

	    public AttachmentRow SelectAttachmentById(int attachmentId)
	    {
	        foreach (AttachmentRow row in attachments)
	        {
	            if (row.AttachmentId == attachmentId)
	            {
	                return row;
	            }
	        }

	        return null;
	    }

	    public IList<int> SelectLineIdentificatorsOnPage(int startRow, int pageSize, string sortExpression, int orderId)
	    {
	        return manager.Line.SelectIdentificatorsOnPage(startRow, pageSize, sortExpression, orderId);
	    }

	    public IList<int> SelectLineIdentificatorsInOrder(int orderId)
	    {
	        IList<int> ids = new List<int>();
			
	        foreach (LineRow row in lines)
	        {
	            ids.Add(row.LineId);
	        }

	        return ids;
	    }
	}
}
