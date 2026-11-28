using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 新月祈愿
// Wish of the New Moon
// 对一个随从 造成$6点伤害。 （施放@个法术 以获得吸血。）
// Deal $6 damage to a minion. (Cast @ |4(spell, spells) to gain Lifesteal.)
internal static class Sim_EDR_460
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
