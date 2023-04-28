using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class SwitchPenAndPencilCmd
{
    private readonly CommonButtonVisualState commonVS;
    private readonly VisualStatesHandler visualState;

    public SwitchPenAndPencilCmd( VisualStatesHandler visualState )
    {
        commonVS = visualState.PencilVS;
        this.visualState = visualState;
    }

    public void SwitchPenAndPencil()
    {
        if ( visualState.IsPaused ) return;

        if ( visualState.EraserVS.IsActive ) {
            visualState.EraserVS.DeactivateButton();
        }

        if ( commonVS.IsActive ) {
            commonVS.DeactivateButton();
        } else {
            commonVS.ActivateButton();
        }
    }
}
