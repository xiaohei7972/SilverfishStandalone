using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 复活术
// Resurrect
// 随机召唤一个在本局对战中死亡的友方随从。
// Summon a random friendly minion that died this game.
[CardSim("BRM_017")]
internal static class Sim_BRM_017
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ResurrectFriendlyMinionEffect()]
    };
}
