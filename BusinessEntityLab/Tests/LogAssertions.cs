using Gateways;
using NUnit.Framework;

namespace Tests
{
	public class LogAssertions
	{
		private readonly QueryLog log;

		public LogAssertions(QueryLog log)
		{
			this.log = log;
		}

		public void OnlySelectOrderById()
		{
			QueryLog expectedLog = new QueryLog()
				.SelectOrderById();

			AssertLog(expectedLog);
		}

		public void NothingIsExecuted()
		{
			AssertLog(new QueryLog());
		}

		public void ExecutedAllSelectOrderOperations()
		{
			QueryLog expectedLog = new QueryLog()
				.SelectOrderById()
				.SelectAllLinesInOrder()
				.SelectAllPartiesInOrder()
				.SelectAllAttachmentsInOrder();

			AssertLog(expectedLog);
		}

		public void ExecutedSelectWithPaging()
		{
			QueryLog expectedLog = new QueryLog()
				.SelectLineIdentificatorsOnPage();

			AssertLog(expectedLog);
		}

		public void OnlySelectOrderByIdAndSelectPartyById()
		{
			QueryLog expectedLog = new QueryLog()
				.SelectOrderById()
				.PartySelect();

			AssertLog(expectedLog);
		}

		public void ExecutedOnlyOrderUpdate()
		{
			QueryLog expectedLog = new QueryLog()
				.OrderUpdate();

			AssertLog(expectedLog);
		}

		public void ExecutedOrderAndPartyUpdate()
		{
			QueryLog expectedLog = new QueryLog()
				.OrderUpdate()
				.PartyUpdate();

			AssertLog(expectedLog);
		}

		public void ExecutedOnlyLineUpdate()
		{
			QueryLog expectedLog = new QueryLog()
				.LineUpdate();

			AssertLog(expectedLog);
		}

		public void ExecutedOnlyLineInsert()
		{
			QueryLog expectedLog = new QueryLog()
				.LineInsert();

			AssertLog(expectedLog);
		}

		public void ExecutedOnlyLineDelete()
		{
			QueryLog expectedLog = new QueryLog()
				.LineDelete();

			AssertLog(expectedLog);
		}

		public void OnlyOrderWasInserted()
		{
			QueryLog expectedLog = new QueryLog()
				.OrderInsert();

			AssertLog(expectedLog);
		}

		public void OrderAndPartyWereInserted()
		{
			QueryLog expectedLog = new QueryLog()
				.OrderInsert()
				.PartyInsert();

			AssertLog(expectedLog);
		}

		public void OrderAndLineWereInserted()
		{
			QueryLog expectedLog = new QueryLog()
				.OrderInsert()
				.LineInsert();

			AssertLog(expectedLog);
		}

		public void ExecutedSelectWithPagingAndSelectByIdentificators()
		{
			QueryLog expectedLog = new QueryLog()
				.SelectLineIdentificatorsOnPage()
				.SelectLineIdentificatorsInOrder()
                .SelectLinesByIdentificators();

			AssertLog(expectedLog);
		}

		public void Reset()
		{
			log.Reset();
		}

		private void AssertLog(QueryLog expectedLog)
		{
			Assert.AreEqual(expectedLog, log, log.GetStringRepresentation());

			log.Reset();
		}

		public void ExecutedGetIdentificatorsAndGetLines()
		{
			QueryLog expectedLog = new QueryLog()
				.SelectLineIdentificatorsInOrder()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById()
				.SelectLineById();

			AssertLog(expectedLog);
		}
	}
}