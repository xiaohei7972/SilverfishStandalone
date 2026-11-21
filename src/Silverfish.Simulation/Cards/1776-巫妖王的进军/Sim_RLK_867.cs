using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 维库通灵师
// Vrykul Necrolyte
// 战吼：使一个友方随从获得“亡语：召唤一个2/2并具有突袭的僵尸。”
// Battlecry: Give a friendly minion "Deathrattle: Summon a 2/2 Zombie with Rush."
internal static class Sim_RLK_867
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
