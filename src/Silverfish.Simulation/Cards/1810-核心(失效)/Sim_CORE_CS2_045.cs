using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 石化武器
// Rockbiter Weapon
// 在本回合中，使一个友方角色获得+3攻击力。
// Give a friendly character +3 Attack this turn.
internal static class Sim_CORE_CS2_045
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
