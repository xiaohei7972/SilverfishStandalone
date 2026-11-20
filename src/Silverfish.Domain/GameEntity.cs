using System.Collections.Immutable;

namespace Silverfish.Domain;

/// <summary>实体的固定标识。模拟中的卡牌、控制者、区域与属性可以独立变化。</summary>
public readonly record struct EntityId(int Value)
{
    public override string ToString() => Value.ToString();
}

public sealed record CharacterState(int Attack, int MaximumHealth, int Damage = 0, int Armor = 0,
    int AttacksThisTurn = 0, bool Exhausted = false, bool DivineShield = false)
{
    public int? BaseAttack { get; init; }
    public int? BaseMaximumHealth { get; init; }
    public bool Frozen { get; init; }
    public int Health => MaximumHealth - Damage;
}

/// <summary>已装备武器的战斗属性。武器 ID 由所属实体提供。</summary>
public sealed record WeaponStats(
    int Attack,
    int Durability,
    int MaximumDurability,
    int AttacksThisTurn = 0);

public sealed record GameEntity(EntityId Id, string CardId, PlayerId Owner, PlayerId Controller)
{
    public CharacterState? Character { get; init; }
    public WeaponStats? Weapon { get; init; }
    public long? PlayOrder { get; init; }
    public bool Silenced { get; init; }
    public ImmutableArray<StatEnchantment> Enchantments { get; init; } = [];
    public ImmutableArray<StatAuraGrant> AuraGrants { get; init; } = [];
    public ImmutableArray<ManaCostModifier> CostModifiers { get; init; } = [];
}

public sealed record ManaCostModifier(EntityId Source, int Delta);
