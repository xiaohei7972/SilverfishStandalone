using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火焰使者
// Blazecaller
// 战吼：如果你在上个回合使用过元素牌，则造成5点伤害。
// Battlecry: If you played an_Elemental last turn, deal 5 damage.
internal static class Sim_UNG_847
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN)],
        []);
    // </legacy-play-requirements>

}
