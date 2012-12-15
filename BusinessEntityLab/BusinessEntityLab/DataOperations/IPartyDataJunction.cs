using Gateways.Party;

namespace BusinessEntityLab.DataOperations
{
	/// <summary>
	/// Party data junction.
	/// </summary>
	public interface IPartyDataJunction
	{
		/// <summary>
		/// Determines whether [is party exists] [the specified party id].
		/// </summary>
		/// <param name="partyId">The party id.</param>
		/// <returns>
		/// 	<c>true</c> if [is party exists] [the specified party id]; otherwise, <c>false</c>.
		/// </returns>
		bool IsPartyExists(int partyId);

		/// <summary>
		/// Selects the party by id.
		/// </summary>
		/// <param name="partyId">The party id.</param>
		/// <returns></returns>
		PartyRow SelectPartyById(int partyId);
	}
}
