using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 为了奎尔萨拉斯！
// For Quel'Thalas!
// 使一个友方随从获得+3攻击力。在本回合中，使你的英雄获得+2攻击力。
// Give a friendly minion +3 Attack. Give your hero +2 Attack this turn.
internal static class Sim_RLK_918
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
