using CommunityToolkit.Mvvm.ComponentModel;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class NumPadButton : ObservableObject
{
    public bool IsActive
    {
        get => _isActive;
        private set {
            bool wasActive = _isActive;
            _isActive = value;

            if ( wasActive != IsActive ) {
                OnPropertyChanged( nameof( BackgroundColor ) );
                OnPropertyChanged( nameof( TextColor ) );
            }
        }
    }

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

    private int _remainingCount;
    private bool _isActive;

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

    public void SetActive() => IsActive = true;

    public void SetInactive() => IsActive = false;

    public void UpdateRemainingCount( int remainingCount ) => RemainingCount = remainingCount;
}
