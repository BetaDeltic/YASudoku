using CommunityToolkit.Mvvm.ComponentModel;

namespace YASudoku.ViewModels.GameViewModel;

public partial class NumPadButton : ObservableObject
{
    public bool IsActive { get; private set; } = false;

    public bool IsEnabled => RemainingCount > 0;

    public Color? BackgroundColor
        => IsActive
            ? IsEnabled ? activeBackgroundColor : disabledActiveBackgroundColor
            : IsEnabled ? inactiveBackgroundColor : disabledInactiveBackgroundColor;

    public Color? TextColor
        => IsActive
            ? IsEnabled ? activeTextColor : disabledActiveTextColor
            : IsEnabled ? inactiveTextColor : disabledInactiveTextColor;

    public int RemainingCount
    {
        get => _remainingCount;
        private set {
            bool wasEnabled = IsEnabled;
            _remainingCount = value;
            OnPropertyChanged( nameof( RemainingCount ) );

            if ( wasEnabled != IsEnabled ) {
                OnPropertyChanged( nameof( BackgroundColor ) );
                OnPropertyChanged( nameof( TextColor ) );
            }
        }
    }

    [ObservableProperty]
    private string number = string.Empty;

    private int _remainingCount;

    private readonly Color activeBackgroundColor;
    private readonly Color inactiveBackgroundColor;
    private readonly Color activeTextColor;
    private readonly Color inactiveTextColor;
    private readonly Color disabledActiveBackgroundColor;
    private readonly Color disabledInactiveBackgroundColor;
    private readonly Color disabledActiveTextColor;
    private readonly Color disabledInactiveTextColor;

    public NumPadButton(
        Color activeBackgroundColor,
        Color inactiveBackgroundColor,
        Color activeTextColor,
        Color inactiveTextColor,
        Color disabledActiveBackgroundColor,
        Color disabledInactiveBackgroundColor,
        Color disabledActiveTextColor,
        Color disabledInactiveTextColor
        )
    {
        this.activeBackgroundColor = activeBackgroundColor;
        this.inactiveBackgroundColor = inactiveBackgroundColor;
        this.activeTextColor = activeTextColor;
        this.inactiveTextColor = inactiveTextColor;
        this.disabledActiveBackgroundColor = disabledActiveBackgroundColor;
        this.disabledInactiveBackgroundColor = disabledInactiveBackgroundColor;
        this.disabledActiveTextColor = disabledActiveTextColor;
        this.disabledInactiveTextColor = disabledInactiveTextColor;
    }

    public void SetActive()
        => ChangeIsActive( true );

    public void SetInactive()
        => ChangeIsActive( false );

    public void UpdateRemainingCount( int remainingCount )
        => RemainingCount = remainingCount;

    private void ChangeIsActive( bool newValue )
    {
        bool wasActive = IsActive;
        IsActive = newValue;

        if ( wasActive != IsActive ) {
            OnPropertyChanged( nameof( BackgroundColor ) );
            OnPropertyChanged( nameof( TextColor ) );
        }
    }
}
