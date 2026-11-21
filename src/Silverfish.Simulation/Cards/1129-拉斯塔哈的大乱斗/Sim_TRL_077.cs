using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 古拉巴什宣传员
// Gurubashi Hypemon
// 战吼： 发现一张具有战吼的随从牌的1/1复制，其法力值消耗为（1）点。
// Battlecry: Discover a 1/1 copy of a Battlecry minion. It costs (1).
internal static class Sim_TRL_077
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
