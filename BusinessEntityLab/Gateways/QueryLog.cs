using System;

namespace Gateways
{
	public class QueryLog : IEquatable<QueryLog>
	{
		private int selectOrderByIdCount;
		private int orderUpdateCount;
		private int orderInsertCount;
		private int orderDeleteCount;
		private int selectLineIdentificatorsInOrderCount;
		private int selectLineByIdCount;
		private int selectLineIdentificatorsOnPageCount;
		private int lineUpdateCount;
		private int lineInsertCount;
		private int lineDeleteCount;
		private int selectInOrderCount;
		private int partyInsertCount;
		private int partySelectCount;
		private int partyUpdateCount;
		private int selectLinesByIdentificatorsCount;
		private int selectAllAttachmentsInOrder;
		private int selectAttachmentIdentificatorsInLine;
		private int selectAttachmentsByIdentificators;
		private int attachmentInsert;
		private int selectAllPartiesInOrder;

		public QueryLog SelectOrderById()
		{
			selectOrderByIdCount++;
			return this;
		}

		public QueryLog OrderUpdate()
		{
			orderUpdateCount++;
			return this;
		}

		public QueryLog OrderInsert()
		{
			orderInsertCount++;
			return this;
		}

		public QueryLog OrderDelete()
		{
			orderDeleteCount++;
			return this;
		}

		public QueryLog SelectLineIdentificatorsInOrder()
		{
			selectLineIdentificatorsInOrderCount++;
			return this;
		}

		public QueryLog SelectLineById()
		{
			selectLineByIdCount++;
			return this;
		}

		public QueryLog SelectLineIdentificatorsOnPage()
		{
			selectLineIdentificatorsOnPageCount++;
			return this;
		}

		public QueryLog LineUpdate()
		{
			lineUpdateCount++;
			return this;
		}

		public QueryLog LineInsert()
		{
			lineInsertCount++;
			return this;
		}

		public QueryLog LineDelete()
		{
			lineDeleteCount++;
			return this;
		}

		public QueryLog SelectAllLinesInOrder()
		{
			selectInOrderCount++;
			return this;
		}

		public QueryLog PartyInsert()
		{
			partyInsertCount++;
			return this;
		}

		public QueryLog PartySelect()
		{
			partySelectCount++;
			return this;
		}

		public QueryLog PartyUpdate()
		{
			partyUpdateCount++;
			return this;
		}

		public QueryLog SelectLinesByIdentificators()
		{
			selectLinesByIdentificatorsCount++;
			return this;
		}

		public QueryLog SelectAllAttachmentsInOrder()
		{
			selectAllAttachmentsInOrder++;
			return this;
		}

		public QueryLog SelectAttachmentIdentificatorsInLine()
		{
			selectAttachmentIdentificatorsInLine++;
			return this;
		}

		public QueryLog SelectAttachmentsByIdentificators()
		{
			selectAttachmentsByIdentificators++;
			return this;
		}

		public QueryLog AttachmentInsert()
		{
			attachmentInsert++;
			return this;
		}

		public string GetStringRepresentation()
		{
			return string.Format(@"selectOrderByIdCount == {0} &&
						 orderUpdateCount == {1} &&
						 orderInsertCount == {2} &&
						 orderDeleteCount == {3} &&
						 selectLineIdentificatorsInOrderCount == {4} &&
						 selectLineByIdCount == {5} &&
						 selectLineIdentificatorsOnPageCount == {6} &&
						 lineUpdateCount == {7} &&
						 lineInsertCount == {8} &&
						 lineDeleteCount == {9} &&
						 selectInOrderCount == {10} &&
						 partyInsertCount == {11} &&
						 partySelectCount == {12} && 
						 partyUpdateCount == {13} && 
						 selectLinesByIdentificatorsCount == {14} && 
						 selectAllAttachmentsInOrder == {15} && 
						 selectAttachmentIdentificatorsInLine == {16} &&
						 selectAttachmentsByIdentificators == {17} && 
						 attachmentInsert == {18} &&
						 selectAllPartiesInOrder == {19}",
													 selectOrderByIdCount,
													 orderUpdateCount,
													 orderInsertCount,
													 orderDeleteCount,
													 selectLineIdentificatorsInOrderCount,
													 selectLineByIdCount,
													 selectLineIdentificatorsOnPageCount,
													 lineUpdateCount,
													 lineInsertCount,
													 lineDeleteCount,
													 selectInOrderCount,
													 partyInsertCount,
													 partySelectCount,
													 partyUpdateCount,
													 selectLinesByIdentificatorsCount,
													 selectAllAttachmentsInOrder,
													 selectAttachmentIdentificatorsInLine,
													 selectAttachmentsByIdentificators,
													 attachmentInsert,
													 selectAllPartiesInOrder);
		}

		public void Reset()
		{
			selectOrderByIdCount = 0;
			orderUpdateCount = 0;
			orderInsertCount = 0;
			orderDeleteCount = 0;
			selectLineIdentificatorsInOrderCount = 0;
			selectLineByIdCount = 0;
			selectLineIdentificatorsOnPageCount = 0;
			lineUpdateCount = 0;
			lineInsertCount = 0;
			lineDeleteCount = 0;
			selectInOrderCount = 0;
			partyInsertCount = 0;
			partySelectCount = 0;
			partyUpdateCount = 0;
			selectLinesByIdentificatorsCount = 0;
			selectAllAttachmentsInOrder = 0;
			selectAttachmentIdentificatorsInLine = 0;
			selectAttachmentsByIdentificators = 0;
			attachmentInsert = 0;
			selectAllPartiesInOrder = 0;
		}

		public bool Equals(QueryLog queryLog)
		{
			if (queryLog == null) return false;
			if (selectOrderByIdCount != queryLog.selectOrderByIdCount) return false;
			if (orderUpdateCount != queryLog.orderUpdateCount) return false;
			if (orderInsertCount != queryLog.orderInsertCount) return false;
			if (orderDeleteCount != queryLog.orderDeleteCount) return false;
			if (selectLineIdentificatorsInOrderCount != queryLog.selectLineIdentificatorsInOrderCount) return false;
			if (selectLineByIdCount != queryLog.selectLineByIdCount) return false;
			if (selectLineIdentificatorsOnPageCount != queryLog.selectLineIdentificatorsOnPageCount) return false;
			if (lineUpdateCount != queryLog.lineUpdateCount) return false;
			if (lineInsertCount != queryLog.lineInsertCount) return false;
			if (lineDeleteCount != queryLog.lineDeleteCount) return false;
			if (selectInOrderCount != queryLog.selectInOrderCount) return false;
			if (partyInsertCount != queryLog.partyInsertCount) return false;
			if (partySelectCount != queryLog.partySelectCount) return false;
			if (partyUpdateCount != queryLog.partyUpdateCount) return false;
			if (selectLinesByIdentificatorsCount != queryLog.selectLinesByIdentificatorsCount) return false;
			if (selectAllAttachmentsInOrder != queryLog.selectAllAttachmentsInOrder) return false;
			if (selectAttachmentIdentificatorsInLine != queryLog.selectAttachmentIdentificatorsInLine) return false;
			if (selectAttachmentsByIdentificators != queryLog.selectAttachmentsByIdentificators) return false;
			if (attachmentInsert != queryLog.attachmentInsert) return false;
			if (selectAllPartiesInOrder != queryLog.selectAllPartiesInOrder) return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as QueryLog);
		}

		public override int GetHashCode()
		{
			int result = selectOrderByIdCount;

			result = 29 * result + orderUpdateCount;
			result = 29 * result + orderInsertCount;
			result = 29 * result + orderDeleteCount;
			result = 29 * result + selectLineIdentificatorsInOrderCount;
			result = 29 * result + selectLineByIdCount;
			result = 29 * result + selectLineIdentificatorsOnPageCount;
			result = 29 * result + lineUpdateCount;
			result = 29 * result + lineInsertCount;
			result = 29 * result + lineDeleteCount;
			result = 29 * result + selectInOrderCount;
			result = 29 * result + partyInsertCount;
			result = 29 * result + partySelectCount;
			result = 29 * result + partyUpdateCount;
			result = 29 * result + selectLinesByIdentificatorsCount;
			result = 29 * result + selectAllAttachmentsInOrder;
			result = 29 * result + selectAttachmentIdentificatorsInLine;
			result = 29 * result + selectAttachmentsByIdentificators;
			result = 29 * result + attachmentInsert;
			result = 29 * result + selectAllPartiesInOrder;

			return result;
		}

		public QueryLog SelectAllPartiesInOrder()
		{
			selectAllPartiesInOrder++;

			return this;
		}
	}
}