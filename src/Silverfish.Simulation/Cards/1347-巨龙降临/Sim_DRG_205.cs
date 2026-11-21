using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚空吐息
// Nether Breath
// 造成$2点伤害。如果你的手牌中有龙牌，则改为造成$4点伤害并具有吸血。
// Deal $2 damage. If you're holding a Dragon, deal $4 damage with Lifesteal instead.
internal static class Sim_DRG_205
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
