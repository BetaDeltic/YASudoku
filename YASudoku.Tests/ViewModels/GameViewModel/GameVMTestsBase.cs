﻿using Moq;
using YASudoku.Common;
using YASudoku.Models.PuzzleGenerators;
using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel;

public class GameVMTestsBase
{
    public readonly GameVM gameVM;

    public NumPadButton? SelectedNumber => gameVM.VisualState?.SelectedNumber;
    public GameGridCellVisualData? SelectedCell => gameVM.VisualState?.SelectedCell;
    public GameGridVisualDataCollection GameData;
    public readonly NumPadVisualState NumPadVS;
    public readonly CommonButtonVisualState EraserVS;
    public readonly VisualStatesHandler VisualState;

    private readonly List<int> usedIndexes = new();

    // Used during board preparation
    public int DisabledNumber = 1; // All numbers are enabled by default, 1 and 4 are just a random choice here
    public int DifferentDisabledNumber = 4;
    public int EnabledNumber = 2; // These will be enabled during board preparation
    public int DifferentEnabledNumber = 3;

    // Calculated from board data
    public int IndexOfLockedCell;
    public int IndexOfDifferentLockedCell;
    public int IndexOfLockedCellWithValueOfEnabledNumber;
    public int IndexOfLockedCellWithValueOfDisabledNumber;
    public int IndexOfEmptyCell;
    public int IndexOfEmptyRelatedCell;
    public int IndexOfDifferentEmptyCell;
    public int IndexOfCellFilledWithCorrectValueOfEnabledNumber;
    public int IndexOfCellFilledWithCorrectValueOfDifferentEnabledNumber;
    public int IndexOfCellFilledWithIncorrectValueOfEnabledNumber;
    public int IndexOfCellFilledWithIncorrectValueOfDifferentEnabledNumber;
    public int IndexOfCellFilledWithCorrectValueOfDisabledNumber;
    public int IndexOfCellFilledWithCorrectValueOfDifferentDisabledNumber;
    public int IndexOfCellFilledWithIncorrectValueOfDisabledNumber;
    public int IndexOfCellFilledWithIncorrectValueOfDifferentDisabledNumber;
    public int IndexOfCellWithCandidateOfEnabledNumber;
    public int IndexOfCellWithCandidateOfDisabledNumber;
    public int IndexOfCellWithCandidateOfDifferentEnabledNumber;

    public const string gameDataNotInitialized = $"{nameof( VisualStatesHandler.GameData )} container is not initiated!";

    public GameVMTestsBase()
    {
        Mock<IPuzzleGenerator> puzzleMock = new();
        puzzleMock.Setup( x => x.GenerateNewPuzzle() ).Returns( TestsCommon.CreateValidContainerWithoutMissingCells() );

        IPlayerJournalingService journalingService = new PlayerJournalingService();

        gameVM = new( puzzleMock.Object, TestsCommon.GetSettingsMock(),
            TestsCommon.GetServiceProviderMock( journalingService ), journalingService, TestsCommon.GetResourcesMock() );
        gameVM.PrepareGameView( true );

        VisualState = gameVM.VisualState ?? throw new SystemException( gameDataNotInitialized );
        GameData = gameVM.VisualState.GameData;
        NumPadVS = gameVM.VisualState.NumPadVS;
        EraserVS = gameVM.VisualState.EraserVS;

        PrepareGameBoard();

        gameVM.VisualState.UpdateAllButtonRemainingCounts();
        VisualState.StartGame();
    }

    private void PrepareGameBoard()
    {
        // Removing value from cells with designated enabled numbers guarantees the will be enabled even after filling a cell with value
        GameData
            .Where( cell => cell.UserFacingValue == EnabledNumber || cell.UserFacingValue == DifferentEnabledNumber )
            .ForEach( cell => cell.SetUserFacingValueInternal( 0 ) );

        // Removing value from one cell that has disabled number in order to put it to incorrect position
        GameData.Where( cell => cell.UserFacingValue == DisabledNumber ).Last().SetUserFacingValueInternal( 0 );

        IndexOfLockedCell = GetUnusedIndexOfFilledCellAndSaveIt();
        IndexOfDifferentLockedCell = GetUnusedIndexOfFilledCellAndSaveIt();
        IndexOfCellFilledWithCorrectValueOfEnabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( EnabledNumber );
        IndexOfCellFilledWithCorrectValueOfDifferentEnabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( DifferentEnabledNumber );
        IndexOfCellFilledWithIncorrectValueOfEnabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( DifferentEnabledNumber );
        IndexOfCellFilledWithIncorrectValueOfDifferentEnabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( EnabledNumber );
        IndexOfCellFilledWithCorrectValueOfDisabledNumber = GetUnusedIndexOfCellWithSpecificUserFacingValueAndSaveIt( DisabledNumber );
        IndexOfCellFilledWithCorrectValueOfDifferentDisabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( DifferentDisabledNumber );
        IndexOfCellFilledWithIncorrectValueOfDisabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( EnabledNumber );
        IndexOfCellFilledWithIncorrectValueOfDifferentDisabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( DifferentEnabledNumber );
        IndexOfCellWithCandidateOfEnabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( EnabledNumber );
        IndexOfCellWithCandidateOfDisabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( EnabledNumber );
        IndexOfCellWithCandidateOfDifferentEnabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( DifferentEnabledNumber );
        IndexOfLockedCellWithValueOfEnabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( EnabledNumber );
        IndexOfLockedCellWithValueOfDisabledNumber = GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( DisabledNumber );

        GameData[ IndexOfLockedCell ].LockCellInternal();
        GameData[ IndexOfCellFilledWithCorrectValueOfEnabledNumber ].SetUserFacingValueInternal( EnabledNumber );
        GameData[ IndexOfCellFilledWithCorrectValueOfDifferentEnabledNumber ].SetUserFacingValueInternal( DifferentEnabledNumber );
        GameData[ IndexOfCellFilledWithIncorrectValueOfEnabledNumber ].SetUserFacingValueInternal( EnabledNumber );
        GameData[ IndexOfCellFilledWithIncorrectValueOfDifferentEnabledNumber ].SetUserFacingValueInternal( DifferentEnabledNumber );
        GameData[ IndexOfCellFilledWithIncorrectValueOfDisabledNumber ].SetUserFacingValueInternal( DisabledNumber );
        GameData[ IndexOfCellWithCandidateOfEnabledNumber ].AddCandidate( EnabledNumber );
        GameData[ IndexOfCellWithCandidateOfDisabledNumber ].AddCandidate( DisabledNumber );
        GameData[ IndexOfCellWithCandidateOfDifferentEnabledNumber ].AddCandidate( DifferentEnabledNumber );
        GameData[ IndexOfLockedCellWithValueOfEnabledNumber ].SetUserFacingValueInternal( EnabledNumber );
        GameData[ IndexOfLockedCellWithValueOfEnabledNumber ].LockCellInternal();
        GameData[ IndexOfLockedCellWithValueOfDisabledNumber ].SetUserFacingValueInternal( DisabledNumber );
        GameData[ IndexOfLockedCellWithValueOfDisabledNumber ].LockCellInternal();

        IndexOfEmptyCell = GetUnusedIndexOfEmptyCellAndSaveIt();
        IndexOfDifferentEmptyCell = GetUnusedIndexOfEmptyCellAndSaveIt();
        IndexOfEmptyRelatedCell = GetUnusedIndexOfRelatedCellAndSaveIt( IndexOfEmptyCell );

        GameData[ IndexOfEmptyRelatedCell ].SetUserFacingValueInternal( 0 );
        gameVM.journal.ClearJournal();
    }

    private int GetUnusedIndexOfRelatedCellAndSaveIt( int indexOfCell )
    {
        int unusedIndex =
            GameData[ indexOfCell ].relatedCells.Select( ( cell, index ) => !usedIndexes.Contains( index ) ? index : -1 )
            .First( index => index != -1 );
        usedIndexes.Add( unusedIndex );
        return unusedIndex;
    }

    protected int GetUnusedIndexOfFilledCellAndSaveIt()
    {
        int unusedIndex =
            GameData.Select( ( cell, index ) => cell.UserFacingValue != 0 && !usedIndexes.Contains( index ) ? index : -1 )
            .First( index => index != -1 );
        usedIndexes.Add( unusedIndex );

        return unusedIndex;
    }

    protected int GetUnusedIndexOfEmptyCellAndSaveIt()
    {
        int unusedIndex =
            GameData.Select( ( cell, index ) => cell.UserFacingValue == 0 && !usedIndexes.Contains( index ) ? index : -1 )
            .First( index => index != -1 );
        usedIndexes.Add( unusedIndex );

        return unusedIndex;
    }

    private int GetUnusedIndexOfCellWithSpecificCorrectValueAndSaveIt( int expectedCorrectValue )
    {
        int unusedIndex =
            GameData.Select( ( cell, index ) => cell.CorrectValue == expectedCorrectValue && !usedIndexes.Contains( index ) ? index : -1 )
            .First( index => index != -1 );

        usedIndexes.Add( unusedIndex );

        return unusedIndex;
    }

    private int GetUnusedIndexOfCellWithSpecificUserFacingValueAndSaveIt( int expectedUserFacingValue )
    {
        int unusedIndex =
            GameData.Select( ( cell, index ) => cell.UserFacingValue == expectedUserFacingValue && !usedIndexes.Contains( index ) ? index : -1 )
            .First( index => index != -1 );
        usedIndexes.Add( unusedIndex );
        return unusedIndex;
    }

    public void ActivateEraser() => gameVM.SelectEraser();
    public void ActivatePencil() => gameVM.SwitchPenAndPencil();
    public void TogglePause() => gameVM.PauseGame();

    public void ActivateEnabledNumber() => SelectEnabledNumber();
    public void ActivateDisabledNumber() => SelectDisabledNumber();

    // At this point same implementation, semantically used in different contexts
    public GameGridCellVisualData ActivateEmptyCell( out int originalValue ) => ClickEmptyCell( out originalValue );
    public GameGridCellVisualData ActivateLockedCell( out int originalValue ) => ClickLockedCell( out originalValue );
    public GameGridCellVisualData ActivateLockedCellWithValueOfEnabledNumber( out int originalValue )
        => ClickLockedCellWithValueOfEnabledNumber( out originalValue );
    public GameGridCellVisualData ActivateLockedCellWithValueOfDisabledNumber( out int originalValue )
        => ClickLockedCellWithValueOfDisabledNumber( out originalValue );
    public GameGridCellVisualData ActivateCellWithCandidateOfEnabledNumber()
        => ClickCellWithCandidateOfEnabledNumber();
    public GameGridCellVisualData ActivateCellWithCandidateOfDisabledNumber()
        => ClickCellWithCandidateOfDisabledNumber();
    public GameGridCellVisualData ActivateCellFilledWithCorrectValueOfEnabledNumber( out int originalValue )
        => ClickCellFilledWithCorrectValueOfEnabledNumber( out originalValue );
    public GameGridCellVisualData ActivateCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue )
        => ClickCellFilledWithIncorrectValueOfEnabledNumber( out originalValue );
    public GameGridCellVisualData ActivateCellFilledWithCorrectValueOfDisabledNumber( out int originalValue )
        => ClickCellFilledWithCorrectValueOfDisabledNumber( out originalValue );
    public GameGridCellVisualData ActivateCellFilledWithIncorrectValueOfDisabledNumber( out int originalValue )
        => ClickCellFilledWithIncorrectValueOfDisabledNumber( out originalValue );

    public GameGridCellVisualData ClickEmptyCell( out int originalValue )
        => ClickCellAndReturnIt( IndexOfEmptyCell, out originalValue );
    public GameGridCellVisualData ClickDifferentEmptyCell()
        => ClickCellAndReturnIt( IndexOfDifferentEmptyCell, out _ );
    public GameGridCellVisualData ClickRelatedEmptyCell()
        => ClickCellAndReturnIt( IndexOfEmptyRelatedCell, out _ );
    public GameGridCellVisualData ClickLockedCell( out int originalValue )
        => ClickCellAndReturnIt( IndexOfLockedCell, out originalValue );
    public GameGridCellVisualData ClickLockedCellWithValueOfEnabledNumber( out int originalValue )
        => ClickCellAndReturnIt( IndexOfLockedCellWithValueOfEnabledNumber, out originalValue );
    public GameGridCellVisualData ClickLockedCellWithValueOfDisabledNumber( out int originalValue )
        => ClickCellAndReturnIt( IndexOfLockedCellWithValueOfDisabledNumber, out originalValue );
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

    public void ClickUndoButton() => gameVM.UndoLastAction();

    private GameGridCellVisualData ClickCellAndReturnIt( int index, out int originalValue )
    {
        originalValue = GameData[ index ].UserFacingValue;
        gameVM.SelectCell( index );
        return GameData[ index ];
    }

    public void SelectEnabledNumber() => gameVM.PressNumber( EnabledNumber );
    public void SelectDifferentEnabledNumber() => gameVM.PressNumber( DifferentEnabledNumber );
    public void SelectDisabledNumber() => gameVM.PressNumber( DisabledNumber );
    public void SelectDifferentDisabledNumber() => gameVM.PressNumber( DifferentDisabledNumber );

    public static void AssertCellIsFilledWithSpecificValue( GameGridCellVisualData cell, int expectedValue )
        => Assert.Equal( expectedValue, cell.UserFacingValue );

    public void AssertNoNumberIsSelected() => Assert.Null( SelectedNumber );

    public void AssertEnabledNumberIsSelected() => AssertNumberIsSelected( EnabledNumber );

    public void AssertEnabledNumberIsActive() => AssertNumberIsActive( EnabledNumber );

    public void AssertDisabledNumberIsActive() => AssertNumberIsActive( DisabledNumber );

    public void AssertDisabledNumberIsSelected() => AssertNumberIsSelected( DisabledNumber );

    public void AssertNumberIsSelected( int buttonNumber )
        => Assert.Equal( GetNumPadButtonFromNumber( buttonNumber ), SelectedNumber );

    public void AssertNumberIsActive( int buttonNumber )
    {
        Assert.True( GetNumPadButtonFromNumber( buttonNumber ).IsActive );
        Assert.Equal( GetNumPadButtonFromNumber( buttonNumber ), SelectedNumber );
    }

    public void AssertNoNumberIsActive() => Assert.DoesNotContain( NumPadVS.NumPadButtons, button => button.IsActive );

    public void AssertNumberIsNotActive( int buttonNumber )
        => Assert.False( GetNumPadButtonFromNumber( buttonNumber ).IsActive );

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
        Assert.False( cell.IsHighlightedAsRelated );
        Assert.False( cell.IsHighlightedAsSelected );
    }

    public void AssertNoCellIsHighlighted() => Assert.False( AreAnyCellsHighlighted() );

    private bool AreAnyCellsHighlighted()
        => GameData.Any( x => x.IsHighlightedAsRelated || x.IsHighlightedAsSelected );

    public void AssertNoOtherAndUnrelatedCellsAreHighlighted( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.False( AreOtherAndUnrelatedCellsHighlighted( cell ) );
    }

    public static void AssertCellIsHighlightedAsSelected( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.True( cell.IsHighlightedAsSelected );
    }

    public void AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.False( AreUnrelatedCellsWithDifferentNumberHighlighted( cell ) );
    }

    public void AssertAllCellsAreHidingValues() => Assert.False( GameData.Any( cell => !cell.IsHidingAllValues ) );

    public void AssertNoCellIsHidingValues() => Assert.False( GameData.Any( cell => cell.IsHidingAllValues ) );

    public void AssertEraserIsSelected() => Assert.True( VisualState.IsEraserActive );

    public void AssertEraserIsNotSelected() => Assert.False( VisualState.IsEraserActive );

    public void AssertPencilIsSelected() => Assert.True( VisualState.IsPencilActive );

    public void AssertPencilIsNotSelected() => Assert.False( VisualState.IsPencilActive );

    public void AssertIsPaused() => Assert.True( VisualState.IsPaused );

    public void AssertIsNotPaused() => Assert.False( VisualState.IsPaused );

    public void AssertRelatedCellsAreHighlightedAsSelected( GameGridCellVisualData? cell )
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
        => Assert.True( ((PlayerJournalingService)gameVM.journal).TransactionJournal.Count == 0 );

    public NumPadButton GetNumPadButtonFromNumber( int buttonNumber ) => NumPadVS.NumPadButtons[ buttonNumber - 1 ];

    private bool AreCellsWithSameNumberHighlightedAsSelected( int number )
        => GameData
            .Where( cell => cell.UserFacingValue == number )
            .All( cell => cell.IsHighlightedAsSelected );

    private bool AreCellsWithSameNumberHighlightedAsSelectedOrRelated( int number )
        => GameData
            .Where( cell => cell.UserFacingValue == number )
            .All( cell => cell.IsHighlightedAsSelected || cell.IsHighlightedAsRelated );

    private bool AreCellsWithDifferentNumberHighlighted( int number )
        => GameData
            .Where( cell => cell.UserFacingValue != number )
            .Any( cell => cell.IsHighlightedAsSelected || cell.IsHighlightedAsRelated );

    private static bool AreRelatedCellsUnhighlighted( GameGridCellVisualData cell )
        => !cell.relatedCells
            .Any( relatedCell => relatedCell.IsHighlightedAsRelated || relatedCell.IsHighlightedAsSelected );

    private static bool AreRelatedCellsHighlightedAsRelated( GameGridCellVisualData cell )
        => cell.relatedCells.All( relatedCell => relatedCell.IsHighlightedAsRelated );

    private bool AreOtherAndUnrelatedCellsHighlighted( GameGridCellVisualData cell )
        => GameData
            .Where( c => c != cell && !cell.relatedCells.Contains( c ) )
            .Any( c => c.IsHighlightedAsRelated || c.IsHighlightedAsSelected );

    private bool AreUnrelatedCellsWithDifferentNumberHighlighted( GameGridCellVisualData cell )
        => GameData
            .Where( c => c != cell && !cell.relatedCells.Contains( c ) && c.UserFacingValue != cell.UserFacingValue )
            .Any( c => c.IsHighlightedAsRelated || c.IsHighlightedAsSelected );
}
