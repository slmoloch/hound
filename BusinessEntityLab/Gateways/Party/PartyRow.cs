using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace Gateways.Party
{
	[TableName("Party")]
	public class PartyRow
	{
		private int id;
		private string name;

		public PartyRow()
		{
		}

		public PartyRow(string name)
		{
			this.name = name;
		}

		public PartyRow(int id, string name)
		{
			this.name = name;
			this.id = id;
		}

		[MapField("PartyId"), PrimaryKey, NonUpdatable]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[MapField("Name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
