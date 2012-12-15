using System.Reflection;
using BusinessEntityLab;
using BusinessEntityLab.Entities;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace BusinessEntityMappers.Test
{
    [TestFixture]
    public class ReflectionTest
    {
        [Test]
        public void PropertyAccessTest()
        {
            Order order = MapperFactory.GetLazyOrderMapper(null).Create();

            order.PurchaseOrderNumber = "lama";

            PropertyInfo[] properties = order.GetType().GetProperties();

            Assert.That(order.GetType().GetProperty("PurchaseOrderNumber").GetValue(order, null), Is.EqualTo("lama"));
        }
    }
}
