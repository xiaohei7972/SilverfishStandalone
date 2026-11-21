using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑暗突变
// Dark Transformation
// 将一个亡灵变形成为一个4/5并具有突袭的亡灵怪物。
// Transform an Undead into a 4/5 Undead Monstrosity with Rush.
internal static class Sim_RLK_057
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.UNDEAD)],
        []);
    // </legacy-play-requirements>

}
