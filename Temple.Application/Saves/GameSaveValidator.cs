using FluentValidation;

namespace Temple.Application.Saves;

public sealed class GameSaveValidator : AbstractValidator<GameSave>
{
    public GameSaveValidator()
    {
        RuleFor(x => x.FormatVersion).Equal(GameSave.CurrentFormatVersion)
            .WithMessage("This save format is not supported.");
        RuleFor(x => x.ContentVersion).Equal(GameSave.CurrentContentVersion)
            .WithMessage("This save requires a different content version.");
        RuleFor(x => x.SavedAtUtc).NotEqual(default(DateTimeOffset));
        RuleFor(x => x.Location).IsInEnum();
        RuleFor(x => x.CurrentSiteId).NotEmpty();
        RuleFor(x => x.Party).NotEmpty();
        RuleForEach(x => x.Party).NotNull().ChildRules(member =>
        {
            member.RuleFor(x => x.CreatureTypeId).NotEmpty();
        });
        RuleFor(x => x.Quests).NotNull();
        RuleForEach(x => x.Quests).NotNull().ChildRules(quest =>
        {
            quest.RuleFor(x => x.QuestId).NotEmpty();
            quest.RuleFor(x => x.State).IsInEnum();
        });
        RuleFor(x => x.KnowledgeGained).NotNull();
        RuleForEach(x => x.KnowledgeGained).NotEmpty();
        RuleFor(x => x.FactsEstablished).NotNull();
        RuleForEach(x => x.FactsEstablished).NotEmpty();
        RuleFor(x => x.BattlesWon).NotNull();
        RuleForEach(x => x.BattlesWon).NotEmpty();
        RuleFor(x => x.SitesUnlocked).NotNull();
        RuleForEach(x => x.SitesUnlocked).NotEmpty();
        RuleFor(x => x).Must(x =>
            (x.PositionX is null && x.PositionY is null && x.Orientation is null) ||
            (x.PositionX is double px && double.IsFinite(px) &&
             x.PositionY is double py && double.IsFinite(py) &&
             x.Orientation is double angle && double.IsFinite(angle)))
            .WithMessage("The saved position and orientation must be complete and finite.");
    }
}
