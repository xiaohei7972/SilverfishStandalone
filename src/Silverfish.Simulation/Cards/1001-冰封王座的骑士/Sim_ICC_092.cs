using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 阿彻鲁斯老兵
// Acherus Veteran
// 战吼：使一个友方随从获得+1攻击力。
// Battlecry: Give a friendly minion +1 Attack.
internal static class Sim_ICC_092
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
