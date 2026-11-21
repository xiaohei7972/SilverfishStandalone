using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪恶挥刺（等级2）
// Wicked Stab (Rank 2)
// 造成$4点伤害。（当你有10点法力值时升级。）
// Deal $4 damage. (Upgrades when you have 10 Mana.)
internal static class Sim_BAR_319t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
