using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 亡者归来
// Anyfin Can Happen
// 召唤七个在本局对战中死亡的 鱼人。
// Summon 7 Murlocs that died this game.
[CardSim("LOE_026")]
internal static class Sim_LOE_026
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ResurrectMatchingMinionsEffect(new CardFilter { MinionType = MinionType.Murloc }, IncludeOpponent: true),
        ],
    };
}
