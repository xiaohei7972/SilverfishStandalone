using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 真言术：耀
// Power Word: Glory
// 选择一个随从。每当其进行攻击，为你的英雄恢复 #4点生命值。
// Choose a minion. Whenever it attacks, restore #4 Health to your hero.
internal static class Sim_AT_013
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
