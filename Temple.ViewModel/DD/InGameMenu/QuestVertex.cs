using Craft.DataStructures.Graph;
using Temple.Application.Interfaces;

namespace Temple.ViewModel.DD.InGameMenu;

public class QuestVertex : LabelledVertex
{
    public QuestVertex(
        string label,
        Quest quest) : base(label)
    {
        Quest = quest;
    }

    public Quest Quest { get; }
}