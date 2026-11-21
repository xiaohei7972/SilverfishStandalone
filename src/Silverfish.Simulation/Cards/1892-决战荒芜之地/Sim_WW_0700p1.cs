using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奥术枪弹
// Arcane Bullet
// 造成$2点伤害。复原2个法力水晶。每回合切换。
// Deal $2 damage. Refresh 2 Mana Crystals. Swaps each turn.
internal static class Sim_WW_0700p1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
