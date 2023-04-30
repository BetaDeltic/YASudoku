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
        GameVMTestsBase baseClass = new();
        yield return new object[] { () => { } };
        yield return new object[] { () => { baseClass.ActivateEnabledNumber(); } };
        yield return new object[] { () => { baseClass.ActivateDisabledNumber(); } };
        yield return new object[] { () => { baseClass.ActivateEmptyCell( out _ ); } };
        yield return new object[] { () => { baseClass.ActivateLockedCell( out _ ); } };
        yield return new object[] { () => { baseClass.ActivateCellWithCandidateOfEnabledNumber(); } };
        yield return new object[] { () => { baseClass.ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ ); } };
        yield return new object[] { () => { baseClass.ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ ); } };
        yield return new object[] { () => { baseClass.ActivateEraser(); } };
        yield return new object[] { () => { baseClass.ActivatePencil(); } };
        yield return new object[] { () => { baseClass.ActivatePencil(); baseClass.ActivateEnabledNumber(); } };
        yield return new object[] { () => { baseClass.ActivatePencil(); baseClass.ActivateDisabledNumber(); } };
        yield return new object[] { () => { baseClass.ActivatePencil(); baseClass.ActivateEmptyCell( out _ ); } };
        yield return new object[] { () => { baseClass.ActivatePencil(); baseClass.ActivateLockedCell( out _ ); } };
        yield return new object[] { () => { baseClass.ActivatePencil(); baseClass.ActivateCellWithCandidateOfEnabledNumber(); } };
        yield return new object[] { () => { baseClass.ActivatePencil(); baseClass.ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ ); } };
        yield return new object[] { () => { baseClass.ActivatePencil(); baseClass.ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ ); } };
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
