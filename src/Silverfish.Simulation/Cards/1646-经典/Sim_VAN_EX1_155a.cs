using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 猛虎之怒
// Tiger's Fury
// +4攻击力。
// +4 Attack.
internal static class Sim_VAN_EX1_155a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
