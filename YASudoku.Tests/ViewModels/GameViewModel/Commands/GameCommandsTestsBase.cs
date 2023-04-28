namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class GameCommandsTestsBase : GameVMTestsBase
{
    protected int IndexOfTestedCell;

    protected int originalValue;

    protected void SendCompletionSignalsOnAnimations()
    {
        VisualState.SignalWhenWipingGameBoard.Subscribe( _ => VisualState.WipingGameBoardCompleted.OnNext( true ) );
        VisualState.SignalWhenStartingNewGame.Subscribe( _ => VisualState.StartingNewGameCompleted.OnNext( true ) );
    }

    public static IEnumerable<object[]> ArrangeActions()
    {
        var baseClass = new GameVMTestsBase();
        yield return new[] { () => { } };
        yield return new[] { baseClass.ActivateEnabledNumber };
        yield return new[] { baseClass.ActivateDisabledNumber };
        yield return new[] { () => { baseClass.ActivateEmptyCell( out _ ); } };
        yield return new[] { () => { baseClass.ActivateLockedCell( out _ ); } };
        yield return new[] { () => { baseClass.ActivateCellWithCandidateOfEnabledNumber(); } };
        yield return new[] { () => { baseClass.ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ ); } };
        yield return new[] { () => { baseClass.ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ ); } };
        yield return new[] { baseClass.ActivateEraser };
        yield return new[] { baseClass.ActivatePencil };
        yield return new[] { () => { baseClass.ActivatePencil(); baseClass.ActivateEnabledNumber(); } };
        yield return new[] { () => { baseClass.ActivatePencil(); baseClass.ActivateDisabledNumber(); } };
        yield return new[] { () => { baseClass.ActivatePencil(); baseClass.ActivateEmptyCell( out _ ); } };
        yield return new[] { () => { baseClass.ActivatePencil(); baseClass.ActivateLockedCell( out _ ); } };
        yield return new[] { () => { baseClass.ActivatePencil(); baseClass.ActivateCellWithCandidateOfEnabledNumber(); } };
        yield return new[] { () => { baseClass.ActivatePencil(); baseClass.ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ ); } };
        yield return new[] { () => { baseClass.ActivatePencil(); baseClass.ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ ); } };
    }

    protected void CommonAssertions()
    {
        AssertTestedCellValueIsReplaced( originalValue );
        AssertNoNumberIsActive();
        AssertNoNumberIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
        AssertPencilIsNotSelected();
        AssertEraserIsNotSelected();
        AssertIsNotPaused();
    }

    protected int ChangeTestedCellValue()
    {
        int newValue = GameData[ IndexOfTestedCell ].UserFacingValue != 9 ? 9 : 8; // 9 and 8 have no special meaning here, just making sure the value is different

        GameData[ IndexOfTestedCell ].SetUserFacingValueInternal( newValue );
        return newValue;
    }

    private void AssertTestedCellValueIsReplaced( int originalValue )
        => Assert.NotEqual( originalValue, GameData[ IndexOfTestedCell ].UserFacingValue );
}
