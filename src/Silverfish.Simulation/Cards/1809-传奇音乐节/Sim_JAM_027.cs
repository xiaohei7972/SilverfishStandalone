using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 饭圈迷弟
// Fanboy
// 抉择：使一个友方随从获得+2攻击力和突袭；或者+2生命值和吸血。
// Choose One - Give a friendly minion +2 Attack and Rush; or +2 Health and Lifesteal.
internal static class Sim_JAM_027
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
