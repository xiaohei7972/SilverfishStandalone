using System.Text.Json.Serialization;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public enum ConditionCharacter { Source, Selected, TriggerSubject, FriendlyHero, EnemyHero }
public enum CharacterMetric { Health, Damage, Attack, Armor }
public enum CharacterDisposition { Alive, Dead, Frozen }
public enum ConditionPlayer { Friendly, Opponent }
public enum NumberComparison { Equal, AtLeast, AtMost }

/// <summary>在当前结算状态上判断是否执行一组效果或发动触发器。</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(CharacterCondition), "character")]
[JsonDerivedType(typeof(CharacterStateCondition), "characterState")]
[JsonDerivedType(typeof(ZoneCountCondition), "zoneCount")]
[JsonDerivedType(typeof(ComboCondition), "combo")]
[JsonDerivedType(typeof(WeaponEquippedCondition), "weaponEquipped")]
[JsonDerivedType(typeof(CharacterMatchCondition), "characterMatch")]
public abstract record EffectCondition;

public sealed record CharacterCondition(
    [property: JsonRequired] ConditionCharacter Target,
    [property: JsonRequired] CharacterMetric Metric,
    [property: JsonRequired] NumberComparison Comparison,
    [property: JsonRequired] int Value) : EffectCondition;

public sealed record CharacterStateCondition(
    [property: JsonRequired] ConditionCharacter Target,
    [property: JsonRequired] CharacterDisposition State) : EffectCondition;

public sealed record ZoneCountCondition(
    [property: JsonRequired] ConditionPlayer Player,
    [property: JsonRequired] GameZone Zone,
    [property: JsonRequired] NumberComparison Comparison,
    [property: JsonRequired] int Value,
    EntityKind? EntityKind = null) : EffectCondition
{
    public CardFilter? CardFilter { get; init; }
}

public sealed record ComboCondition : EffectCondition;

public sealed record WeaponEquippedCondition(ConditionPlayer Player = ConditionPlayer.Friendly) : EffectCondition;

public sealed record CharacterMatchCondition(
    [property: JsonRequired] ConditionCharacter Target,
    TriggerRelation Relation = TriggerRelation.Any,
    CardFilter? CardFilter = null) : EffectCondition;
