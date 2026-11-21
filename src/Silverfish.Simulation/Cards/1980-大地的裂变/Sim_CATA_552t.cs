using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 乌鳞斥候
// Ebonscale Scout
// 战吼：造成等同于本随从攻击力的伤害。
// Battlecry: Deal damage equal to this minion's Attack.
internal static class Sim_CATA_552t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
