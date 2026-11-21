using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 旋岩虫
// Gyreworm
// 战吼：如果你在上个回合使用过元素牌，则造成3点伤害。
// Battlecry: If you played an Elemental last turn, deal 3_damage.
internal static class Sim_DMF_062
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN)],
        []);
    // </legacy-play-requirements>

}
