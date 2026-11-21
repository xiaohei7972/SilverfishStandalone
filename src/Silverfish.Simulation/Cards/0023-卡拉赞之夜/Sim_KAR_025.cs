using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 附灵术
// Kara Kazham!
// 召唤一个1/1的蜡烛，2/2的扫帚和3/3的茶壶。
// Summon a 1/1 Candle, 2/2 Broom, and 3/3 Teapot.
internal static class Sim_KAR_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
