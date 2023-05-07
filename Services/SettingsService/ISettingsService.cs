namespace YASudoku.Services.SettingsService;

public enum PrimaryColors
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

    public void SetPrimaryColor( PrimaryColors primaryColor );
    public Color GetPrimaryColor();

    public void SetHighlightingRelatedCells( bool highlightingRelatedCells );
    public bool CanHighlightRelatedCells();

    public void SetHighlightingMistakes( bool highlightingMistakes );
    public bool CanHighlightMistakes();
}
