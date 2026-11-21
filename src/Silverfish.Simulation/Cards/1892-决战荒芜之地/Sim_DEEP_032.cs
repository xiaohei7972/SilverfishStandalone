using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵魂冻结
// Soulfreeze
// 冻结一个随从及其相邻随从，对你的英雄造成等同于所冻结随从数量的伤害。
// Freeze a minion and its neighbors. Deal damage to your hero equal to the number Frozen.
internal static class Sim_DEEP_032
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
