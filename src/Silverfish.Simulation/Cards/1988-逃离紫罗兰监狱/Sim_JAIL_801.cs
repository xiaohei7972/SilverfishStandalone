using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 熔炼黄金
// Molten Gold
// 造成$4点伤害。（施放@个法术以变为随从牌！）
// Deal $4 damage. (Cast @ |4(spell, spells) to turn into a minion!)
internal static class Sim_JAIL_801
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
