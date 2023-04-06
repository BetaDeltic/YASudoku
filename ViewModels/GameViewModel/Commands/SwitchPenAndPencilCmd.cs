using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class SwitchPenAndPencilCmd
{
    private readonly CommonButtonVisualState visualState;

    public SwitchPenAndPencilCmd( CommonButtonVisualState visualState )
    {
        this.visualState = visualState;
    }

    public void SwitchPenAndPencil()
    {
        if ( visualState.IsActive ) {
            visualState.DeactivateButton();
        } else {
            visualState.ActivateButton();
        }
    }
}
