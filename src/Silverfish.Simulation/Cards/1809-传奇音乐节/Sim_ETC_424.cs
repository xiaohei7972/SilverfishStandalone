using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 死亡嘶吼
// Death Growl
// 选择一个随从。将其亡语传播到相邻随从上。
// Choose a minion. Spread its Deathrattle to adjacent minions.
internal static class Sim_ETC_424
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE)],
        []);
    // </legacy-play-requirements>

}
