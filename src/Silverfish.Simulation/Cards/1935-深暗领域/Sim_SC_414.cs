using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雷神
// Thor
// 战吼：造成5点伤害。（如果你在本局对战中发射过星舰，则会变形。）
// Battlecry: Deal 5 damage. (Transforms if you launched a Starship this game.)
internal static class Sim_SC_414
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
