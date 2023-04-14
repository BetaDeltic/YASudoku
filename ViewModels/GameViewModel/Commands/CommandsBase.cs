using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class CommandsBase
{
    protected readonly CommonButtonVisualState pencil;
    protected readonly CommonButtonVisualState eraser;
    protected readonly NumPadVisualState numPad;
    protected readonly GameGridVisualState grid;
    protected readonly VisualStatesHandler visualState;

    protected GameGridCellVisualData? SelectedCell => grid.SelectedCell;
    protected NumPadButton? SelectedNumber => numPad.SelectedButton;

    protected bool IsPaused => visualState.IsPaused;

    public CommandsBase( VisualStatesHandler visualState )
    {
        pencil = visualState.PencilVS;
        eraser = visualState.EraserVS;
        numPad = visualState.NumPadVS;
        grid = visualState.GameGridVS;
        this.visualState = visualState;
    }
}
