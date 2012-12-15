using System.Collections.Generic;
using BusinessEntityLab;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Mappers;
using BusinessEntityMappers;
using Gateways;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Tests
{
    [TestFixture]
    public class FullyLoadedOrderTests
    {

        private GatewayManager manager;
        private BusinessEntityMapper<Order> orderMapper;
        private BusinessEntitySaveMapper<Order> orderSaveMapper;
        private BusinessEntityMapper<Line> lineMapper;
        private PartyMapper partyMapper;
        private LogAssertions log;
        private TestStorage storage;

        [SetUp]
        public void SetUp()
        {
            manager = new GatewayManager();
            orderMapper = MapperFactory.GetPreLoadedOrderMapper(manager);
            orderSaveMapper = MapperFactory.GetSaveOrderMapper(manager);
            lineMapper = MapperFactory.GetLineMapper(manager);
            partyMapper = MapperFactory.GetPreLoadedPartyMapper(manager);

            storage = new TestStorage();

            log = new LogAssertions(manager.Log);
        }

        [TearDown]
        public void TearDown()
        {
            manager.Dispose();
        }

        [Test]
        public void SelectSinglePoTest()
        {
            log.NothingIsExecuted();

            Order po = orderMapper.Load(storage.OrderId);

            log.ExecutedAllSelectOrderOperations();

            Assert.That(po.OrderId, Is.EqualTo(storage.OrderId));
            Assert.That(po.PurchaseOrderNumber, Is.EqualTo("lama"));
            Assert.That(po.BillToParty.Name, Is.EqualTo("party1"));
            Assert.That(po.SupplierParty, Is.Null);

            Assert.That(po.Lines.Count, Is.EqualTo(20));

            Line firstLine = po.Lines.SelectById(storage.FirstLineId);
            Assert.That(firstLine.LineNumber, Is.EqualTo("lama1"));
            Assert.That(firstLine.Attachments.Count, Is.EqualTo(2));
            Assert.That(firstLine.Attachments.SelectById(storage.FirstAttachmentId).FileName, Is.EqualTo("fileName1"));

            foreach (Line line in po.Lines)
            {
                Assert.That(line.LineNumber, Is.Not.Null);
            }

            log.NothingIsExecuted();
        }

        [Test]
        public void SelectSingleUnexistedPoTest()
        {
            Order po = orderMapper.Load(-1);
            Assert.That(po, Is.Null);
        }

        [Test]
        public void SelectLinesWithPagingTest()
        {
            Order po = orderMapper.Load(storage.OrderId);

            log.Reset();

            IList<Line> list = po.Lines.SelectWithPaging(2, 3, "OrderId asc");
            log.ExecutedSelectWithPaging();

            Assert.That(list.Count, Is.EqualTo(3));

            foreach (Line line in list)
            {
                Assert.That(line.LineNumber, Is.Not.Empty);
                Assert.That(po.Lines.SelectById(line.LineId), Is.EqualTo(line));
            }

            log.NothingIsExecuted();
        }

        [Test]
        public void AddLine()
        {
            Order po = orderMapper.Load(storage.OrderId);

            log.Reset();

            Assert.That(po.Lines.Count, Is.EqualTo(20));
            po.Lines.Add(lineMapper.Create());
            Assert.That(po.Lines.Count, Is.EqualTo(21));

            log.NothingIsExecuted();
        }

        [Test]
        public void AddLineAndSave()
        {
            Order po = orderMapper.Load(storage.OrderId);

            Line newLine = lineMapper.Create();
            newLine.LineNumber = "Lama11";

            log.Reset();

            po.Lines.Add(newLine);
            Assert.That(po.Lines.Count, Is.EqualTo(21));

            log.NothingIsExecuted();

            orderSaveMapper.Store(po);
            Assert.That(po.Lines.Count, Is.EqualTo(21));

            log.ExecutedOnlyLineInsert();

            Order loaded = orderMapper.Load(po.OrderId);
            AssertOrdersAreEqual(loaded, po);
        }

        [Test]
        public void DeleteLine()
        {
            Order po = orderMapper.Load(storage.OrderId);

            log.Reset();

            Assert.That(po.Lines.Count, Is.EqualTo(20));
            po.Lines.Remove(po.Lines.SelectById(storage.FirstLineId));
            Assert.That(po.Lines.Count, Is.EqualTo(19));

            log.NothingIsExecuted();
        }

        [Test]
        public void DeleteLineAndSave()
        {
            Order po = orderMapper.Load(storage.OrderId);

            Assert.That(po.Lines.Count, Is.EqualTo(20));

            po.Lines.Remove(po.Lines.SelectById(storage.FirstLineId));
            Assert.That(po.Lines.Count, Is.EqualTo(19));

            orderSaveMapper.Store(po);

            AssertOrdersAreEqual(orderMapper.Load(po.OrderId), po);
        }

        [Test]
        public void StoreOnlyMainEntity()
        {
            Order po = orderMapper.Load(storage.OrderId);

            log.Reset();

            po.PurchaseOrderNumber = "new Number";

            orderSaveMapper.Store(po);

            log.ExecutedOnlyOrderUpdate();
        }

        [Test]
        public void StoreOnlyNestedEntity()
        {
            Order po = orderMapper.Load(storage.OrderId);

            log.Reset();

            po.BillToParty.Name = "new party name";

            orderSaveMapper.Store(po);

            log.ExecutedOrderAndPartyUpdate();
        }

        [Test]
        public void StoreOnlyOneItemInCollection()
        {
            Order po = orderMapper.Load(storage.OrderId);

            log.Reset();

            po.Lines.SelectById(storage.FirstLineId).LineNumber = "new line name";

            orderSaveMapper.Store(po);

            log.ExecutedOnlyLineUpdate();
        }

        [Test]
        public void DeleteOneItemInCollection()
        {
            Order po = orderMapper.Load(storage.OrderId);

            Line lineToDelete = po.Lines.SelectById(storage.FirstLineId);

            log.Reset();

            po.Lines.Remove(lineToDelete);

            log.NothingIsExecuted();

            orderSaveMapper.Store(po);

            log.ExecutedOnlyLineDelete();
        }

        [Test]
        public void InsertEmptyOrder()
        {
            Order po = orderMapper.Create();

            po.PurchaseOrderNumber = "Lama";

            log.NothingIsExecuted();

            orderSaveMapper.Store(po);

            log.OnlyOrderWasInserted();

            AssertOrdersAreEqual(orderMapper.Load(po.OrderId), po);
        }

        [Test]
        public void InsertOrderWithNestedEntities()
        {
            Order po = orderMapper.Create();
            Party billToParty = partyMapper.Create();

            billToParty.Name = "Party";

            po.PurchaseOrderNumber = "Lama";
            po.BillToParty = billToParty;

            log.NothingIsExecuted();

            orderSaveMapper.Store(po);

            log.OrderAndPartyWereInserted();

            AssertOrdersAreEqual(orderMapper.Load(po.OrderId), po);
        }

        [Test]
        public void InsertOrderWithLine()
        {
            Order po = orderMapper.Create();
            po.PurchaseOrderNumber = "Lama";

            Line line = lineMapper.Create();
            line.LineNumber = "Lama1";

            po.Lines.Add(line);

            log.NothingIsExecuted();

            orderSaveMapper.Store(po);

            log.OrderAndLineWereInserted();

            AssertOrdersAreEqual(orderMapper.Load(po.OrderId), po);
        }

        [Test]
        public void DeleteOrder()
        {
            Order po = orderMapper.Load(storage.OrderId);

            Assert.That(po, Is.Not.Null);
            orderSaveMapper.Delete(po);

            Order po1 = orderMapper.Load(storage.OrderId);
            Assert.That(po1, Is.Null);
        }

        [Test]
        public void MultiOrderSingleMapper()
        {
            Order po1 = orderMapper.Load(storage.OrderId);
            log.ExecutedAllSelectOrderOperations();

            Order po2 = orderMapper.Load(storage.SecondOrderId);
            log.ExecutedAllSelectOrderOperations();

            Assert.That(po1.PurchaseOrderNumber, Is.EqualTo("lama"));
            Assert.That(po2.PurchaseOrderNumber, Is.EqualTo("order2"));
        }

        private static void AssertOrdersAreEqual(Order order, Order expected)
        {
            Assert.That(order.OrderId, Is.EqualTo(expected.OrderId));
            Assert.That(order.PurchaseOrderNumber, Is.EqualTo(expected.PurchaseOrderNumber));

            if (expected.BillToParty != null)
            {
                Assert.That(order.BillToParty, Is.Not.Null);
                Assert.That(order.BillToParty.Name, Is.EqualTo(expected.BillToParty.Name));
                Assert.That(order.BillToParty.PartyId, Is.EqualTo(expected.BillToParty.PartyId));
            }
            else
            {
                Assert.That(order.BillToParty, Is.Null);
            }

            Assert.That(order.Lines.Count, Is.EqualTo(expected.Lines.Count));

            foreach (Line expectedLine in expected.Lines)
            {
                Line line = order.Lines.SelectById(expectedLine.LineId);

                Assert.That(line, Is.Not.Null);
                Assert.That(line.LineId, Is.EqualTo(expectedLine.LineId));
                Assert.That(line.LineNumber, Is.EqualTo(expectedLine.LineNumber));
                Assert.That(line.ParentId, Is.EqualTo(expectedLine.ParentId));
            }
        }
    }
}
