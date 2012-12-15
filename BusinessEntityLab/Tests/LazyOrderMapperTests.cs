using System.Collections.Generic;
using BusinessEntityLab;
using BusinessEntityLab.Entities;
using BusinessEntityMappers;
using BusinessEntityMappers.Exceptions;
using Gateways;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Tests
{
	[TestFixture]
	public class LazyOrderMapperTests
	{
		private int orderId;
		private GatewayManager manager;
		private BusinessEntityMapper<Order> lazyOrderMapper;
		private LogAssertions log;
		private TestStorage storage;

		[SetUp]
		public void SetUp()
		{
			manager = new GatewayManager();
			lazyOrderMapper = MapperFactory.GetLazyOrderMapper(manager);

			storage = new TestStorage();

			orderId = storage.OrderId;

			log = new LogAssertions(manager.Log);
		}

		[TearDown]
		public void TearDown()
		{
			manager.Dispose();
		}

		[Test]
		public void SelectSingleLazyPoTest()
		{
			log.NothingIsExecuted();

			Order po = lazyOrderMapper.Load(orderId);

			log.NothingIsExecuted();

			Assert.That(po.OrderId, Is.EqualTo(storage.OrderId));
			Assert.That(po.PurchaseOrderNumber, Is.EqualTo("lama"));
			log.OnlySelectOrderById();

			Assert.That(po.PurchaseOrderNumber, Is.EqualTo("lama"));
			log.NothingIsExecuted();
		}

		[Test, ExpectedException(typeof(LoadUnexistedEntityException))]
		public void SelectSingleLazyUnexistedPoTest()
		{
			Order po = lazyOrderMapper.Load(-1);
			Assert.That(po, Is.Not.Null);

			log.NothingIsExecuted();

			po.PurchaseOrderNumber.TrimEnd();
		}

		[Test]
		public void AccessToNestedEntity()
		{
			Order po = lazyOrderMapper.Load(orderId);

			log.NothingIsExecuted();

			Assert.That(po.BillToParty.Name, Is.EqualTo("party1"));

			log.OnlySelectOrderByIdAndSelectPartyById();
		}

		[Test]
		public void SelectLinesWithPagingTest()
		{
			Order po = lazyOrderMapper.Load(orderId);
			log.NothingIsExecuted();

			IList<Line> list = po.Lines.SelectWithPaging(2, 3, "OrderId asc");
			log.ExecutedSelectWithPagingAndSelectByIdentificators();

			Assert.That(list.Count, Is.EqualTo(3));
			foreach (Line line in list)
			{
				Assert.That(line.LineNumber, Is.Not.Empty);
				Assert.That(po.Lines.SelectById(line.LineId), Is.EqualTo(line));
			}

			log.NothingIsExecuted();
		}

		[Test]
		public void ListEnumeration()
		{
			Order po = lazyOrderMapper.Load(orderId);
			log.NothingIsExecuted();
			
			foreach (Line line in po.Lines)
			{
				Assert.That(line.LineNumber, Is.Not.Empty);
				Assert.That(po.Lines.SelectById(line.LineId), Is.EqualTo(line));
			}

			log.ExecutedGetIdentificatorsAndGetLines();
		}
	}
}
