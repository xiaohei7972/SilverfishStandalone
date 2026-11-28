using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

/// <summary>按静态卡牌数据筛选实体。</summary>
public sealed record CardFilter
{
    public EntityKind? Kind { get; init; }
    public MinionType? MinionType { get; init; }
    public SpellSchool? SpellSchool { get; init; }
    public int? MinimumPrintedCost { get; init; }
    public int? MaximumPrintedCost { get; init; }
    public bool? HasBattlecry { get; init; }
    public bool? HasDeathrattle { get; init; }
    public bool? IsLegendary { get; init; }
    public string? CardId { get; init; }
    public TAG_CLASS? CardClass { get; init; }
    public bool? HasTaunt { get; init; }
    public bool? HasSpellDamage { get; init; }
}

public sealed partial class GameSimulator
{
    private static readonly MinionType KnownMinionTypes = Enum.GetValues<MinionType>()
        .Aggregate(MinionType.None, (all, type) => all | type);

    private static bool HasValidCardTraits(CardDefinition definition) =>
        (definition.MinionTypes is not MinionType types ||
            ((types & ~KnownMinionTypes) == 0 && (types == MinionType.None || definition.Kind == EntityKind.Minion))) &&
        (definition.SpellSchool is not SpellSchool school || Enum.IsDefined(school));

    private static bool IsValidCardFilter(CardFilter? filter) =>
        filter is null || (
            (filter.Kind is not EntityKind kind || (kind != EntityKind.Unknown && Enum.IsDefined(kind))) &&
            (filter.MinionType is not MinionType type || Enum.IsDefined(type)) &&
            (filter.SpellSchool is not SpellSchool school || Enum.IsDefined(school)) &&
            (filter.CardClass is not TAG_CLASS cardClass || Enum.IsDefined(cardClass)) &&
            filter.MinimumPrintedCost is not < 0 && filter.MaximumPrintedCost is not < 0 &&
            (filter.MinimumPrintedCost is not int minimum || filter.MaximumPrintedCost is not int maximum || minimum <= maximum));

    private static bool MatchesCardFilter(CardDefinition definition, CardFilter filter) =>
        (filter.Kind is not EntityKind kind || definition.Kind == kind) &&
        (filter.MinionType is not MinionType type || MatchesMinionType(definition, type)) &&
        (filter.SpellSchool is not SpellSchool school || definition.SpellSchool.GetValueOrDefault() == school) &&
        (filter.MinimumPrintedCost is not int minimum || definition.PrintedCost >= minimum) &&
        (filter.MaximumPrintedCost is not int maximum || definition.PrintedCost <= maximum) &&
        (filter.CardClass is not TAG_CLASS cardClass || (definition.ClassMask & CardMetadata.ClassBit(cardClass)) != 0) &&
        (filter.CardId is null || definition.CardId == filter.CardId);

    private static bool MatchesCardFilter(SimulationCard card, CardFilter filter) =>
        MatchesCardFilter(card.Definition, filter) &&
        (filter.HasBattlecry is not bool battlecry ||
            (PlayReqTags.Has(card.RequirementTagMask, (int)GAME_TAG.BATTLECRY) ||
                (card.Definition.Kind == EntityKind.Minion && (!card.OnPlay.IsEmpty || card.ChooseAfterPlay is not null))) == battlecry) &&
        (filter.HasDeathrattle is not bool deathrattle ||
            (!card.Deathrattle.IsEmpty || PlayReqTags.Has(card.RequirementTagMask, (int)Silverfish.Domain.Client.GAME_TAG.DEATHRATTLE)) == deathrattle) &&
        (filter.IsLegendary is not bool legendary ||
            PlayReqTags.Has(card.RequirementTagMask, (int)Silverfish.Domain.Client.GAME_TAG.RARITY) == legendary);

    private bool MatchesEntityFilter(GameEntity entity, CardFilter filter) =>
        MatchesCardFilter(Card(entity), filter) &&
        (filter.HasTaunt is not bool taunt || IsTaunt(entity) == taunt) &&
        (filter.HasSpellDamage is not bool power ||
            ((!entity.Silenced && (Card(entity).SpellDamage > 0 || Card(entity).BothPlayersSpellDamage > 0)) ||
                entity.Enchantments.Any(layer => layer.GrantedTag == GAME_TAG.SPELLPOWER)) == power);

    private static bool MatchesMinionType(CardDefinition definition, MinionType type)
    {
        if (definition.Kind != EntityKind.Minion)
            return false;
        var printed = definition.MinionTypes.GetValueOrDefault();
        return type == MinionType.None ? printed == MinionType.None
            : printed.HasFlag(MinionType.All) || printed.HasFlag(type);
    }
}
