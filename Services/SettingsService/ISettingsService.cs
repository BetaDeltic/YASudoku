namespace YASudoku.Services.SettingsService;

public enum AccentColors
{
    Magenta,
    Green,
    Blue,
    Cyan,
    GoldenRod,
    OliveGreen,
    Orange,
    SlateGrey
}

public interface ISettingsService
{
    public event Action<bool>? MistakesHighlightingChanged;

    public void SetAccentColor( AccentColors accentColor );
    public Color GetAccentColor();

    public void SetHighlightingRelatedCells( bool highlightingRelatedCells );
    public bool CanHighlightRelatedCells();

    public void SetHighlightingMistakes( bool highlightingMistakes );
    public bool CanHighlightMistakes();
}
