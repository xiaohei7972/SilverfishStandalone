using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毒蛇假发
// Serpent Wig
// 使一个随从获得+1/+2。如果你在本牌在你手中时使用过纳迦牌，将一张毒蛇假发置入你的手牌。
// Give a minion +1/+2. If you played a Naga while holding this, add a Serpent Wig to your hand.
internal static class Sim_TSC_215
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
