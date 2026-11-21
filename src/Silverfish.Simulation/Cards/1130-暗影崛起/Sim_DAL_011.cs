using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拉祖尔的阴谋
// Lazul's Scheme
// 直到你的下个回合，使一个敌方随从的攻击力降低@点。（每回合都会升级！）
// Reduce the Attack of an enemy minion by @ until your next turn. (Upgrades each turn!)
internal static class Sim_DAL_011
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
