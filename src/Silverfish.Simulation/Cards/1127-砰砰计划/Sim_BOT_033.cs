using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 投掷炸弹
// Bomb Toss
// 造成$2点伤害。召唤一个0/2的地精炸弹。
// Deal $2 damage. Summon a 0/2 Goblin_Bomb.
internal static class Sim_BOT_033
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
