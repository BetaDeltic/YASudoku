namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public class NumPadVisualState
{
    public event Action<int>? NewNumberSelected;

    public List<NumPadButton> NumPadButtons;

    public NumPadButton? SelectedButton;
    public int SelectedButtonNumber;

    private readonly Color enabledBackgroundColor = Colors.White;
    private readonly Color enabledForegroundColor = Colors.DarkMagenta;
    private readonly Color disabledBackgroundColor = Colors.LightGray;
    private readonly Color disabledForegroundColor = Colors.Black;

    public NumPadVisualState( int gridSize )
    {
        NumPadButtons = new( gridSize );
        for ( int i = 0; i < gridSize; i++ ) {
            NumPadButtons.Add( new(
                activeBackgroundColor: enabledBackgroundColor,
                activeTextColor: enabledForegroundColor,
                inactiveBackgroundColor: enabledForegroundColor,
                inactiveTextColor: enabledBackgroundColor,
                disabledActiveBackgroundColor: disabledBackgroundColor,
                disabledActiveTextColor: disabledForegroundColor,
                disabledInactiveBackgroundColor: disabledForegroundColor,
                disabledInactiveTextColor: disabledBackgroundColor
                )
            );
        }
    }

    public void SelectNewNumber( int newNumber )
    {
        SelectedButton = GetNumPadButton( newNumber );
        SelectedButton.SetActive();
        SelectedButtonNumber = newNumber;

        NewNumberSelected?.Invoke( newNumber );
    }

    public void DeselectCurrentNumber()
    {
        if ( SelectedButton == null )
            return;

        SelectedButton.SetInactive();
        SelectedButton = null;
        SelectedButtonNumber = 0;
    }

    public bool IsNumberSelected( int number ) => SelectedButtonNumber == number;

    public bool IsAnyNumberSelected() => SelectedButtonNumber != 0;

    public void UpdateButtonRemainingCount( int buttonNumber, int remaining )
    {
        if ( buttonNumber <= 0 ) {
            return;
        }

        int numberIndex = buttonNumber - 1;

        NumPadButtons[ numberIndex ].UpdateRemainingCount( remaining );
    }

    private NumPadButton GetNumPadButton( int buttonNumber )
    {
        int numberIndex = buttonNumber - 1;
        return NumPadButtons[ numberIndex ];
    }
}
