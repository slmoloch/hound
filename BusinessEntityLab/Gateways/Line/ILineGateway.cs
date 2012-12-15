using System.Collections.Generic;

namespace Gateways.Line
{
	public interface ILineGateway
	{
		IList<int> SelectIdentificatorsInOrder(int orderId);
		LineRow SelectById(int lineId);
		IList<int> SelectIdentificatorsOnPage(int startRow, int pageSize, string sortExpression, int orderId);
		void Update(LineRow line);
		void Insert(LineRow line);
		void Delete(int lineId);
		IList<LineRow> SelectInOrder(int orderId);
		IList<LineRow> SelectByIdentificators(IList<int> identificators);
	}
}