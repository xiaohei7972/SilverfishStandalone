using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小鬼油膏
// Impbalming
// 消灭一个随从。将三张“游荡小鬼”洗入你的牌库。
// Destroy a minion. Shuffle 3 Worthless Imps into your deck.
internal static class Sim_ULD_324
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
