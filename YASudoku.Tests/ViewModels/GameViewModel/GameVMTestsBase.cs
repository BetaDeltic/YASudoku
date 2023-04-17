using Moq;
using YASudoku.Common;
using YASudoku.Models.PuzzleGenerator;
using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel;

public class GameVMTestsBase
{
    public readonly GameVM gameVM;

    public NumPadButton? SelectedNumber => gameVM.VisualState?.SelectedNumber;
    public GameGridCellVisualData? SelectedCell => gameVM.VisualState?.SelectedCell;
    public readonly GameGridVisualDataCollection GameData;
    public readonly NumPadVisualState NumPadVS;
    public readonly CommonButtonVisualState EraserVS;
    public readonly VisualStatesHandler VisualState;

    private readonly Mock<IPlayerJournalingService> journalMock = new();

    // Used during board preparation
    public int DisabledNumber = 1; // All numbers are enabled by default, 1 is just a random choice here
    public int EnabledNumber = 2; // These will be enabled during board preparation
    public int DifferentEnabledNumber = 3;

    // Calculated from board data
    public int IndexOfLockedCell;
    public int IndexOfDifferentLockedCell;
    public int IndexOfEmptyCell;
    public int IndexOfDifferentEmptyCell;
    public int IndexOfCellFilledWithCorrectValueOfEnabledNumber;
    public int IndexOfCellFilledWithCorrectValueOfDifferentEnabledNumber;
    public int IndexOfCellFilledWithIncorrectValueOfEnabledNumber;
    public int IndexOfCellFilledWithIncorrectValueOfDifferentEnabledNumber;
    public int IndexOfCellFilledWithCorrectValueOfDisabledNumber;
    public int IndexOfCellFilledWithIncorrectValueOfDisabledNumber;
    public int IndexOfCellWithCandidateOfEnabledNumber;
    public int IndexOfCellWithCandidateOfDisabledNumber;
    public int IndexOfCellWithCandidateOfDifferentEnabledNumber;

    public GameVMTestsBase()
    {
        Mock<IPuzzleGenerator> puzzleMock = new();
        puzzleMock.Setup( x => x.GenerateNewPuzzle() ).Returns( TestsCommon.CreateValidContainerWithoutMissingCells() );

        gameVM = new( puzzleMock.Object, TestsCommon.GetSettingsMock(), TestsCommon.GetServiceProviderMock(), journalMock.Object );
        gameVM.PrepareGameView( true );

        VisualState = gameVM.VisualState ?? throw new SystemException( gameDataNotInitialized );
        GameData = gameVM.VisualState.GameData;
        NumPadVS = gameVM.VisualState.NumPadVS;
        EraserVS = gameVM.VisualState.EraserVS;

        PrepareGameBoard();

        gameVM.VisualState.UpdateAllButtonRemainingCounts();
    }

    private void PrepareGameBoard()
    {
        // Removing value from cells with designated enabled numbers guarantees the will be enabled even after filling a cell with value
        GameData
            .Where( cell => cell.UserFacingValue == EnabledNumber || cell.UserFacingValue == DifferentEnabledNumber )
            .ForEach( cell => cell.SetUserFacingValueInternal( 0 ) );

        // Removing value from one cell that has disabled number in order to put it to incorrect position
        GameData.Where( cell => cell.UserFacingValue == DisabledNumber ).First().SetUserFacingValueInternal( 0 );

        IndexOfLockedCell = GameData.IndexOf( GameData.Where( cell => cell.UserFacingValue != 0 ).First() );
        IndexOfDifferentLockedCell = GameData.IndexOf( GameData.Where( cell => cell.UserFacingValue != 0 ).Skip( 1 ).First() );
        IndexOfCellFilledWithCorrectValueOfEnabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.CorrectValue == EnabledNumber ).First() );
        IndexOfCellFilledWithCorrectValueOfDifferentEnabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.CorrectValue == DifferentEnabledNumber ).First() );
        IndexOfCellFilledWithIncorrectValueOfEnabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.CorrectValue == DifferentEnabledNumber ).Skip( 1 ).First() );
        IndexOfCellFilledWithIncorrectValueOfDifferentEnabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.CorrectValue == EnabledNumber ).Skip( 1 ).First() );
        IndexOfCellFilledWithCorrectValueOfDisabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.UserFacingValue == DisabledNumber ).Skip( 1 ).First() );
        IndexOfCellFilledWithIncorrectValueOfDisabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.CorrectValue == EnabledNumber ).Skip( 2 ).First() );
        IndexOfCellWithCandidateOfEnabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.CorrectValue == EnabledNumber ).Skip( 3 ).First() );
        IndexOfCellWithCandidateOfDisabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.CorrectValue == EnabledNumber ).Skip( 4 ).First() );
        IndexOfCellWithCandidateOfDifferentEnabledNumber =
            GameData.IndexOf( GameData.Where( cell => cell.CorrectValue == DifferentEnabledNumber ).Skip( 2 ).First() );

        GameData[ IndexOfLockedCell ].LockCellInternal();
        GameData[ IndexOfCellFilledWithCorrectValueOfEnabledNumber ].SetUserFacingValueInternal( EnabledNumber );
        GameData[ IndexOfCellFilledWithCorrectValueOfDifferentEnabledNumber ].SetUserFacingValueInternal( DifferentEnabledNumber );
        GameData[ IndexOfCellFilledWithIncorrectValueOfEnabledNumber ].SetUserFacingValueInternal( EnabledNumber );
        GameData[ IndexOfCellFilledWithIncorrectValueOfDifferentEnabledNumber ].SetUserFacingValueInternal( DifferentEnabledNumber );
        GameData[ IndexOfCellFilledWithIncorrectValueOfDisabledNumber ].SetUserFacingValueInternal( DisabledNumber );
        GameData[ IndexOfCellWithCandidateOfEnabledNumber ].AddCandidate( EnabledNumber );
        GameData[ IndexOfCellWithCandidateOfDisabledNumber ].AddCandidate( DisabledNumber );
        GameData[ IndexOfCellWithCandidateOfDifferentEnabledNumber ].AddCandidate( DifferentEnabledNumber );

        IndexOfEmptyCell = GameData.IndexOf( GameData.Where( cell => cell.UserFacingValue == 0 ).First() );
        IndexOfDifferentEmptyCell =
            GameData.IndexOf( GameData.Where( cell => cell.UserFacingValue == 0 ).Skip( 1 ).First() );
    }

    public const string gameDataNotInitialized = $"{nameof( VisualStatesHandler.GameData )} container is not initiated!";

    public void ActivateEraser() => gameVM.SelectEraser();

    public void ActivatePencil() => gameVM.SwitchPenAndPencil();

    // At this point same implementation, semantically used in different contexts
    public void ActivateEmptyCell() => ClickEmptyCell();
    public void ActivateLockedCell() => ClickLockedCell( out _ );
    public void ActivateEnabledNumber() => SelectEnabledNumber();
    public void ActivateDisabledNumber() => SelectDisabledNumber();
    public void ActivateCellWithCandidateOfEnabledNumber() => ClickCellWithCandidateOfEnabledNumber();
    public void ActivateCellFilledWithCorrectValueOfEnabledNumber()
        => ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
    public void ActivateCellFilledWithIncorrectValueOfEnabledNumber()
        => ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );

    public GameGridCellVisualData ClickEmptyCell()
        => ClickCellAndReturnIt( IndexOfEmptyCell, out _ );
    public GameGridCellVisualData ClickDifferentEmptyCell()
        => ClickCellAndReturnIt( IndexOfDifferentEmptyCell, out _ );
    public GameGridCellVisualData ClickLockedCell( out int originalValue )
        => ClickCellAndReturnIt( IndexOfLockedCell, out originalValue );
    public GameGridCellVisualData ClickDifferentLockedCell()
        => ClickCellAndReturnIt( IndexOfDifferentLockedCell, out _ );
    public GameGridCellVisualData ClickCellWithCandidateOfEnabledNumber()
        => ClickCellAndReturnIt( IndexOfCellWithCandidateOfEnabledNumber, out _ );
    public GameGridCellVisualData ClickCellWithCandidateOfDifferentEnabledNumber()
        => ClickCellAndReturnIt( IndexOfCellWithCandidateOfDifferentEnabledNumber, out _ );
    public GameGridCellVisualData ClickCellWithCandidateOfDisabledNumber()
        => ClickCellAndReturnIt( IndexOfCellWithCandidateOfDisabledNumber, out _ );
    public GameGridCellVisualData ClickCellFilledWithCorrectValueOfEnabledNumber( out int originalValue )
        => ClickCellAndReturnIt( IndexOfCellFilledWithCorrectValueOfEnabledNumber, out originalValue );
    public GameGridCellVisualData ClickCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue )
        => ClickCellAndReturnIt( IndexOfCellFilledWithIncorrectValueOfEnabledNumber, out originalValue );
    public GameGridCellVisualData ClickCellFilledWithIncorrectValueOfDifferentEnabledNumber( out int originalValue )
        => ClickCellAndReturnIt( IndexOfCellFilledWithIncorrectValueOfDifferentEnabledNumber, out originalValue );
    public GameGridCellVisualData ClickCellFilledWithCorrectValueOfDisabledNumber( out int originalValue )
        => ClickCellAndReturnIt( IndexOfCellFilledWithCorrectValueOfDisabledNumber, out originalValue );
    public GameGridCellVisualData ClickCellFilledWithIncorrectValueOfDisabledNumber( out int originalValue )
        => ClickCellAndReturnIt( IndexOfCellFilledWithIncorrectValueOfDisabledNumber, out originalValue );
    public GameGridCellVisualData ClickCellFilledWithCorrectValueOfDifferentEnabledNumber( out int originalValue )
        => ClickCellAndReturnIt( IndexOfCellFilledWithCorrectValueOfDifferentEnabledNumber, out originalValue );

    private GameGridCellVisualData ClickCellAndReturnIt( int index, out int originalValue )
    {
        originalValue = GameData[ index ].UserFacingValue;
        gameVM.SelectCell( index );
        return GameData[ index ];
    }

    public void SelectEnabledNumber() => gameVM.PressNumber( EnabledNumber );
    public void SelectDisabledNumber() => gameVM.PressNumber( DisabledNumber );

    public static void AssertCellIsFilledWithSpecificValue( GameGridCellVisualData cell, int expectedValue )
        => Assert.Equal( expectedValue, cell.UserFacingValue );

    public void AssertNoNumberIsSelected() => Assert.Null( SelectedNumber );

    public void AssertEnabledNumberIsSelected() => AssertNumberIsSelected( EnabledNumber );

    public void AssertEnabledNumberIsActive() => AssertNumberIsActive( EnabledNumber );

    public void AssertNumberIsSelected( int buttonNumber )
        => Assert.Equal( GetNumPadButtonFromNumber( buttonNumber ), SelectedNumber );

    public void AssertNumberIsActive( int buttonNumber )
    {
        Assert.True( GetNumPadButtonFromNumber( buttonNumber ).IsActive );
        Assert.Equal( GetNumPadButtonFromNumber( buttonNumber ), SelectedNumber );
    }

    public void AssertNumberIsEnabled( int buttonNumber )
        => Assert.True( GetNumPadButtonFromNumber( buttonNumber ).IsEnabled );

    public void AssertNumberIsDisabled( int buttonNumber )
        => Assert.False( GetNumPadButtonFromNumber( buttonNumber ).IsEnabled );

    public void AssertNumberRemainingCountIsExpected( int buttonNumber, int expectedCount )
        => Assert.Equal( expectedCount, GetNumPadButtonFromNumber( buttonNumber ).RemainingCount );

    public void AssertNoCellIsSelected() => Assert.Null( SelectedCell );

    public void AssertCellIsSelected( GameGridCellVisualData? expectedCell )
        => Assert.Equal( expectedCell, SelectedCell );

    public static void AssertCellContainsCandidate( GameGridCellVisualData? cell, int candidate )
        => Assert.True( cell?.HasNumberAsCandidate( candidate ) );

    public static void AssertCellHasNoCandidates( GameGridCellVisualData? cell )
        => Assert.False( cell?.GetAllCandidateValues().Any() );

    public static void AssertCellDoesNotHaveSpecificCandidate( GameGridCellVisualData? cell, int candidate )
        => Assert.False( cell?.HasNumberAsCandidate( candidate ) );

    public void AssertCellsWithValueAreHighlightedAsSelected( int cellValue )
        => Assert.True( AreCellsWithSameNumberHighlightedAsSelected( cellValue ) );

    public void AssertCellsWithValueAreHighlightedAsSelectedOrRelated( int cellValue )
        => Assert.True( AreCellsWithSameNumberHighlightedAsSelectedOrRelated( cellValue ) );

    public void AssertCellsWithDifferentValueAreNotHighlighted( int cellsValue )
        => Assert.False( AreCellsWithDifferentNumberHighlighted( cellsValue ) );

    public static void AssertCellHasNoValue( GameGridCellVisualData? cell ) => Assert.Equal( 0, cell?.UserFacingValue );

    public void AssertCellIsNotSelectedOrHighlighted( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.NotEqual( cell, SelectedCell );
        Assert.False( cell.IsHighlightedAsRelatedInternal );
        Assert.False( cell.IsHighlightedAsSelectedInternal );
    }

    public void AssertNoCellIsHighlighted() => Assert.False( AreAnyCellsHighlighted() );

    private bool AreAnyCellsHighlighted()
        => GameData.Any( x => x.IsHighlightedAsRelatedInternal || x.IsHighlightedAsSelectedInternal );

    public void AssertNoOtherAndUnrelatedCellsAreHighlighted( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.False( AreOtherAndUnrelatedCellsHighlighted( cell ) );
    }

    public static void AssertCellIsHighlightedAsSelected( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.True( cell.IsHighlightedAsSelectedInternal );
    }

    public void AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.False( AreUnrelatedCellsWithDifferentNumberHighlighted( cell ) );
    }

    public void AssertEraserIsSelected() => Assert.True( VisualState.IsEraserActive );

    public void AssertPencilIsSelected() => Assert.True( VisualState.IsPencilActive );

    public static void AssertRelatedCellsAreHighlightedAsSelected( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.True( AreRelatedCellsHighlightedAsRelated( cell ) );
    }

    public static void AssertRelatedCellsAreNotHighlighted( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.True( AreRelatedCellsUnhighlighted( cell ) );
    }

    public void AssertNoTransactionAddedToJournal()
        => journalMock.Verify(
            x => x.AddTransaction( It.IsAny<PlayerTransactionTypes>(), It.IsAny<GameGridCellVisualData>(),
                It.IsAny<int>() ), Times.Never );

    public NumPadButton GetNumPadButtonFromNumber( int buttonNumber ) => NumPadVS.NumPadButtons[ buttonNumber - 1 ];

    private bool AreCellsWithSameNumberHighlightedAsSelected( int number )
        => GameData
            .Where( cell => cell.UserFacingValue == number )
            .All( cell => cell.IsHighlightedAsSelectedInternal );

    private bool AreCellsWithSameNumberHighlightedAsSelectedOrRelated( int number )
        => GameData
            .Where( cell => cell.UserFacingValue == number )
            .All( cell => cell.IsHighlightedAsSelectedInternal || cell.IsHighlightedAsRelatedInternal );

    private bool AreCellsWithDifferentNumberHighlighted( int number )
        => GameData
            .Where( cell => cell.UserFacingValue != number )
            .Any( cell => cell.IsHighlightedAsSelectedInternal || cell.IsHighlightedAsRelatedInternal );

    private static bool AreRelatedCellsUnhighlighted( GameGridCellVisualData cell )
        => !cell.relatedCells
            .Any( relatedCell => relatedCell.IsHighlightedAsRelatedInternal || relatedCell.IsHighlightedAsSelectedInternal );

    private static bool AreRelatedCellsHighlightedAsRelated( GameGridCellVisualData cell )
        => cell.relatedCells.All( relatedCell => relatedCell.IsHighlightedAsRelatedInternal );

    private bool AreOtherAndUnrelatedCellsHighlighted( GameGridCellVisualData cell )
        => GameData
            .Where( c => c != cell && !cell.relatedCells.Contains( c ) )
            .Any( c => c.IsHighlightedAsRelatedInternal || c.IsHighlightedAsSelectedInternal );

    private bool AreUnrelatedCellsWithDifferentNumberHighlighted( GameGridCellVisualData cell )
        => GameData
            .Where( c => c != cell && !cell.relatedCells.Contains( c ) && c.UserFacingValue != cell.UserFacingValue )
            .Any( c => c.IsHighlightedAsRelatedInternal || c.IsHighlightedAsSelectedInternal );
}
