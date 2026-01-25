using Craft.DataStructures.Graph;
using Temple.Application.Core;
using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSession : IDialogueSession
{
    private QuestEventBus _eventBus;
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> _graph;
    private int _activeVertexId;

    public string CurrentNPCText => ((DialogueVertex) _graph.GetVertex(_activeVertexId)).Text;

    public IReadOnlyList<DialogueChoice> AvailableChoices
    {
        get
        {
            return _graph.OutgoingEdges(_activeVertexId)
                .Select(_ => new DialogueChoice
                {
                    Id = _.VertexId2,
                    Text = ((LabelledEdge)_).Label
                })
                .ToList();
        }
    }

    public DialogueSession(
        QuestEventBus eventBus)
    {
        _eventBus = eventBus;

        var vertices = new List<DialogueVertex>
        {
            new("Beautiful song, isn't it? I've heard her sing a hundred times, and each time, it still moves me."),
            new("It's the spirit of an elven woman; she haunts this tavern, singing once every couple of nights."),
            new("No one truly knows. Her spirit was here when I first bought this tavern. Some say she sings for a lost love, a soldier who died defending Baldur's Gate. They say she sings in the hope he will her her voice and return home. Still, that's nothing but hearsay and tales - welcome to the Elfsong Tavern. What can I get you?"),
            new("That's a stuffed beholder... a small version of the species, I'm told, not that I've seen many of them. One of my regulars, Ethon, found it in the cellar."),
            new("They're also called eye tyrants, if that name's any more familiar to you. Beholders are beasts that float above the ground and can cast terrible spells from their eyes. Evil things... I wouldn't want to meet one, and neither would you."),
            new("Hmmm. Sounds like members of that new thieves' guild I've been hearing about. You're lucky to be alive. Word is, they're responsible for the murder of two city watchmen and the \"disappearance\" of several thieves from the old guild."),
            new("Yes... Look, I wouldn't cross blades with those thugs if I were you. Just stay clear of them unless you want to end up dead in an alley, all right?"),
            new("Well, no one knows where their guild hall is. Still, if you're determined to find them, try the sewers."),
            new("I'' wager they've been using them to move around Baldur's Gate. It's probably what's been driving all those sewer rats up to the surface."),
            new("Well, there's a gate to the sewers in the cellar of this tavern; I locked it up a long time ago, before the guild war began."),
            new("Well, there's a problem with that... hmmm, Actually, maybe we can help each other out."),
            new()
            {
                Text = "Well, we've had to lock up the cellar because of the horde of rats that suddenly showed up down there. Clear them out for me, and I'll give you the key to the sewer gate and a little gold to help you get back on your feet. What do you say?",
                GameEventTrigger = new QuestDiscoveredEventTrigger("rat_infestation")
            },
            new("The door to the cellar's locked, so you'll need to get the key from Ethon over in the corner there."),
            new("Ethon's one of our regulars. He usually fetches wine from the cellar for me, but he hasn't been able to go down there since the rats appeared."),
            new("Only this past week. If those thieves are using the sewers to move around Baldur's Gate, they may have driven the rats out."),
            new()
            {
                Text = "Luck be with you... and watch those rats. Some of them can be vicious when backed in a corner",
                GameEventTrigger = new QuestAcceptedEventTrigger("rat_infestation")
            },
            new(""),
            new()
            {
                Text = "What an ignorant! Wanna kill rats?",
                GameEventTrigger = new QuestDiscoveredEventTrigger("rat_infestation")
            },
            new("Ok, then get out of my inn, rat lover!")
        };

        _graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        _graph.AddEdge(new LabelledEdge(0, 1, "It is beautiful... but where's the voice coming from?"));
        _graph.AddEdge(new LabelledEdge(0, 17, "I think it sounds rather lame"));
        _graph.AddEdge(new LabelledEdge(1, 2, "Why does she sing?"));
        _graph.AddEdge(new LabelledEdge(2, 3, "What's that thing hanging over the fireplace?"));
        _graph.AddEdge(new LabelledEdge(3, 4, "What's a beholder?"));
        _graph.AddEdge(new LabelledEdge(2, 5, "I was hoping you could help me. I was robbed on the streets by a band of thieves, and I'm looking to find them."));
        _graph.AddEdge(new LabelledEdge(3, 5, "Actually, I was hoping you could help me. I was robbed on the streets by a band of thieves, and I'm looking to find them."));
        _graph.AddEdge(new LabelledEdge(4, 5, "I see. Look, I came here because I was attacked on the streets by a band of thieves, and I'm looking to find them."));
        _graph.AddEdge(new LabelledEdge(5, 6, "So this new guild's at war with the old guild?"));
        _graph.AddEdge(new LabelledEdge(6, 7, "I don't have much choice; they stole every last coin I had. If you know where I can find them, tell me."));
        _graph.AddEdge(new LabelledEdge(5, 7, "Do you know where I can find these thieves?"));
        _graph.AddEdge(new LabelledEdge(7, 8, "Why the sewers?"));
        _graph.AddEdge(new LabelledEdge(8, 9, "All right. How do I get into the sewers?"));
        _graph.AddEdge(new LabelledEdge(9, 10, "Could I use the gate?"));
        _graph.AddEdge(new LabelledEdge(10, 11, "What do you mean?"));
        _graph.AddEdge(new LabelledEdge(11, 12, "It's a deal."));
        _graph.AddEdge(new LabelledEdge(12, 13, "Who's Ethon"));
        _graph.AddEdge(new LabelledEdge(12, 15, "I'll go speak to him, then."));
        _graph.AddEdge(new LabelledEdge(13, 14, "When did they start appearing?"));
        _graph.AddEdge(new LabelledEdge(13, 15, "I'll go speak to him, then."));
        _graph.AddEdge(new LabelledEdge(14, 15, "I'll go get the key from Ethon and see about taking care of those rats, then."));
        _graph.AddEdge(new LabelledEdge(15, 16, "OK"));
        _graph.AddEdge(new LabelledEdge(17, 18, "No, I think rats are cute"));
        _graph.AddEdge(new LabelledEdge(17, 15, "Sure, why not"));
        _graph.AddEdge(new LabelledEdge(18, 16, "OK"));

        _activeVertexId = 0;
    }

    public void SelectChoice(
        int choiceId)
    {
        _activeVertexId = choiceId;

        switch (((DialogueVertex)_graph.GetVertex(_activeVertexId)).GameEventTrigger)
        {
            case QuestDiscoveredEventTrigger questDiscoveredEventTrigger:
                _eventBus.Publish(new QuestDiscoveredEvent(questDiscoveredEventTrigger.QuestId));
                break;
            case QuestAcceptedEventTrigger questAcceptedEventTrigger:
                _eventBus.Publish(new QuestAcceptedEvent(questAcceptedEventTrigger.QuestId));
                break;
        }
    }

    public bool IsFinished => !AvailableChoices.Any();
}