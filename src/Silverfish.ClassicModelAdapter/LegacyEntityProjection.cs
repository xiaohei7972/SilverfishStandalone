using System.Collections.Immutable;
using HREngine.Bots;
using Silverfish.Domain;

namespace Silverfish.ClassicModelAdapter;

/// <summary>
/// READ-ONLY migration probe for the original classic models.
/// It captures a known subset of old mutable fields into detached immutable
/// values; it is not yet a replacement for the full classic simulator.
/// </summary>
public static class LegacyEntityProjection
{
    public static EntityState CaptureCharacter(Minion legacy, CardDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(legacy);
        ArgumentNullException.ThrowIfNull(definition);
        var kind = legacy.isHero ? EntityKind.Hero : EntityKind.Minion;
        if (definition.Kind != kind)
            throw new ArgumentException("Card definition kind is inconsistent with legacy character type.");
        var keywords = ImmutableHashSet.CreateBuilder<string>(StringComparer.OrdinalIgnoreCase);
        Add(legacy.taunt, "TAUNT");
        Add(legacy.divineshild, "DIVINE_SHIELD");
        Add(legacy.stealth, "STEALTH");
        Add(legacy.windfury, "WINDFURY");
        Add(legacy.poisonous, "POISONOUS");
        Add(legacy.lifesteal, "LIFESTEAL");
        Add(legacy.immune, "IMMUNE");
        Add(legacy.reborn, "REBORN");
        Add(legacy.silenced, "SILENCED");
        Add(ReadBool(legacy, "Titan") == true, "TITAN");
        Add(legacy.frozen, "FROZEN");

        var snapshot = new EntityState
        {
            Id = new EntityId(legacy.entitiyID),
            CardId = definition.CardId,
            Kind = kind,
            Side = legacy.own ? PlayerSide.Friendly : PlayerSide.Opponent,
            Zone = EntityZone.Battlefield,
            Position = legacy.zonepos,
            Combat = new CombatStats(
                legacy.Angr, legacy.Hp, legacy.maxHp, legacy.armor,
                legacy.numAttacksThisTurn, legacy.Ready),
            Keywords = keywords.ToImmutable(),
            EnchantmentCardIds = legacy.enchs.Select(x => x.ToString()).ToImmutableArray(),
            Counters = CharacterCounters(legacy)
        };
        snapshot.Validate();
        return snapshot;

        void Add(bool enabled, string keyword)
        {
            if (enabled) keywords.Add(keyword);
        }
    }

    public static EntityState CaptureHandCard(
        Handmanager.Handcard legacy,
        CardDefinition definition,
        PlayerSide side = PlayerSide.Friendly)
    {
        ArgumentNullException.ThrowIfNull(legacy);
        ArgumentNullException.ThrowIfNull(definition);
        var snapshot = new EntityState
        {
            Id = new EntityId(legacy.entity),
            CardId = definition.CardId,
            Kind = definition.Kind,
            Side = side,
            Zone = EntityZone.Hand,
            Position = legacy.position,
            Cost = new ManaCost(definition.PrintedCost, legacy.manacost),
            HandModifiers = new HandStatModifiers(legacy.addattack, legacy.addHp),
            EnchantmentCardIds = legacy.enchs.Select(x => x.ToString()).ToImmutableArray(),
            Counters = HandCounters(legacy)
        };
        snapshot.Validate();
        return snapshot;
    }

    private static ImmutableDictionary<string, int> CharacterCounters(Minion source)
    {
        var counters = ImmutableDictionary<string, int>.Empty
            .Add("dormant", source.dormant);
        counters = AddIfPresent(counters, "scriptData1", ReadInt(source, "TAG_SCRIPT_DATA_NUM_1"));
        counters = AddIfPresent(counters, "scriptData2", ReadInt(source, "TAG_SCRIPT_DATA_NUM_2"));
        counters = AddIfPresent(counters, "titanAbility1Used", ReadBool(source, "TitanAbilityUsed1") is bool a ? (a ? 1 : 0) : null);
        counters = AddIfPresent(counters, "titanAbility2Used", ReadBool(source, "TitanAbilityUsed2") is bool b ? (b ? 1 : 0) : null);
        counters = AddIfPresent(counters, "titanAbility3Used", ReadBool(source, "TitanAbilityUsed3") is bool c ? (c ? 1 : 0) : null);
        return counters;
    }

    private static ImmutableDictionary<string, int> HandCounters(Handmanager.Handcard source)
    {
        var counters = ImmutableDictionary<string, int>.Empty
            .Add("conditionalCount", source.conditionalCount)
            .Add("poweredUp", source.poweredUp)
            .Add("temporary", source.temporary ? 1 : 0);
        counters = AddIfPresent(counters, "scriptData1", ReadInt(source, "SCRIPT_DATA_NUM_1"));
        counters = AddIfPresent(counters, "scriptData2", ReadInt(source, "SCRIPT_DATA_NUM_2"));
        counters = AddIfPresent(counters, "unplayable", ReadBool(source, "literallyUnplayable") is bool u ? (u ? 1 : 0) : null);
        return counters;
    }

    private static ImmutableDictionary<string, int> AddIfPresent(
        ImmutableDictionary<string, int> counters, string name, int? value) =>
        value is int present ? counters.Add(name, present) : counters;

    // Old forks lack some modern tags. Missing fields are omitted; they must
    // never be presented as known zero values in the migrated state.
    private static int? ReadInt(object source, string name) =>
        source.GetType().GetField(name)?.GetValue(source) is int value ? value : null;

    private static bool? ReadBool(object source, string name)
    {
        var type = source.GetType();
        var value = type.GetField(name)?.GetValue(source)
                    ?? type.GetProperty(name)?.GetValue(source);
        return value is bool present ? present : null;
    }

    /// <summary>
    /// Classic Weapon has no reliable entity ID or maximum durability. Never
    /// invent either: the caller must supply both from an authoritative source.
    /// </summary>
    public static EntityState CaptureWeapon(
        Weapon legacy, CardDefinition definition, EntityId entityId,
        PlayerSide side, int maximumDurability)
    {
        ArgumentNullException.ThrowIfNull(legacy);
        ArgumentNullException.ThrowIfNull(definition);
        if (definition.Kind != EntityKind.Weapon)
            throw new ArgumentException("Expected a weapon card definition.");
        if (legacy.enchants.Count != 0)
            throw new NotSupportedException("Old weapon enchantments need a lossless effect adapter.");

        var keywords = ImmutableHashSet.CreateBuilder<string>(StringComparer.OrdinalIgnoreCase);
        if (legacy.lifesteal) keywords.Add("LIFESTEAL");
        if (legacy.poisonous) keywords.Add("POISONOUS");
        if (legacy.windfury) keywords.Add("WINDFURY");
        if (legacy.immune) keywords.Add("IMMUNE");

        var snapshot = new EntityState
        {
            Id = entityId,
            CardId = definition.CardId,
            Kind = EntityKind.Weapon,
            Side = side,
            Zone = EntityZone.Weapon,
            Equipment = new WeaponStats(
                legacy.Angr, legacy.Durability, maximumDurability,
                legacy.numAttacksThisTurn),
            Keywords = keywords.ToImmutable(),
            Counters = ImmutableDictionary<string, int>.Empty
                .Add("scriptData1", legacy.scriptNum1)
        };
        snapshot.Validate();
        return snapshot;
    }
}
