using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 死亡之影
// Shadow of Death
// 选择一个随从。将三张“阴影”牌洗入你的牌库，当抽到“阴影”时，召唤该随从的一个复制。
// Choose a minion. Shuffle 3 'Shadows' into your deck that summon a copy when drawn.
internal static class Sim_ULD_286
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
