using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 贪食软泥怪
// Gluttonous Ooze
// 战吼：摧毁对手的武器，并获得等同于其攻击力的护甲值。
// Battlecry: Destroy your opponent's weapon and gain Armor equal to its Attack.
internal static class Sim_UNG_946
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
