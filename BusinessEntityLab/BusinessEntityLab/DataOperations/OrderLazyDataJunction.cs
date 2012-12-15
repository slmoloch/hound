using System.Collections.Generic;
using BusinessEntityMappers.Exceptions;
using Gateways;
using Gateways.Attachment;
using Gateways.Line;
using Gateways.Order;
using Gateways.Party;

namespace BusinessEntityLab.DataOperations
{
	public class OrderLazyDataJunction : IOrderDataJunction
	{
		private readonly GatewayManager manager;
		private readonly IDictionary<int, LineRow> linesDict = new Dictionary<int, LineRow>();

		public OrderLazyDataJunction(GatewayManager manager)
		{
			this.manager = manager;
		}

		public bool IsOrderExists(int orderId)
		{
			return true;
		}

		public bool IsPartyExists(int partyId)
		{
			return true;
		}

		public bool IsAttachmentExists(int attachmentId)
		{
			return true;
		}

	    public bool IsLineExists(int lineId)
	    {
	        return true;
	    }

	    public OrderRow SelectOrderById(int orderId)
		{
			OrderRow orderRow = manager.Order.SelectById(orderId);

			if (orderRow == null)
			{
				throw new LoadUnexistedEntityException();
			}

			return orderRow;
		}

	    public PartyRow SelectPartyById(int partyId)
	    {
	        PartyRow partyRow = manager.Party.SelectById(partyId);

	        if (partyRow == null)
	        {
	            throw new LoadUnexistedEntityException();
	        }

	        return partyRow;
	    }

	    public LineRow SelectLineById(int lineId)
	    {
	        if (linesDict.ContainsKey(lineId))
	        {
	            return linesDict[lineId];
	        }

	        return manager.Line.SelectById(lineId);
	    }

	    public AttachmentRow SelectAttachmentById(int attachmentId)
	    {
	        throw new System.NotImplementedException();
	    }

	    public IList<int> SelectLineIdentificatorsOnPage(int startRow, int pageSize, string sortExpression, int orderId)
		{
			IList<int> identificators = manager.Line.SelectIdentificatorsOnPage(startRow, pageSize, sortExpression, orderId);

			IList<LineRow> lines = manager.Line.SelectByIdentificators(identificators);

			foreach (LineRow lineRow in lines)
			{
				linesDict.Add(lineRow.LineId, lineRow);
			}

			return identificators;
		}

		public IList<int> SelectLineIdentificatorsInOrder(int orderId)
		{
			return manager.Line.SelectIdentificatorsInOrder(orderId);
		}

	    public IList<int> SelectAttachmentIdentificatorsInLine(int lineId)
		{
			throw new System.NotImplementedException();
		}
	}
}
