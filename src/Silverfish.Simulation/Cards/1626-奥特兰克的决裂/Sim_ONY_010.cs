using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灭龙射击
// Dragonbane Shot
// 造成$2点伤害。荣誉消灭：将一张灭龙射击置入你的手牌。
// Deal $2 damage. Honorable Kill: Add a Dragonbane Shot to your hand.
internal static class Sim_ONY_010
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
