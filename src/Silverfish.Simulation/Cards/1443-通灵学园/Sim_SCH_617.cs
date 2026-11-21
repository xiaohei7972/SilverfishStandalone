using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 萌物来袭
// Adorable Infestation
// 使一个随从获得+1/+1。召唤一个1/1的魔鼠宝宝。将一张魔鼠宝宝置入你的手牌。
// Give a minion +1/+1. Summon a 1/1 Cub. Add a Cub to your hand.
internal static class Sim_SCH_617
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
