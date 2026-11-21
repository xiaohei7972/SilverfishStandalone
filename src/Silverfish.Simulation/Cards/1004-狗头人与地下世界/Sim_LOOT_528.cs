using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暮光侍僧
// Twilight Acolyte
// 战吼：如果你的手牌中有龙牌，则将本随从的攻击力与另一个随从交换。
// Battlecry: If you're holding a Dragon, swap this minion's Attack with another minion's.
internal static class Sim_LOOT_528
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
