namespace Silverfish.Simulation.Cards;

// 疯狂投弹者
// Mad Bomber
// 战吼：造成3点伤害，随机分配到所有其他角色身上。
// Battlecry: Deal 3 damage randomly split between all other characters.
[CardSim("EX1_082")]
internal static class Sim_EX1_082
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.OtherCharacters, 1, Count: 3)]
    };
}
