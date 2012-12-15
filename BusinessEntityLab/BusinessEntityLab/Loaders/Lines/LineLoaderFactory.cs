using BusinessEntityLab.DataOperations;
using BusinessEntityLab.Entities;
using BusinessEntityLab.Loaders.Lines;
using BusinessEntityMappers;

namespace BusinessEntityLab.Loaders.Lines
{
	public class LineLoaderFactory : ILoaderFactory<Line>
	{
		private readonly ILineDataJunction lineDataJunction;

		public LineLoaderFactory(ILineDataJunction lineDataJunction)
		{
			this.lineDataJunction = lineDataJunction;
		}

		public IEntityLoader<Line> GetEntityLoader(int id)
		{
			return new LineLoader(id, lineDataJunction);
		}
	}
}