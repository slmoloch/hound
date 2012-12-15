using System;
using BLToolkit.Data;
using Gateways.Attachment;
using Gateways.Line;
using Gateways.Order;
using Gateways.Party;

namespace Gateways
{
	public class GatewayManager : IDisposable
	{
		private readonly DbManager manager;
		private readonly QueryLog log;

		public GatewayManager()
		{
			manager = new DbManager();
			log = new QueryLog();
		}

		public IOrderGateway Order
		{
			get { return new OrderGateway(manager, log); }
		}

		public ILineGateway Line
		{
			get { return new LineGateway(manager, log); }
		}

		public IPartyGateway Party
		{
			get { return new PartyGateway(manager, log);}
		}

		public IAttachmentGateway Attachment
		{
			get { return new AttachmentGateway(manager, log);}
		}

		public QueryLog Log
		{
			get { return log; }
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			manager.Dispose();
		}
	}
}
