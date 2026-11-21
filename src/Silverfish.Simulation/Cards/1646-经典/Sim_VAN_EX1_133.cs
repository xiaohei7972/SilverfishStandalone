using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毁灭之刃
// Perdition's Blade
// 战吼：造成1点伤害。连击：改为造成2点伤害。
// Battlecry: Deal 1 damage. Combo: Deal 2 instead.
internal static class Sim_VAN_EX1_133
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
