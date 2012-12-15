using Gateways.Line;

namespace BusinessEntityLab.DataOperations
{
    /// <summary>
    /// Line data junction.
    /// </summary>
    public interface ILineDataJunction : IAttachmentDataJunction
    {
        /// <summary>
        /// Determines whether [is line exists] [the specified line id].
        /// </summary>
        /// <param name="lineId">The line id.</param>
        /// <returns>
        /// 	<c>true</c> if [is line exists] [the specified line id]; otherwise, <c>false</c>.
        /// </returns>
        bool IsLineExists(int lineId);

        /// <summary>
        /// Selects the line by id.
        /// </summary>
        /// <param name="lineId">The line id.</param>
        /// <returns></returns>
        LineRow SelectLineById(int lineId);
    }
}
