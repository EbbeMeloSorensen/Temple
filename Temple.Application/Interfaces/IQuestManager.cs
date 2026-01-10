namespace Temple.Application.Interfaces;

/// <summary>
/// Defines functionality for managing and retrieving quests within the game.
/// </summary>
/// <remarks>The <see cref="IQuestManager"/> interface provides methods to access all quests and to query quest
/// relationships, such as retrieving quests that follow a given quest in a quest tree. Implementations may vary in how
/// quests are stored or retrieved, but all expose consistent methods for quest navigation and enumeration.</remarks>
public interface IQuestManager
{
    /// <summary>
    /// Retrieves all quests in the game.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{Quest}"/> containing all quests.</returns>
    IEnumerable<Quest> GetAllQuests();

    /// <summary>
    /// Returns a collection of quests that directly follow the specified quest in the quest tree.
    /// </summary>
    /// <param name="quest">The quest for which to retrieve subsequent quests. Cannot be <c>null</c>.</param>
    /// <returns>An enumerable collection of <see cref="Quest"/> objects that are immediate successors of the specified quest.
    /// Returns an empty collection if there are no subsequent quests.</returns>
    IEnumerable<Quest> GetSubsequentQuests(Quest quest);
}