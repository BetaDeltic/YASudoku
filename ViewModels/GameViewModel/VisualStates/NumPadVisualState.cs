using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public class NumPadVisualState
{
    public event Action<int>? NewNumberSelected;

    public readonly List<NumPadButton> NumPadButtons;

    public NumPadButton? SelectedButton { get; private set; }
    public int SelectedButtonNumber { get; private set; }

    public NumPadVisualState( int gridSize, ISettingsService settings, IResourcesService resources )
    {
        NumPadButtons = new( gridSize );
        for ( int i = 0; i < gridSize; i++ ) {
            NumPadButtons.Add( new( settings, resources ) );
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
