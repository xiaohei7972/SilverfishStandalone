using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪能篝火
// Felfire Bonfire
// 对一个随从造成$4点伤害。如果该随从死亡，你的下一个亡语随从的法力值消耗减少（3）点。
// Deal $4 damage to a minion. If it dies, your next Deathrattle minion costs (3) less.
internal static class Sim_VAC_952
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
