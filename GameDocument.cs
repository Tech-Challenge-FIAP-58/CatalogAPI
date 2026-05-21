public record GameDocument
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string TitleRaw { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string Genre { get; init; } = default!;
    public float Price { get; init; }
    public DateTime ReleaseDate { get; init; }
}