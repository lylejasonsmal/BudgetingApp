namespace MyFirstAppComponentLibrary.UI.Svgs
{
    public record SvgIconModel
    {
        public required string ViewBox { get; init; }
        public required IReadOnlyList<SvgPath> Paths { get; init; }
    }

    public record SvgPath
    {
        public required string Data { get; init; }
        public string Fill { get; init; } = "currentColor";
    }
}
