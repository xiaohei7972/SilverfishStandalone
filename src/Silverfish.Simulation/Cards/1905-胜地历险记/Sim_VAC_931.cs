using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 生死一线
// Skirting Death
// 选择一个随从。在本回合中，你的英雄从该随从处偷取4点攻击力。
// Choose a minion. This turn, your hero steals 4 Attack from it.
internal static class Sim_VAC_931
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
