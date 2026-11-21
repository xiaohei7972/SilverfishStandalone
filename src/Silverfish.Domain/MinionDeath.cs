namespace Silverfish.Domain;

public sealed record MinionDeath(EntityId Entity, string CardId, PlayerId Controller, int Turn);
