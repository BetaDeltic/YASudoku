using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class SwitchPenAndPencilCmd
{
    private readonly CommonButtonVisualState commonVS;

    public SwitchPenAndPencilCmd( CommonButtonVisualState visualState )
    {
        commonVS = visualState;
    }

    public void SwitchPenAndPencil()
    {
        if ( commonVS.IsActive ) {
            commonVS.DeactivateButton();
        } else {
            commonVS.ActivateButton();
        }
    }
}
