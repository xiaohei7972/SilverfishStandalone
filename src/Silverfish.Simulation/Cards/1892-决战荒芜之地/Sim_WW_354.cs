using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 残骸遍野
// Fistful of Corpses
// 对一个随从造成等同于你的残骸数量的伤害。
// Deal damage to a minion equal to your Corpses.
internal static class Sim_WW_354
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
