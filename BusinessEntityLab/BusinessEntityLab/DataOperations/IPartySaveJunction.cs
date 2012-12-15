using Gateways.Party;

namespace BusinessEntityLab.DataOperations
{
	public interface IPartySaveJunction
	{
		/// <summary>
		/// Updates the party.
		/// </summary>
		/// <param name="partyRow">The party row.</param>
		void UpdateParty(PartyRow partyRow);

		/// <summary>
		/// Inserts the party.
		/// </summary>
		/// <param name="partyRow">The party row.</param>
		void InsertParty(PartyRow partyRow);
	}
}
