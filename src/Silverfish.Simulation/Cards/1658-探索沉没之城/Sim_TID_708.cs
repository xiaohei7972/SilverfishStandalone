using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 变形术：水母
// Polymorph: Jellyfish
// 将一个随从变形成为一只4/1并具有法术伤害+2的水母。
// Transform a minion into a 4/1 Jellyfish with Spell Damage +2.
internal static class Sim_TID_708
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
