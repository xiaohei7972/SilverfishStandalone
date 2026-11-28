using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 强效治疗药水
// Greater Healing Potion
// 为一个友方角色恢复#12点生命值。抽一张牌。
// Restore #12 Health to a friendly character. Draw a card.
internal static class Sim_CFM_604
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
