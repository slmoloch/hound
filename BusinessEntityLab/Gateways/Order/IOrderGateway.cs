namespace Gateways.Order
{
	public interface IOrderGateway
	{
		OrderRow SelectById(int id);
		void Update(OrderRow order);
		void Insert(OrderRow order);
	    void Delete(int id);
	}
}