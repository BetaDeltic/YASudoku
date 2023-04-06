namespace YASudoku.Services.SettingsService;

public class SettingsService : ISettingsService
{
    public event Action<bool>? MistakesHighlightingChanged;

    public static Color ForegroundColor { get; set; } = Colors.White;
    public static Color DisabledForegroundColor { get; set; } = Colors.LightGray;
    public static Color DisabledBackgroundColor { get; set; } = Colors.Black;

    private static readonly Color accentColor = Colors.DarkMagenta;
    private const string highlightRelatedCellsSettingName = "HighlightRelatedCells";
    private const string hightlightMistakesSettingName = "HighlightMistakes";

    public SettingsService()
    {
        SetAccentColor( AccentColors.Magenta );
        SetHighlightingRelatedCells( true );
        SetHighlightingMistakes( true );
    }

    public void SetAccentColor( AccentColors colorTheme )
        => Preferences.Set( nameof( accentColor ), ( (int)colorTheme ) );

    public Color GetAccentColor()
    {
        int colorNumber = Preferences.Get( nameof( SettingsService.accentColor ), defaultValue: 0 );

        Color accentColor = (AccentColors)colorNumber switch {
            AccentColors.Magenta => Colors.DarkMagenta,
            AccentColors.Green => Colors.DarkGreen,
            AccentColors.Blue => Colors.DarkBlue,
            AccentColors.Cyan => Colors.DarkCyan,
            AccentColors.GoldenRod => Colors.DarkGoldenrod,
            AccentColors.OliveGreen => Colors.DarkOliveGreen,
            AccentColors.Orange => Colors.DarkOrange,
            AccentColors.SlateGrey => Colors.DarkSlateGrey,
            _ => Colors.DarkMagenta,
        };

        return accentColor;
    }

    public void SetHighlightingRelatedCells( bool highlightingRelatedCells )
        => Preferences.Set( highlightRelatedCellsSettingName, highlightingRelatedCells );

    public bool CanHighlightRelatedCells()
        => Preferences.Get( highlightRelatedCellsSettingName, defaultValue: true );

    public void SetHighlightingMistakes( bool highlightingMistakes )
    {
        Preferences.Set( hightlightMistakesSettingName, highlightingMistakes );
        MistakesHighlightingChanged?.Invoke( highlightingMistakes );
    }

    public bool CanHighlightMistakes()
        => Preferences.Get( hightlightMistakesSettingName, defaultValue: true );
}
