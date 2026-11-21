using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪恶挥刺（等级1）
// Wicked Stab (Rank 1)
// 造成$2点伤害。（当你有5点法力值时升级。）
// Deal $2 damage. (Upgrades when you have 5 Mana.)
internal static class Sim_BAR_319
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
