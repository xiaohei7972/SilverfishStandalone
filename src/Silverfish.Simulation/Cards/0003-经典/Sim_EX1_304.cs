using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 虚空恐魔
// Void Terror
// 战吼：消灭两侧相邻的随从，并获得他们的攻击力和生命值。
// Battlecry: Destroy both adjacent minions and gain their Attack and Health.
[CardSim("EX1_304")]
internal static class Sim_EX1_304
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ConsumeAdjacentMinionsEffect(Sim_EX1_304e.CardId)]
    };
}
