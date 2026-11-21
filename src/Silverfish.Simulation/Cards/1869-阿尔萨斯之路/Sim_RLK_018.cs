using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 凋零打击
// Plague Strike
// 对一个随从造成$3点伤害。如果该随从死亡，召唤一个2/2并具有突袭的僵尸。
// Deal $3 damage to a minion. If it dies, summon a 2/2 Zombie with Rush.
internal static class Sim_RLK_018
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
