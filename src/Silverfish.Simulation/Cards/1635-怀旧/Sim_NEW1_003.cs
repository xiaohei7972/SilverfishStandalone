using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 牺牲契约
// Sacrificial Pact
// 消灭一个友方恶魔，为你的英雄恢复#5点生命值。
// Destroy a friendly Demon. Restore #5 Health to your hero.
internal static class Sim_NEW1_003
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DEMON)],
        []);
    // </legacy-play-requirements>

}
