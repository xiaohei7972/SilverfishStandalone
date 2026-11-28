using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 疯狂的药剂师
// Crazed Chemist
// 连击：使一个友方随从获得+4攻击力。
// Combo: Give a friendly minion +4 Attack.
internal static class Sim_BOT_576
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_FOR_COMBO), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
