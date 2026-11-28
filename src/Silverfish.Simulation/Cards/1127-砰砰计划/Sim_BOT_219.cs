using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 增生手臂
// Extra Arms
// 使一个随从获得+2/+2。将一张可使一个随从获得+2/+2的“更多手臂”置入你的手牌。
// Give a minion +2/+2. Add 'More Arms!' to your hand that gives +2/+2.
internal static class Sim_BOT_219
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
