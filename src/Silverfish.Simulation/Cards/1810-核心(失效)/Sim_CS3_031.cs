using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 生命的缚誓者阿莱克丝塔萨
// Alexstrasza the Life-Binder
// 战吼：选择一个角色。如果是友方角色，为其恢复#8点生命值；如果是敌方角色，对其造成8点伤害。
// Battlecry: Choose a character. If it's friendly, restore #8 Health. If it's an ___enemy, deal 8 damage.
internal static class Sim_CS3_031
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
