using Craft.DataStructures.Graph;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.DD;

public class QuestVertex : EmptyVertex
{
    public Quest Quest { get; set; }
}