using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 尤格-萨隆的监狱
// Prison of Yogg-Saron
// 选择一个角色。随机施放4个法术（尽可能以其为目标）。
// Choose a character. Cast 4 random spells (targeting it if possible).
internal static class Sim_TTN_090
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)]);
    // </legacy-play-requirements>

}
