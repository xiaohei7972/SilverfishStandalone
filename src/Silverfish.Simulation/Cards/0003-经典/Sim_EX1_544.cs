namespace Silverfish.Simulation.Cards;

// 照明弹
// Flare
// 所有随从失去潜行，摧毁所有敌方奥秘，抽一张牌。
// All minions lose Stealth. Destroy all enemy Secrets. Draw a card.
[CardSim("EX1_544")]
internal static class Sim_EX1_544
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RemoveAllStealthEffect(), new DestroySecretsEffect(), new DrawEffect(1)]
    };
}
