using Moq;
using YASudoku.Models;
using YASudoku.Models.PuzzleGenerator;
using YASudoku.Services.JournalingServices;
using YASudoku.Services.SettingsService;
using YASudoku.ViewModels.GameViewModel;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels;

public class GameVMTests
{
    private readonly GameVM gameVM;

    public GameVMTests()
    {
        Mock<IPuzzleGenerator> puzzleMock = new();
        puzzleMock.Setup( x => x.GenerateNewPuzzle() ).Returns( TestsCommon.CreateValidContainerWithEmptyCells() );

        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock.Setup( x => x.GetService( typeof( ISettingsService ) ) ).Returns( TestsCommon.GetSettingsMock() );

        Mock<IPlayerJournalingService> playerJournalingServiceMock = new();

        gameVM = new( puzzleMock.Object, serviceProviderMock.Object, playerJournalingServiceMock.Object );
        gameVM.PrepareGameView( true );
        SetAllRemainingCountsToNonZero();
    }

    [Fact]
    public void PressNumberCommand_WithoutActiveNumber_ActivatesNumber_HighlightsOnlyCellsWithSameNumber()
    {
        // Arrange
        gameVM.VisualState!.NumPadVS.DeselectCurrentNumber();
        gameVM.VisualState.GameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Range( 1, 2 ) );

        // Act
        const int pressedNumber = 1;
        gameVM.PressNumber( pressedNumber );

        // Assert
        AssertButtonIsPressed( pressedNumber );
        AssertButtonIsEnabled( pressedNumber );
        AssertSameCellsAreHighlightedAsSelected( pressedNumber );
        AssertDifferentCellsAreNotHighlighted( pressedNumber );
    }

    [Fact]
    public void PressNumberCommand_WithActiveNumber_DeactivatesPreviousOneActivatesNewOne_HighlightsOnlyCellWithSameNumber()
    {
        // Arrange
        gameVM.VisualState!.NumPadVS.SelectedButtonNumber = 1;
        gameVM.VisualState.GameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Range( 1, 2 ) );
        const int pressedNumber = 2;
        GetNumPadButtonFromNumber( pressedNumber ).UpdateRemainingCount( 9 );

        // Act
        gameVM.PressNumber( pressedNumber );

        // Assert
        AssertButtonIsPressed( pressedNumber );
        AssertButtonIsEnabled( pressedNumber );
        AssertSameCellsAreHighlightedAsSelected( pressedNumber );
        AssertDifferentCellsAreNotHighlighted( pressedNumber );
    }

    [Fact]
    public void UpdateNumberCount_ToZero_DisablesGivenNumber_KeepsItActiveForRemovals()
    {
        // Arrange
        gameVM.VisualState!.NumPadVS.DeselectCurrentNumber();
        const int affectedNumber = 1;
        gameVM.VisualState.GameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Repeat( affectedNumber, 9 ) );
        gameVM.PressNumber( affectedNumber );

        // Act
        gameVM.VisualState.UpdateButtonRemainingCount( affectedNumber );

        // Assert
        AssertButtonIsDisabled( affectedNumber );
        AssertButtonIsActive( affectedNumber );
        AssertButtonRemainingCountIsExpected( affectedNumber, 0 );
    }

    [Fact]
    public void PressNumberCommand_OnDisabledNumber_HavingSameNumberSelectedOnGrid_RemovesTheNumberFromGrid_KeepsCellSelected_ReEnablesTheNumber()
    {
        // Arrange
        gameVM.VisualState!.NumPadVS.DeselectCurrentNumber();
        const int affectedNumber = 1;
        gameVM.VisualState.GameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Repeat( affectedNumber, 9 ) );
        gameVM.VisualState.UpdateButtonRemainingCount( 1 );
        gameVM.VisualState.GameGridVS.SelectNewCell( 0 );

        // Act
        GameGridCell? affectedCell = gameVM.VisualState.SelectedCell;
        gameVM.PressNumber( affectedNumber );

        // Assert
        AssertNoButtonIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsEmpty( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
    }

    [Fact]
    public void SelectCellCommand_WithoutActiveNumber_HighlightsCellAndRelatedCells()
    {
        // Arrange
        gameVM.VisualState!.NumPadVS.DeselectCurrentNumber();
        const int affectedCellIndex = 0;
        gameVM.VisualState.GameData = TestsCommon.CreateValidContainerWithEmptyCells();

        // Act
        gameVM.SelectCell( affectedCellIndex );
        GameGridCell? affectedCell = gameVM.VisualState.SelectedCell;

        // Assert
        AssertNoButtonIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
    }

    [Fact]
    public void SelectCellCommand_WithoutActiveNumber_WithDifferentCellActive_HighlightsCellAndRelatedCells()
    {
        // Arrange
        gameVM.VisualState!.NumPadVS.DeselectCurrentNumber();
        const int affectedCellIndex = 0;
        gameVM.VisualState.GameData = TestsCommon.CreateValidContainerWithEmptyCells();
        gameVM.VisualState.GameGridVS.SelectNewCell( 1 );

        // Act
        gameVM.SelectCell( affectedCellIndex );
        GameGridCell? affectedCell = gameVM.VisualState.SelectedCell;

        // Assert
        AssertNoButtonIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
    }

    [Fact]
    public void SelectCellCommand_WithoutActiveNumber_WithSameCellActive_UnhighlightsCellAndRelatedCells()
    {
        // Arrange
        gameVM.VisualState!.NumPadVS.DeselectCurrentNumber();
        const int affectedCellIndex = 0;
        gameVM.VisualState.GameData = TestsCommon.CreateValidContainerWithEmptyCells();
        gameVM.VisualState.GameGridVS.SelectNewCell( affectedCellIndex );

        // Act
        gameVM.SelectCell( affectedCellIndex );
        GameGridCell? affectedCell = gameVM.VisualState.GameData.AllCells[ affectedCellIndex ];

        // Assert
        AssertNoButtonIsSelected();
        AssertNoCellIsSelected();
        AssertCellIsNotSelectedOrHighlighted( affectedCell );
        AssertRelatedCellsAreNotHighlighted( affectedCell );
    }

    // Test that verifies that cell is filled and cells with same number are highlighted when I select a number and then click on a cell.
    [Fact]
    public void SelectCellCommand_WithActiveNumber_FillsCellWithNumber_HighlightsAllCellsWithSameNumber()
    {
        // Arrange
        gameVM.VisualState!.NumPadVS.DeselectCurrentNumber();
        const int affectedCellIndex = 0;
        const int affectedNumber = 1;
        gameVM.VisualState.GameData = TestsCommon.CreateValidContainerWithEmptyCells();
        gameVM.PressNumber( affectedNumber );

        // Act
        gameVM.SelectCell( affectedCellIndex );

        // Assert
        AssertButtonIsPressed( affectedNumber );
        AssertButtonIsEnabled( affectedNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithNumber( affectedCellIndex, affectedNumber );
    }

    private void AssertCellIsFilledWithNumber( int affectedCellIndex, int affectedNumber )
        => Assert.Equal( affectedNumber, gameVM.VisualState!.GameData.AllCells[ affectedCellIndex ].UserFacingValue );

    private void AssertNoButtonIsSelected()
        => Assert.Null( gameVM.VisualState?.SelectedNumber );

    private void AssertButtonIsPressed( int buttonNumber )
        => Assert.Equal( GetNumPadButtonFromNumber( buttonNumber ), gameVM.VisualState?.SelectedNumber );

    private void AssertButtonIsActive( int buttonNumber )
    {
        Assert.True( GetNumPadButtonFromNumber( buttonNumber ).IsActive );
        Assert.Equal( GetNumPadButtonFromNumber( buttonNumber ), gameVM.VisualState?.SelectedNumber );
    }

    private void AssertButtonIsEnabled( int buttonNumber )
        => Assert.True( GetNumPadButtonFromNumber( buttonNumber ).IsEnabled );

    private void AssertButtonIsDisabled( int buttonNumber )
        => Assert.False( GetNumPadButtonFromNumber( buttonNumber ).IsEnabled );

    private void AssertButtonRemainingCountIsExpected( int buttonNumber, int expectedCount )
        => Assert.Equal( expectedCount, GetNumPadButtonFromNumber( buttonNumber ).RemainingCount );

    private void AssertNoCellIsSelected()
        => Assert.Null( gameVM.VisualState?.SelectedCell );

    private void AssertCellIsSelected( GameGridCell? expectedCell )
    {
        Assert.NotNull( expectedCell );
        Assert.Equal( expectedCell, gameVM.VisualState?.SelectedCell );
    }

    private void AssertSameCellsAreHighlightedAsSelected( int cellValue )
            => Assert.True( AreCellsWithSameNumberHighlightedAsSelected( cellValue ) );

    private void AssertDifferentCellsAreNotHighlighted( int cellsValue )
        => Assert.False( AreCellsWithDifferentNumberHighlighted( cellsValue ) );

    private static void AssertCellIsEmpty( GameGridCell? cell )
        => Assert.Equal( 0, cell?.UserFacingValue );

    private void AssertCellIsNotSelectedOrHighlighted( GameGridCell? cell )
    {
        Assert.NotNull( cell );
        Assert.NotEqual( cell, gameVM.VisualState?.SelectedCell );
        Assert.False( cell.IsHighlightedAsRelatedInternal );
        Assert.False( cell.IsHighlightedAsSelectedInternal );
    }

    private static void AssertCellIsHighlightedAsSelected( GameGridCell? cell )
    {
        Assert.NotNull( cell );
        Assert.True( cell.IsHighlightedAsSelectedInternal );
    }

    private static void AssertRelatedCellsAreHighlightedAsSelected( GameGridCell? cell )
    {
        Assert.NotNull( cell );
        Assert.True( AreRelatedCellsHighlightedAsRelated( cell ) );
    }

    private static void AssertRelatedCellsAreNotHighlighted( GameGridCell? cell )
    {
        Assert.NotNull( cell );
        Assert.True( AreRelatedCellsUnhighlighted( cell ) );
    }

    private NumPadButton GetNumPadButtonFromNumber( int buttonNumber )
        => gameVM.VisualState!.NumPadButtons[ buttonNumber - 1 ];

    private bool AreCellsWithSameNumberHighlightedAsSelected( int number )
    {
        GameDataNullContainerCheck();

        IEnumerable<GameGridCell> cellsWithSameNumber = gameVM.VisualState!.GameData!.AllCells.Where( cell => cell.UserFacingValue == number );

        return cellsWithSameNumber.All( cell => cell.IsHighlightedAsSelectedInternal );
    }

    private bool AreCellsWithDifferentNumberHighlighted( int number )
    {
        GameDataNullContainerCheck();

        IEnumerable<GameGridCell> cellsWithDifferentNumber = gameVM.VisualState!.GameData!.AllCells.Where( cell => cell.UserFacingValue != number );

        return cellsWithDifferentNumber
            .Any( cell => cell.IsHighlightedAsSelectedInternal || cell.IsHighlightedAsRelatedInternal );
    }

    private static bool AreRelatedCellsUnhighlighted( GameGridCell cell )
        => !cell.relatedCells.Any( relatedCell => relatedCell.IsHighlightedAsRelatedInternal || relatedCell.IsHighlightedAsSelectedInternal );

    private static bool AreRelatedCellsHighlightedAsRelated( GameGridCell cell )
        => cell.relatedCells.All( relatedCell => relatedCell.IsHighlightedAsRelatedInternal );

    private void SetAllRemainingCountsToNonZero()
        => gameVM.VisualState!.NumPadButtons.ForEach( button => button.UpdateRemainingCount( 1 ) );

    private void GameDataNullContainerCheck()
    {
        if ( gameVM.VisualState!.GameData == null )
            throw new SystemException( $"{nameof( VisualStatesHandler.GameData )} container is not initiated!" );
    }
}
