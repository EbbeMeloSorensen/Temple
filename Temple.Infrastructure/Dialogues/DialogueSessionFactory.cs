using Craft.DataStructures.Graph;
using Temple.Application.Core;
using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSessionFactory : IDialogueSessionFactory
{
    private IQuestStatusReadModel _questStatusReadModel;

    public IDialogueSession GetDialogueSession(
        IQuestStatusReadModel questStatusReadModel,
        QuestEventBus eventBus,
        string npcId)
    {
        _questStatusReadModel = questStatusReadModel;

        return new DialogueSession(eventBus, npcId, GenerateGraph_Dialogue(npcId));
    }

    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Dialogue(
        string npcId)
    {
        // Dette skal læses fra fil på lidt længere sigt
        var dialogueGraphCollection = npcId switch
        {
            "innkeeper" => GetDialogueGraphCollectionForInnKeeper(),
            "captain" => GetDialogueGraphCollectionForCaptain(),
            _ => throw new InvalidOperationException("Unknown npc id")
        };

        // Filtrer de grafer fra, som ikke kvalificerer
        dialogueGraphCollection = new DialogueGraphCollection(
            dialogueGraphCollection.DialogueGraphs.Where(DialogueGraphMeetsConditions));

        // Returner den af de kvalificerende grafer, som har den højeste prioritet
        var dialogueGraph = dialogueGraphCollection.DialogueGraphs
            .OrderByDescending(_ => _.Priority)
            .First();

        return dialogueGraph.Graph;
    }

    private DialogueGraphCollection GetDialogueGraphCollectionForInnKeeper()
    {
        var dialogueGraphs = new List<DialogueGraph>
        {
            new()
            {
                Priority = 100.0,
                Conditions = new List<DialogueGraphCondition>
                {
                    new()
                    {
                        QuestId = "rat_infestation",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = true
                        }
                    }
                },
                Graph = GenerateGraph_Innkeeper_RatQuestTurnIn()
            },
            new()
            {
                Priority = 100.0,
                Conditions = new List<DialogueGraphCondition>
                {
                    new()
                    {
                        QuestId = "rat_infestation",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = false
                        }
                    }
                },
                Graph = GenerateGraph_Innkeeper_RatQuestActive()
            },
            new()
            {
                Priority = 100.0,
                Conditions = new List<DialogueGraphCondition>
                {
                    new()
                    {
                        QuestId = "rat_infestation",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Available,
                            AreCompletionCriteriaSatisfied = false
                        }
                    }
                },
                Graph = GenerateGraph_Innkeeper_RatQuestAvailable()
            },
            new()
            {
                Priority = 100.0,
                Conditions = new List<DialogueGraphCondition>
                {
                    new()
                    {
                        QuestId = "rat_infestation",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Hidden,
                            AreCompletionCriteriaSatisfied = false
                        }
                    }
                },
                Graph = GenerateGraph_Innkeeper_RatQuestHidden()
            },
            new()
            {
                Priority = 0,
                Graph = GenerateGraph_Innkeeper_SmallTalkDialogue()
            }
        };

        return new DialogueGraphCollection(dialogueGraphs);
    }
    private DialogueGraphCollection GetDialogueGraphCollectionForCaptain()
    {
        var dialogueGraphs = new List<DialogueGraph>
        {
            new()
            {
                Priority = 100.0,
                Conditions = new List<DialogueGraphCondition>
                {
                    new()
                    {
                        QuestId = "skeleton_trouble",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Hidden,
                            AreCompletionCriteriaSatisfied = false
                        }
                    }
                },
                Graph = GenerateGraph_Captain_SkeletonQuestHidden()
            },
            new()
            {
                Priority = 0.0,
                Graph = GenerateGraph_Captain_SmallTalkDialogue()
            }
        };

        return new DialogueGraphCollection(dialogueGraphs);
    }

    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Innkeeper_RatQuestHidden()
    {
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

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "It is beautiful... but where's the voice coming from?"));
        graph.AddEdge(new LabelledEdge(0, 17, "I think it sounds rather lame"));
        graph.AddEdge(new LabelledEdge(1, 2, "Why does she sing?"));
        graph.AddEdge(new LabelledEdge(2, 3, "What's that thing hanging over the fireplace?"));
        graph.AddEdge(new LabelledEdge(3, 4, "What's a beholder?"));
        graph.AddEdge(new LabelledEdge(2, 5, "I was hoping you could help me. I was robbed on the streets by a band of thieves, and I'm looking to find them."));
        graph.AddEdge(new LabelledEdge(3, 5, "Actually, I was hoping you could help me. I was robbed on the streets by a band of thieves, and I'm looking to find them."));
        graph.AddEdge(new LabelledEdge(4, 5, "I see. Look, I came here because I was attacked on the streets by a band of thieves, and I'm looking to find them."));
        graph.AddEdge(new LabelledEdge(5, 6, "So this new guild's at war with the old guild?"));
        graph.AddEdge(new LabelledEdge(6, 7, "I don't have much choice; they stole every last coin I had. If you know where I can find them, tell me."));
        graph.AddEdge(new LabelledEdge(5, 7, "Do you know where I can find these thieves?"));
        graph.AddEdge(new LabelledEdge(7, 8, "Why the sewers?"));
        graph.AddEdge(new LabelledEdge(8, 9, "All right. How do I get into the sewers?"));
        graph.AddEdge(new LabelledEdge(9, 10, "Could I use the gate?"));
        graph.AddEdge(new LabelledEdge(10, 11, "What do you mean?"));
        graph.AddEdge(new LabelledEdge(11, 12, "It's a deal."));
        graph.AddEdge(new LabelledEdge(12, 13, "Who's Ethon"));
        graph.AddEdge(new LabelledEdge(12, 15, "I'll go speak to him, then."));
        graph.AddEdge(new LabelledEdge(13, 14, "When did they start appearing?"));
        graph.AddEdge(new LabelledEdge(13, 15, "I'll go speak to him, then."));
        graph.AddEdge(new LabelledEdge(14, 15, "I'll go get the key from Ethon and see about taking care of those rats, then."));
        graph.AddEdge(new LabelledEdge(15, 16, "OK"));
        graph.AddEdge(new LabelledEdge(17, 18, "No, I think rats are cute"));
        graph.AddEdge(new LabelledEdge(17, 15, "Sure, why not"));
        graph.AddEdge(new LabelledEdge(18, 16, "OK"));

        return graph;
    }
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Innkeeper_RatQuestActive()
    {
        var vertices = new List<DialogueVertex>
        {
            new("Have you taken care of those rats yet? I'm not giving you the key to the sewers until they're all wiped out."),
            new("Well, I'd prefer you get to it before they change the name of this tavern to the Rat's Nest. Talk to Ethon if you don't have the key to the cellar yet."),
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "No, not yet."));
        graph.AddEdge(new LabelledEdge(1, 2, "Very well."));

        return graph;
    }
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Innkeeper_RatQuestAvailable()
    {
        var vertices = new List<DialogueVertex>
        {
            new("Hi again, you ignorant rat lover. Do you want to kill them for me after all?"),
            new("Then fuck off, will ya?"),
            new()
            {
                Text = "Ok then, best of luck!",
                GameEventTrigger = new QuestAcceptedEventTrigger("rat_infestation")
            },
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "No. As I said, I think rats are cute"));
        graph.AddEdge(new LabelledEdge(0, 2, "Ok then. I'll kill those critters for you."));
        graph.AddEdge(new LabelledEdge(1, 3, "Ok"));
        graph.AddEdge(new LabelledEdge(2, 3, "Thanks. See you later"));

        return graph;
    }
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Innkeeper_RatQuestTurnIn()
    {
        var vertices = new List<DialogueVertex>
        {
            new("You're back! Did you see Ethon down there?"),
            new("He followed you down there not long ago, and I fear he may have gotten lost in the cellar... or worse, he may have wandered into the sewers. I tried to stop him..."),
            new("Thank you... but before you go, please take these coins -- in payment for all you've done so far. And as promised, here's the key to the sewer gate. Be careful down there - there's bound to be worse things than sewer rats in those tunnels."),
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "In the cellar? No I didn't see him."));
        graph.AddEdge(new LabelledEdge(1, 2, "Don't worry, Alyth.  I've taken care of all the rats, so he probably just got lost. I'll find him."));
        graph.AddEdge(new LabelledEdge(2, 3, "I'll be careful. Thanks, Alyth."));

        return graph;
    }
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Innkeeper_SmallTalkDialogue()
    {
        var vertices = new List<DialogueVertex>
        {
            new("Nice weather today, huh?"),
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "Sure"));

        return graph;
    }

    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Captain_SkeletonQuestHidden()
    {
        var vertices = new List<DialogueVertex>
        {
            new()
            {
                Text = "Hello there. Please slay some skeletons for me, will ya?",
                GameEventTrigger = new QuestDiscoveredEventTrigger("skeleton_trouble")
            },
            new()
            {
                Text = "Great, they are on the graveyard outside of the village. Good luck",
                GameEventTrigger = new QuestAcceptedEventTrigger("skeleton_trouble")
            },
            new("Ok, then fuck off, skeleton lover!"),
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 2, "No, I think skeletons are cute"));
        graph.AddEdge(new LabelledEdge(0, 1, "Sure, why not"));
        graph.AddEdge(new LabelledEdge(1, 3, "OK"));
        graph.AddEdge(new LabelledEdge(2, 3, "OK"));

        return graph;
    }
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Captain_SmallTalkDialogue()
    {
        var vertices = new List<DialogueVertex>
        {
            new("Terrible weather today, huh?"),
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "Nah I think the weather is nice"));

        return graph;
    }

    private bool DialogueGraphMeetsConditions(
        DialogueGraph graph)
    {
        if (graph.Conditions == null || !graph.Conditions.Any())
        {
            return true;
        }

        foreach (var condition in graph.Conditions)
        {
            var status = _questStatusReadModel.GetQuestStatus(condition.QuestId);

            if (!status.Equals(condition.RequiredStatus))
            {
                return false;
            }
        }

        return true;
    }
}