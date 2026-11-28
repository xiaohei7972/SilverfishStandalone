using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 咒怨纪念品
// Cursed Souvenir
// 使一个随从获得+3/+3和“在你的回合开始时，对你的英雄造成3点伤害”。
// Give a minion +3/+3 and "At the start of your turn, deal 3 damage to your hero."
internal static class Sim_VAC_944
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
