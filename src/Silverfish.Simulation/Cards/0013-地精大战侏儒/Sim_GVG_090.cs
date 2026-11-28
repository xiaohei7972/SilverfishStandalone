namespace Silverfish.Simulation.Cards;

// 疯狂爆破者
// Madder Bomber
// 战吼：造成6点伤害，随机分配到所有其他角色身上。
// Battlecry: Deal 6 damage randomly split between all other characters.
[CardSim("GVG_090")]
internal static class Sim_GVG_090
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.OtherCharacters, 1, Count: 6)]
    };
}
