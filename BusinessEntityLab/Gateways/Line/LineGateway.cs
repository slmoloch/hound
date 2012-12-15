using System.Collections.Generic;
using BLToolkit.Data;
using BLToolkit.DataAccess;

namespace Gateways.Line
{
	internal class LineGateway : ILineGateway
	{
		private readonly DbManager manager;
		private readonly QueryLog log;


		public LineGateway(DbManager manager, QueryLog log)
		{
			this.manager = manager;
			this.log = log;
		}

		public IList<int> SelectIdentificatorsInOrder(int orderId)
		{
			log.SelectLineIdentificatorsInOrder();

			return manager.SetCommand("Select LineId from Line Where OrderId = @OrderId", manager.Parameter("@OrderId", orderId)).
				ExecuteScalarList<int>();
		}

		public IList<LineRow> SelectInOrder(int orderId)
		{
			log.SelectAllLinesInOrder();

			return manager
				.SetCommand("Select * from Line Where OrderId = @OrderId", manager.Parameter("@OrderId", orderId))
				.ExecuteList<LineRow>();
		}

		public IList<LineRow> SelectByIdentificators(IList<int> identificators)
		{
			log.SelectLinesByIdentificators();

			string sql = "Select * from Line Where LineId in (";

			foreach (int i in identificators)
			{
				sql += i + ",";
			}

			sql += "-1)";


			return manager
				.SetCommand(sql)
				.ExecuteList<LineRow>();
		}

		public LineRow SelectById(int lineId)
		{
			log.SelectLineById();
			return new SqlQuery<LineRow>(manager).SelectByKey(lineId);
		}

		public IList<int> SelectIdentificatorsOnPage(int startRow, int pageSize, string sortExpression, int orderId)
		{
			log.SelectLineIdentificatorsOnPage();

			return manager.SetCommand("Select TOP " + pageSize + " LineId from Line Where OrderId = @OrderId", manager.Parameter("@OrderId", orderId)).
				ExecuteScalarList<int>();
		}

		public void Update(LineRow line)
		{
			log.LineUpdate();

			new SqlQuery<LineRow>(manager).Update(line);
		}

		public void Insert(LineRow line)
		{
			log.LineInsert();

			line.LineId = manager
					.SetCommand("Insert into [Line] (Number, OrderId) Values (@Number, @OrderId); select @@Identity;", manager.CreateParameters(line))
					.ExecuteScalar<int>();
		}

		public void Delete(int lineId)
		{
			log.LineDelete();

			manager.SetCommand("Delete from Line Where LineId = @LineId", manager.Parameter("@LineId", lineId)).
					 ExecuteNonQuery();
		}
	}
}