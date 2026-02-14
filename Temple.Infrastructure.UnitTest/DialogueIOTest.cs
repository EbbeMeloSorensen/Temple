using Newtonsoft.Json;
using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;
using Temple.Application.DD;
using Temple.Domain.Entities.DD.Quests;
using Temple.Infrastructure.Dialogues;
using Temple.Infrastructure.IO;
using Temple.Infrastructure.Dialogues.GameEventTriggers;
using Temple.Infrastructure.GameConditions;

namespace Temple.Infrastructure.UnitTest
{
    public class DialogueIOTest
    {
        [Fact]
        public void GenerateDialogueGraphCollectionForAlythThenWriteToJsonFile()
        {
            // Arrange
            var dialogueGraphs = new List<DialogueGraph>
            {
                new DialogueGraph
                {
                    Priority = 100,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "rat_infestation",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Hidden,
                            AreCompletionCriteriaSatisfied = false
                        }
                    },
                    Graph = GenerateGraph_Alyth_RatQuestHidden()
                },
                new DialogueGraph
                {
                    Priority = 100,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "rat_infestation",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Available,
                            AreCompletionCriteriaSatisfied = false
                        }
                    },
                    Graph = GenerateGraph_Alyth_RatQuestAvailable()
                },
                new DialogueGraph
                {
                    Priority = 100,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "rat_infestation",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = false
                        }
                    },
                    Graph = GenerateGraph_Alyth_RatQuestActive()
                },
                new DialogueGraph
                {
                    Priority = 100,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "rat_infestation",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = true
                        }
                    },
                    Graph = GenerateGraph_Alyth_RatQuestTurnIn()
                },
                new DialogueGraph
                {
                    Priority = 100,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "find_ethon",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = false
                        }
                    },
                    Graph = GenerateGraph_Alyth_EthonQuestActive()
                },
                new DialogueGraph
                {
                    Priority = 100,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "find_ethon",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = true
                        }
                    },
                    Graph = GenerateGraph_Alyth_EthonQuestTurnIn()
                },
                new DialogueGraph
                {
                    Priority = 0,
                    Graph = GenerateGraph_Alyth_SmallTalkDialogue()
                }
            };

            dialogueGraphs.WriteToFile(@"C:\Temp\alyth.json");
        }

        [Fact]
        public void GenerateDialogueGraphCollectionForCaptainThenWriteToJsonFile()
        {
            // Arrange
            var dialogueGraphs = new List<DialogueGraph>
            {
                new DialogueGraph
                {
                    Priority = 100,
                    Condition = new AndGameCondition
                    {
                        Conditions =
                        {
                            new FactEstablishedCondition
                            {
                                FactId = "party_talked_with_lortimer"
                            },
                            new FactEstablishedCondition
                            {
                                FactId = "party_talked_with_nebbish"
                            }
                        }
                    },
                    Graph = GenerateGraph_Captain_SmallTalkDialogue2()
                },
                new DialogueGraph
                {
                    Priority = 90,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "skeleton_trouble",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Hidden,
                            AreCompletionCriteriaSatisfied = false
                        }
                    },
                    Graph = GenerateGraph_Captain_SkeletonQuestHidden()
                },
                new DialogueGraph
                {
                    Priority = 90,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "skeleton_trouble",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Available,
                            AreCompletionCriteriaSatisfied = false
                        }
                    },
                    Graph = GenerateGraph_Captain_SkeletonQuestAvailable()
                },
                new DialogueGraph
                {
                    Priority = 90,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "skeleton_trouble",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = false
                        }
                    },
                    Graph = GenerateGraph_Captain_SkeletonQuestActive()
                },
                new DialogueGraph
                {
                    Priority = 90,
                    Condition = new QuestStatusCondition()
                    {
                        QuestId = "skeleton_trouble",
                        RequiredStatus = new QuestStatus
                        {
                            QuestState = QuestState.Active,
                            AreCompletionCriteriaSatisfied = true
                        }
                    },
                    Graph = GenerateGraph_Captain_SkeletonQuestTurnIn()
                },
                new DialogueGraph
                {
                    Priority = 0,
                    Graph = GenerateGraph_Captain_SmallTalkDialogue1()
                }
            };

            dialogueGraphs.WriteToFile(@"C:\Temp\captain.json");
        }

        [Fact]
        public void ReadDialogueGraphCollectionforAlythFromJsonFileThenExportAsDotFile()
        {
            var npcId = "alyth";

            var dialogueGraphs =
                DialogueIO.ReadDialogueGraphListFromFile($@"C:\Git\GitHub\Temple\Temple.UI.WPF\DD\Assets\DialogueGraphCollections\{npcId}.json");

            var count = 0;
            foreach (var dialogueGraph in dialogueGraphs)
            {
                count++;
                var outputFileName = $@"C:\Temp\DialogueGrapg_{npcId}_{count}.dot";
                dialogueGraph.WriteToDotFile(outputFileName);
            }
        }

        [Fact]
        public void ReadDialogueGraphCollectionforCaptainFromJsonFileThenExportAsDotFile()
        {
            var npcId = "captain";

            var dialogueGraphs =
                DialogueIO.ReadDialogueGraphListFromFile($@"C:\Git\GitHub\Temple\Temple.UI.WPF\DD\Assets\DialogueGraphCollections\{npcId}.json");

            var count = 0;
            foreach (var dialogueGraph in dialogueGraphs)
            {
                count++;
                var outputFileName = $@"C:\Temp\DialogueGrapg_{npcId}_{count}.dot";
                dialogueGraph.WriteToDotFile(outputFileName);
            }
        }

        [Fact]
        public void ReadDialogueGraphCollectionforEthonFromJsonFileThenExportAsDotFile()
        {
            var npcId = "ethon";

            var dialogueGraphs =
                DialogueIO.ReadDialogueGraphListFromFile($@"C:\Git\GitHub\Temple\Temple.UI.WPF\DD\Assets\DialogueGraphCollections\{npcId}.json");

            var count = 0;
            foreach (var dialogueGraph in dialogueGraphs)
            {
                count++;
                var outputFileName = $@"C:\Temp\DialogueGraph_{npcId}_{count}.dot";
                dialogueGraph.WriteToDotFile(outputFileName);
            }
        }

        [Fact]
        public void WriteDialogueGraphToDotFile()
        {
            // Arrange
            var dialogueGraph = new DialogueGraph
            {
                Priority = 100,
                Condition = new QuestStatusCondition()
                {
                    QuestId = "rat_infestation",
                    RequiredStatus = new QuestStatus
                    {
                        QuestState = QuestState.Hidden,
                        AreCompletionCriteriaSatisfied = false
                    }
                },
                Graph = GenerateGraph_Alyth_RatQuestHidden()
            };

            // Act
            dialogueGraph.WriteToDotFile(@"C:\Temp\MyDialogueGraph.dot");
        }

        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Alyth_RatQuestHidden()
        {
            var vertices = new List<DialogueVertex>
        {
            new("Beautiful song, isn't it? I've heard her sing a hundred times, and each time, it still moves me."),
            new("It's the spirit of an elven woman; she haunts this tavern, singing once every couple of nights."),
            new("No one truly knows. Her spirit was here when I first bought this tavern. Some say she sings for a lost love, a soldier who died defending Baldur's Gate. They say she sings in the hope he will her her voice and return home. Still, that's nothing but hearsay and tales - welcome to the Elfsong Tavern. What can I get you?"),
            new()
            {
                Text = "That's a stuffed beholder... a small version of the species, I'm told, not that I've seen many of them. One of my regulars, Ethon, found it in the cellar.",
                GameEventTrigger = new KnowledgeGainedEventTrigger("ethon_found_beholder")
            },
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
            new()
            {
                Text = "The door to the cellar's locked, so you'll need to get the key from Ethon over in the corner there.",
                GameEventTrigger = new KnowledgeGainedEventTrigger("ethon_has_key_to_cellar")
            },
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
            new("Ok, then get out of my inn, rat lover!"),
            new("How wonderful, I love strawberries.")
        };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "It is beautiful... but where's the voice coming from?"));
            graph.AddEdge(new DialogueEdge(0, 17, "I think it sounds rather lame"));
            graph.AddEdge(new DialogueEdge(0, 19, "I brought you some strawberries")
            {
                KnowledgeRequired = "alyth_likes_strawberries"
            });
            graph.AddEdge(new DialogueEdge(1, 2, "Why does she sing?"));
            graph.AddEdge(new DialogueEdge(2, 3, "What's that thing hanging over the fireplace?"));
            graph.AddEdge(new DialogueEdge(3, 4, "What's a beholder?"));
            graph.AddEdge(new DialogueEdge(2, 5, "I was hoping you could help me. I was robbed on the streets by a band of thieves, and I'm looking to find them."));
            graph.AddEdge(new DialogueEdge(3, 5, "Actually, I was hoping you could help me. I was robbed on the streets by a band of thieves, and I'm looking to find them."));
            graph.AddEdge(new DialogueEdge(4, 5, "I see. Look, I came here because I was attacked on the streets by a band of thieves, and I'm looking to find them."));
            graph.AddEdge(new DialogueEdge(5, 6, "So this new guild's at war with the old guild?"));
            graph.AddEdge(new DialogueEdge(6, 7, "I don't have much choice; they stole every last coin I had. If you know where I can find them, tell me."));
            graph.AddEdge(new DialogueEdge(5, 7, "Do you know where I can find these thieves?"));
            graph.AddEdge(new DialogueEdge(7, 8, "Why the sewers?"));
            graph.AddEdge(new DialogueEdge(8, 9, "All right. How do I get into the sewers?"));
            graph.AddEdge(new DialogueEdge(9, 10, "Could I use the gate?"));
            graph.AddEdge(new DialogueEdge(10, 11, "What do you mean?"));
            graph.AddEdge(new DialogueEdge(11, 12, "It's a deal."));
            graph.AddEdge(new DialogueEdge(12, 13, "Who's Ethon"));
            graph.AddEdge(new DialogueEdge(12, 15, "I'll go speak to him, then."));
            graph.AddEdge(new DialogueEdge(13, 14, "When did they start appearing?"));
            graph.AddEdge(new DialogueEdge(13, 15, "I'll go speak to him, then."));
            graph.AddEdge(new DialogueEdge(14, 15, "I'll go get the key from Ethon and see about taking care of those rats, then."));
            graph.AddEdge(new DialogueEdge(15, 16, "OK"));
            graph.AddEdge(new DialogueEdge(17, 18, "No, I think rats are cute"));
            graph.AddEdge(new DialogueEdge(17, 15, "Sure, why not"));
            graph.AddEdge(new DialogueEdge(18, 16, "OK"));
            graph.AddEdge(new DialogueEdge(19, 16, "See you later.."));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Alyth_RatQuestAvailable()
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

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "No. As I said, I think rats are cute"));
            graph.AddEdge(new DialogueEdge(0, 2, "Ok then. I'll kill those critters for you."));
            graph.AddEdge(new DialogueEdge(1, 3, "Ok"));
            graph.AddEdge(new DialogueEdge(2, 3, "Thanks. See you later"));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Alyth_RatQuestActive()
        {
            var vertices = new List<DialogueVertex>
        {
            new("Have you taken care of those rats yet? I'm not giving you the key to the sewers until they're all wiped out."),
            new("Well, I'd prefer you get to it before they change the name of this tavern to the Rat's Nest. Talk to Ethon if you don't have the key to the cellar yet."),
            new(""),
        };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "No, not yet."));
            graph.AddEdge(new DialogueEdge(1, 2, "Very well."));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Alyth_RatQuestTurnIn()
        {
            var vertices = new List<DialogueVertex>
            {
                new()
                {
                    Text = "You're back! Did you see Ethon down there?",
                    GameEventTrigger = new QuestDiscoveredEventTrigger("find_ethon")
                },
                new()
                {
                    Text = "He followed you down there not long ago, and I fear he may have gotten lost in the cellar... or worse, he may have wandered into the sewers. I tried to stop him...",
                    GameEventTrigger = new QuestAcceptedEventTrigger("find_ethon")
                },
                new()
                {
                    Text = "Thank you... but before you go, please take these coins -- in payment for all you've done so far. And as promised, here's the key to the sewer gate. Be careful down there - there's bound to be worse things than sewer rats in those tunnels.",
                    GameEventTrigger = new FactEstablishedEventTrigger("got_key_to_sewer_door_from_alyth")
                },
                new(""),
            };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "In the cellar? No I didn't see him."));
            graph.AddEdge(new DialogueEdge(1, 2, "Don't worry, Alyth.  I've taken care of all the rats, so he probably just got lost. I'll find him."));
            graph.AddEdge(new DialogueEdge(2, 3, "I'll be careful. Thanks, Alyth."));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Alyth_EthonQuestActive()
        {
            var vertices = new List<DialogueVertex>
            {
                new("You're back! Did you see Ethon down there?"),
                new(""),
            };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "Not yet. I'm still looking. I'll return when I find him."));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Alyth_EthonQuestTurnIn()
        {
            var vertices = new List<DialogueVertex>
            {
                new()
                {
                    Text = "Thank you for finding Ethon; he told me what happened down in the Sewers."
                },
                new()
                {
                    Text = "Here's some coins for your trouble... and please, you're welcome to rest here anytime, no charge."
                },
                new(""),
            };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "You're welcome, Alyth."));
            graph.AddEdge(new DialogueEdge(1, 2, "Thank you. I appreciate the hospitality."));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Alyth_SmallTalkDialogue()
        {
            var vertices = new List<DialogueVertex>
        {
            new("Nice weather today, huh?"),
            new(""),
        };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "Sure"));

            return graph;
        }

        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Captain_SkeletonQuestHidden()
        {
            var vertices = new List<DialogueVertex>
            {
                new() // 0
                {
                    Text = "Hi there, welcome to the village. I am Boris, the captain of the city watch.",
                    GameEventTrigger = new FactEstablishedEventTrigger("party_talked_with_captain")
                },
                new() // 1
                {
                    Text = "Sure, you can slay some skeletons for me, will ya?",
                    GameEventTrigger = new QuestDiscoveredEventTrigger("skeleton_trouble")
                },
                new() // 2
                {
                    Text = "They are on the graveyard outside of the village.",
                    GameEventTrigger = new SiteUnlockedEventTrigger("graveyard")
                },
                new() // 3
                {
                    Text = "Ok, then fuck off, skeleton lover! By the way, Alyth likes strawberries.",
                    GameEventTrigger = new KnowledgeGainedEventTrigger("alyth_likes_strawberries")
                },
                new() // 4
                {
                    Text = "Ok, then good luck, my friend.",
                    GameEventTrigger = new QuestAcceptedEventTrigger("skeleton_trouble")
                }, // 5
                new(""),
            };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "Nice to meet you, do you have a quest for me?"));
            graph.AddEdge(new DialogueEdge(1, 3, "No thanks, I think skeletons are cute"));
            graph.AddEdge(new DialogueEdge(3, 5, "OK"));
            graph.AddEdge(new DialogueEdge(1, 2, "Where are those skeletons?"));
            graph.AddEdge(new DialogueEdge(2, 3, "That's too far away."));
            graph.AddEdge(new DialogueEdge(2, 4, "Ok, I'll go there and slay them for ya"));
            graph.AddEdge(new DialogueEdge(4, 5, "Thanks, see you later"));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Captain_SkeletonQuestAvailable()
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

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "No. As I said, I think skeletons are cute"));
            graph.AddEdge(new DialogueEdge(0, 2, "Ok then. I'll kill'em for you."));
            graph.AddEdge(new DialogueEdge(1, 3, "Ok"));
            graph.AddEdge(new DialogueEdge(2, 3, "Thanks. See you later"));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Captain_SkeletonQuestActive()
        {
            var vertices = new List<DialogueVertex>
        {
            new("Have you taken care of those skeletons yet?"),
            new("Well, then get to it."),
            new(""),
        };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "No, not yet."));
            graph.AddEdge(new DialogueEdge(1, 2, "Very well."));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Captain_SkeletonQuestTurnIn()
        {
            var vertices = new List<DialogueVertex>
        {
            new("Way to go, my friend - you smacked them skeletons up good. Here you have 100 coins in reward."),
            new(""),
        };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "Thanks, man."));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Captain_SmallTalkDialogue1()
        {
            var vertices = new List<DialogueVertex>
        {
            new("Terrible weather today, huh?"),
            new(""),
        };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "Nah I think the weather is nice"));

            return graph;
        }
        private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Captain_SmallTalkDialogue2()
        {
            var vertices = new List<DialogueVertex>
            {
                new("Dude, you talked with nebbish and lortimer"),
                new(""),
            };

            var graph = new GraphAdjacencyList<DialogueVertex, DialogueEdge>(vertices, true);

            graph.AddEdge(new DialogueEdge(0, 1, "Sure did, dude"));

            return graph;
        }
    }
}