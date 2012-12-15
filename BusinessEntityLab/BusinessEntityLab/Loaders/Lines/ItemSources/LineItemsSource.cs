using System.Collections.Generic;
using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Mappers;
using BusinessEntityMappers;

namespace BusinessEntityLab.Loaders.Lines.ItemSources
{
	public class LineItemsSource : IBusinessCollectionItemsSource<Line>
	{
		private readonly int orderId;
		private readonly IOrderDataJunction orderDataJunction;
		private readonly LineMapper mapper;

		public LineItemsSource(int orderId, IOrderDataJunction orderDataJunction, LineMapper mapper)
		{
			this.orderId = orderId;
			this.orderDataJunction = orderDataJunction;
			this.mapper = mapper;
		}

		public IList<int> SelectPageIdentificators(int startRow, int pageSize, string sortExpression)
		{
			return orderDataJunction.SelectLineIdentificatorsOnPage(startRow, pageSize, sortExpression, orderId);
		}

		public IList<Line> LoadAllEntities()
		{
			IList<int> identificators = orderDataJunction.SelectLineIdentificatorsInOrder(orderId);

			IList<Line> lines = new List<Line>();

			foreach (int identificator in identificators)
			{
				lines.Add(mapper.Load(identificator));
			}
			
			return lines;
		}
	}
}