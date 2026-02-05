using Craft.DataStructures.Graph;
using Temple.Application.Core;
using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests;
using Temple.Infrastructure.IO;

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
        var dialogueGraphs = npcId switch
        {
            "alyth" => GetDialogueGraphCollectionForInnKeeper(),
            "captain" => GetDialogueGraphCollectionForCaptain(),
            _ => throw new InvalidOperationException("Unknown npc id")
        };

        // Filtrer de grafer fra, som ikke kvalificerer
        dialogueGraphs = dialogueGraphs.Where(DialogueGraphMeetsConditions);

        // Returner den af de kvalificerende grafer, som har den højeste prioritet
        return dialogueGraphs
            .OrderByDescending(_ => _.Priority)
            .First().Graph;
    }

    private IEnumerable<DialogueGraph> GetDialogueGraphCollectionForInnKeeper()
    {
        var dialogueIO = new DialogueIO();
        var dialogueGraphs = dialogueIO.ReadDialogueGraphListFromFile(@"C:\Temp\dialogueGraphs_alyth.json");
        return dialogueGraphs;
    }
    private IEnumerable<DialogueGraph> GetDialogueGraphCollectionForCaptain()
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
                Priority = 100.0,
                Conditions = new List<DialogueGraphCondition>
                {
                    new()
                    {
                        QuestId = "skeleton_trouble",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Available,
                            AreCompletionCriteriaSatisfied = false
                        }
                    }
                },
                Graph = GenerateGraph_Captain_SkeletonQuestAvailable()
            },
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
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = false
                        }
                    }
                },
                Graph = GenerateGraph_Captain_SkeletonQuestActive()
            },
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
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = true
                        }
                    }
                },
                Graph = GenerateGraph_Captain_SkeletonQuestTurnIn()
            },
            new()
            {
                Priority = 0.0,
                Graph = GenerateGraph_Captain_SmallTalkDialogue()
            }
        };

        return dialogueGraphs;
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
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Captain_SkeletonQuestAvailable()
    {
        var vertices = new List<DialogueVertex>
        {
            new("Hi again, skeleton lover. Do you want to kill them for me after all?"),
            new("Then fuck off, dude"),
            new()
            {
                Text = "Ok then, best of luck!",
                GameEventTrigger = new QuestAcceptedEventTrigger("skeleton_trouble")
            },
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "No. As I said, I think skeletons are cute"));
        graph.AddEdge(new LabelledEdge(0, 2, "Ok then. I'll kill'em for you."));
        graph.AddEdge(new LabelledEdge(1, 3, "Ok"));
        graph.AddEdge(new LabelledEdge(2, 3, "Thanks. See you later"));

        return graph;
    }
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Captain_SkeletonQuestActive()
    {
        var vertices = new List<DialogueVertex>
        {
            new("Have you taken care of those skeletons yet?"),
            new("Well, then get to it."),
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "No, not yet."));
        graph.AddEdge(new LabelledEdge(1, 2, "Very well."));

        return graph;
    }
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> GenerateGraph_Captain_SkeletonQuestTurnIn()
    {
        var vertices = new List<DialogueVertex>
        {
            new("Way to go, my friend - you smacked them skeletons up good. Here you have 100 coins in reward."),
            new(""),
        };

        var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

        graph.AddEdge(new LabelledEdge(0, 1, "Thanks, man."));

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