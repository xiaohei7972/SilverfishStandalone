using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 山羊坐骑
// Ramming Mount
// 使一个随从获得+2/+2和攻击时具有免疫。当该随从死亡时，召唤一只山羊。
// Give a minion +2/+2 and Immune while attacking. When it dies, summon a Ram.
internal static class Sim_SW_458
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
