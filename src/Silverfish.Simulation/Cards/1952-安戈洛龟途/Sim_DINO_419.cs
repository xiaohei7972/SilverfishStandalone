using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 饲草助手
// Herbivore Assistant
// 战吼：使一只友方野兽获得+2/+2和突袭。
// Battlecry: Give a friendly Beast +2/+2 and Rush.
internal static class Sim_DINO_419
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
