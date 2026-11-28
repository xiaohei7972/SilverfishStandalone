using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迷彩坐骑
// Camouflage Mount
// 使一个随从获得+3/+3和一项随机额外效果。当该随从死亡时，召唤一只变色龙。
// Give a minion +3/+3 and a random Bonus Effect. When it dies, summon a Chameleon.
internal static class Sim_WW_810
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
