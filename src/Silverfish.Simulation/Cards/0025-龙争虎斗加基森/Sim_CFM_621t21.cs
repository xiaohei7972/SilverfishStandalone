using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神秘羊毛
// Mystic Wool
// 随机使一个敌方随从变形成为1/1的绵羊。@随机使一个敌方随从变形。
// Transform a random enemy minion into a 1/1 Sheep.@Polymorph a random enemy minion.
internal static class Sim_CFM_621t21
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_TOTAL_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
