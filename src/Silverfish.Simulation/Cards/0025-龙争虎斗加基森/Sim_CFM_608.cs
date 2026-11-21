using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 爆晶药水
// Blastcrystal Potion
// 消灭一个随从，和你的一个法力水晶。
// Destroy a minion and one of your Mana Crystals.
internal static class Sim_CFM_608
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
