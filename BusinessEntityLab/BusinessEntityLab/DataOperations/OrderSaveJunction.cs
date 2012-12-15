using Gateways;
using Gateways.Line;
using Gateways.Order;
using Gateways.Party;

namespace BusinessEntityLab.DataOperations
{
    public class OrderSaveJunction : IOrderSaveJunction
    {
        private readonly GatewayManager manager;

        public OrderSaveJunction(GatewayManager manager)
        {
            this.manager = manager;
        }

        public void UpdateLine(LineRow row)
        {
            manager.Line.Update(row);
        }

        public void InsertLine(LineRow row)
        {
            manager.Line.Insert(row);
        }

        public void DeleteLine(int lineId)
        {
            manager.Line.Delete(lineId);
        }

        public void DeleteOrder(int orderId)
        {
            manager.Order.Delete(orderId);
        }

        public void UpdateOrder(OrderRow orderRow)
        {
            manager.Order.Update(orderRow);
        }

        public void InsertOrder(OrderRow orderRow)
        {
            manager.Order.Insert(orderRow);
        }

        public void UpdateParty(PartyRow partyRow)
        {
            manager.Party.Update(partyRow);
        }

        public void InsertParty(PartyRow partyRow)
        {
            manager.Party.Insert(partyRow);
        }
    }
}