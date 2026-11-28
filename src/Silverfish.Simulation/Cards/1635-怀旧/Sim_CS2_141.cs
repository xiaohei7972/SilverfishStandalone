using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 铁炉堡火枪手
// Ironforge Rifleman
// 战吼：造成1点伤害。
// Battlecry: Deal 1 damage.
internal static class Sim_CS2_141
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
