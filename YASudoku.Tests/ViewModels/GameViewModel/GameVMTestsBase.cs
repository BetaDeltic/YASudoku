using Moq;
using YASudoku.Models.PuzzleGenerator;
using YASudoku.Services.JournalingServices;
using YASudoku.Services.SettingsService;
using YASudoku.ViewModels.GameViewModel;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel;

public class GameVMTestsBase
{
    public readonly GameVM gameVM;

    public GameVMTestsBase()
    {
        Mock<IPuzzleGenerator> puzzleMock = new();
        puzzleMock.Setup( x => x.GenerateNewPuzzle() ).Returns( TestsCommon.CreateValidContainerWithEmptyCells() );

        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock.Setup( x => x.GetService( typeof( IPlayerJournalingService ) ) ).Returns( new PlayerJournalingService() );
        serviceProviderMock.Setup( x => x.GetService( typeof( ISettingsService ) ) ).Returns( TestsCommon.GetSettingsMock() );

        Mock<IPlayerJournalingService> playerJournalingServiceMock = new();

        gameVM = new( puzzleMock.Object, TestsCommon.GetSettingsMock(), serviceProviderMock.Object, playerJournalingServiceMock.Object );
        gameVM.PrepareGameView( true );
        SetAllRemainingCountsToNonZero();
    }

    public const string gameDataNotInitialized = $"{nameof( VisualStatesHandler.GameData )} container is not initiated!";

    public void AssertCellIsFilledWithNumber( int affectedCellIndex, int affectedNumber )
        => Assert.Equal( affectedNumber, gameVM.VisualState?.GameData[ affectedCellIndex ].UserFacingValue );

    public void AssertNoButtonIsSelected()
        => Assert.Null( gameVM.VisualState?.SelectedNumber );

    public void AssertButtonIsPressed( int buttonNumber )
        => Assert.Equal( GetNumPadButtonFromNumber( buttonNumber ), gameVM.VisualState?.SelectedNumber );

    public void AssertButtonIsActive( int buttonNumber )
    {
        Assert.True( GetNumPadButtonFromNumber( buttonNumber ).IsActive );
        Assert.Equal( GetNumPadButtonFromNumber( buttonNumber ), gameVM.VisualState?.SelectedNumber );
    }

    public void AssertButtonIsEnabled( int buttonNumber )
        => Assert.True( GetNumPadButtonFromNumber( buttonNumber ).IsEnabled );

    public void AssertButtonIsDisabled( int buttonNumber )
        => Assert.False( GetNumPadButtonFromNumber( buttonNumber ).IsEnabled );

    public void AssertButtonRemainingCountIsExpected( int buttonNumber, int expectedCount )
        => Assert.Equal( expectedCount, GetNumPadButtonFromNumber( buttonNumber ).RemainingCount );

    public void AssertNoCellIsSelected()
        => Assert.Null( gameVM.VisualState?.SelectedCell );

    public void AssertCellIsSelected( GameGridCellVisualData? expectedCell )
    {
        Assert.NotNull( expectedCell );
        Assert.Equal( expectedCell, gameVM.VisualState?.SelectedCell );
    }

    public void AssertSameCellsAreHighlightedAsSelected( int cellValue )
            => Assert.True( AreCellsWithSameNumberHighlightedAsSelected( cellValue ) );

    public void AssertDifferentCellsAreNotHighlighted( int cellsValue )
        => Assert.False( AreCellsWithDifferentNumberHighlighted( cellsValue ) );

    public static void AssertCellIsEmpty( GameGridCellVisualData? cell )
        => Assert.Equal( 0, cell?.UserFacingValue );

    public void AssertCellIsNotSelectedOrHighlighted( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.NotEqual( cell, gameVM.VisualState?.SelectedCell );
        Assert.False( cell.IsHighlightedAsRelatedInternal );
        Assert.False( cell.IsHighlightedAsSelectedInternal );
    }

    public static void AssertCellIsHighlightedAsSelected( GameGridCellVisualData? cell )
    {
        Assert.NotNull( cell );
        Assert.True( cell.IsHighlightedAsSelectedInternal );
    }

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

    public NumPadButton GetNumPadButtonFromNumber( int buttonNumber )
        => gameVM.VisualState!.NumPadButtons[ buttonNumber - 1 ];

    public bool AreCellsWithSameNumberHighlightedAsSelected( int number )
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );

        IEnumerable<GameGridCellVisualData> cellsWithSameNumber = gameVM.VisualState!.GameData!.Where( cell => cell.UserFacingValue == number );

        return cellsWithSameNumber.All( cell => cell.IsHighlightedAsSelectedInternal );
    }

    public bool AreCellsWithDifferentNumberHighlighted( int number )
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );

        IEnumerable<GameGridCellVisualData> cellsWithDifferentNumber = gameVM.VisualState!.GameData!.Where( cell => cell.UserFacingValue != number );

        return cellsWithDifferentNumber
            .Any( cell => cell.IsHighlightedAsSelectedInternal || cell.IsHighlightedAsRelatedInternal );
    }

    public static bool AreRelatedCellsUnhighlighted( GameGridCellVisualData cell )
        => !cell.relatedCells.Any( relatedCell => relatedCell.IsHighlightedAsRelatedInternal || relatedCell.IsHighlightedAsSelectedInternal );

    public static bool AreRelatedCellsHighlightedAsRelated( GameGridCellVisualData cell )
        => cell.relatedCells.All( relatedCell => relatedCell.IsHighlightedAsRelatedInternal );

    public void SetAllRemainingCountsToNonZero()
        => gameVM.VisualState!.NumPadButtons.ForEach( button => button.UpdateRemainingCount( 1 ) );
}
