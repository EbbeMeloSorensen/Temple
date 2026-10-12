using FluentAssertions;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Quests;
using Temple.Infrastructure.GameConditions;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.UnitTest;

public class GameConditionSerializationTest
{
    public static IEnumerable<object[]> Conditions()
    {
        yield return new object[] { new FactEstablishedCondition { FactId = "key" } };
        yield return new object[] { new KnowledgeGainedCondition { KnowledgeId = "secret" } };
        yield return new object[] { new BattleWonCondition { BattleId = "rats" } };
        yield return new object[]
        {
            new QuestStatusCondition
            {
                QuestId = "quest",
                RequiredStatus = new QuestStatus
                {
                    QuestState = QuestState.Active,
                    AreCompletionCriteriaSatisfied = false
                }
            }
        };
        yield return new object[]
        {
            new AndGameCondition
            {
                Conditions = new List<IGameCondition>
                {
                    new FactEstablishedCondition { FactId = "key" },
                    new OrGameCondition
                    {
                        Conditions = new List<IGameCondition>
                        {
                            new KnowledgeGainedCondition { KnowledgeId = "secret" },
                            new NotGameCondition
                            {
                                Condition = new BattleWonCondition { BattleId = "rats" }
                            }
                        }
                    }
                }
            }
        };
        yield return new object[]
        {
            new OrGameCondition
            {
                Conditions = new List<IGameCondition>
                {
                    new FactEstablishedCondition { FactId = "key" }
                }
            }
        };
        yield return new object[]
        {
            new NotGameCondition
            {
                Condition = new FactEstablishedCondition { FactId = "key" }
            }
        };
    }

    [Theory]
    [MemberData(nameof(Conditions))]
    public void ConditionRoundTripPreservesConcreteTypesAndValues(IGameCondition condition)
    {
        var json = condition.ConvertToJsonRepresentation();

        var deserialized = json.ConvertToGameCondition();

        deserialized.Should().BeOfType(condition.GetType());
        deserialized.Should().BeEquivalentTo(condition, options => options.PreferringRuntimeMemberTypes());
        deserialized.ConvertToJsonRepresentation().Should().Be(json);
    }
}
