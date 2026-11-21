using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 地精跟班
// Goblin Lackey
// 战吼： 使一个友方随从获得+1攻击力和突袭。
// Battlecry: Give a friendly minion +1 Attack and_Rush.
internal static class Sim_DAL_739
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
