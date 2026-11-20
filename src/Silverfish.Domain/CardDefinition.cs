namespace Silverfish.Domain;

public enum EntityKind
{
    Unknown, Hero, Minion, Spell, Weapon, Location, HeroPower,
    Enchantment, Other
}

/// <summary>不可变的卡牌印刷属性。战斗标签与临时增益记录在实体状态中。</summary>
public sealed record CardDefinition(
    string CardId,
    EntityKind Kind,
    int PrintedCost,
    int PrintedAttack = 0,
    int PrintedHealth = 0,
    int PrintedDurability = 0);
