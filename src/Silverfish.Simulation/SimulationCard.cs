using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Silverfish.Domain;

namespace Silverfish.Simulation;

[Flags]
public enum CardKeywords { None = 0, Taunt = 1, Charge = 2, DivineShield = 4 }

public enum TargetRequirement { None, Character, EnemyCharacter, EnemyMinion }
public enum DamageTarget { Selected, EnemyHero, AllMinions }

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(DamageEffect), "damage")]
[JsonDerivedType(typeof(DrawEffect), "draw")]
[JsonDerivedType(typeof(SummonEffect), "summon")]
[JsonDerivedType(typeof(TakeControlEffect), "takeControl")]
public abstract record CardEffect;

public sealed record DamageEffect(DamageTarget Target, int Amount) : CardEffect;
public sealed record DrawEffect(int Count) : CardEffect;
/// <summary>Summons on the right edge of the controlling player's battlefield.</summary>
public sealed record SummonEffect(string CardId) : CardEffect;
public sealed record TakeControlEffect : CardEffect;

public sealed record SimulationCard(CardDefinition Definition)
{
    public CardKeywords Keywords { get; init; }
    public TargetRequirement Target { get; init; }
    public ImmutableArray<CardEffect> OnPlay { get; init; } = [];
    public ImmutableArray<CardEffect> Deathrattle { get; init; } = [];

    internal CharacterState CreateCharacter() => new(Definition.PrintedAttack, Definition.PrintedHealth,
        Exhausted: Definition.Kind == EntityKind.Minion && !Keywords.HasFlag(CardKeywords.Charge),
        DivineShield: Keywords.HasFlag(CardKeywords.DivineShield));
}
