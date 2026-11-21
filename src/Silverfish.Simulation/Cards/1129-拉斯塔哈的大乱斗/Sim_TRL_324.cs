using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 重金属狂潮
// Heavy Metal!
// 随机召唤一个法力值消耗等同于你的护甲值（最高不超过10点）的随从。
// Summon a random minion with Cost equal to your Armor (up to 10).
internal static class Sim_TRL_324
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
