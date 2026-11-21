using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鲁莽试验
// Unexpected Results
// 随机召唤两个法力值消耗为（$2）的随从（受法术伤害加成影响）。
// Summon two random $2-Cost minions (improved by Spell Damage).
internal static class Sim_BOT_254
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
