using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星体射击
// Celestial Shot
// 造成$3点伤害。你的下一个法术拥有法术伤害+2。
// Deal $3 damage. Your next spell has Spell Damage +2.
internal static class Sim_YOG_082
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
