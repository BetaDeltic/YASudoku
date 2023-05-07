namespace YASudoku.Services.SettingsService;

public class SettingsService : ISettingsService
{
    public event Action<bool>? MistakesHighlightingChanged;

    private static readonly Color accentColor = Colors.DarkMagenta;
    private const string highlightRelatedCellsSettingName = "HighlightRelatedCells";
    private const string hightlightMistakesSettingName = "HighlightMistakes";

    public SettingsService()
    {
        SetPrimaryColor( PrimaryColors.Magenta );
        SetHighlightingRelatedCells( true );
        SetHighlightingMistakes( true );
    }

    public void SetPrimaryColor( PrimaryColors colorTheme )
        => Preferences.Set( nameof( accentColor ), ( (int)colorTheme ) );

    public Color GetPrimaryColor()
    {
        int colorNumber = Preferences.Get( nameof( SettingsService.accentColor ), defaultValue: 0 );

        Color accentColor = (PrimaryColors)colorNumber switch {
            PrimaryColors.Magenta => Colors.DarkMagenta,
            PrimaryColors.Green => Colors.DarkGreen,
            PrimaryColors.Blue => Colors.DarkBlue,
            PrimaryColors.Cyan => Colors.DarkCyan,
            PrimaryColors.GoldenRod => Colors.DarkGoldenrod,
            PrimaryColors.OliveGreen => Colors.DarkOliveGreen,
            PrimaryColors.Orange => Colors.DarkOrange,
            PrimaryColors.SlateGrey => Colors.DarkSlateGrey,
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
