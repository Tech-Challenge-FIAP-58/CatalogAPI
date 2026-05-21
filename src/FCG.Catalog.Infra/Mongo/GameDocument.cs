namespace FCG.Catalog.Infra.Mongo
{
    public record GameDocument
    {
        public string Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string PublisherName { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string Platform { get; init; } = default!;
        public double Price { get; init; }
        public bool IsAvailable { get; init; }
    }
}
