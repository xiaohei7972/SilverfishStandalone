using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火焰试炼
// Trial by Fire
// 召唤五个1/1并具有突袭的瓦格里。当一个死亡时，使其他的获得+1/+1。
// Summon five 1/1 Val'kyr with Rush. When one dies, give the others +1/+1.
internal static class Sim_TTN_470
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 5)],
        []);
    // </legacy-play-requirements>

}
