using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 构造区
// Construct Quarter
// 消灭一个友方随从，召唤一个4/5并具有突袭的亡灵。
// Destroy a friendly minion to summon a 4/5 Undead with Rush.
internal static class Sim_NX2_036
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
