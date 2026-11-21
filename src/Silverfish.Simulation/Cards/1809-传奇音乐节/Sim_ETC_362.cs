using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 跳吧，虫子！
// JIVE, INSECT!
// 将一个随从变形成为炎魔之王拉格纳罗斯。过载：（2）
// Transform a minion into Ragnaros the Firelord. Overload: (2)
internal static class Sim_ETC_362
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
