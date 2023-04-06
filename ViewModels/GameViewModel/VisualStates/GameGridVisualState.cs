﻿using YASudoku.Common;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public class GameGridVisualState
{
    public event Action<int, GameGridCellVisualData>? NumberCountChanged;

    public GameGridCellVisualData? SelectedCell;

    private GameGridVisualDataCollection? allCells;

    private int highlightedNumber;

    private readonly ISettingsService settings;

    public GameGridVisualState( ISettingsService settings, GameGridVisualDataCollection cellData )
    {
        this.settings = settings;
        allCells = cellData;
    }

    public void ChangeCellData( GameGridVisualDataCollection? cellData ) => allCells = cellData;

    public void SelectNewCell( int cellIndex )
    {
        if ( allCells == null ) return;

        if ( SelectedCell != null ) DeselectCell();

        SelectedCell = allCells[ cellIndex ];

        SelectNewCell();
    }

    public void SelectNewCell( GameGridCellVisualData cell )
    {
        if ( SelectedCell != null ) DeselectCell();

        SelectedCell = cell;

        SelectNewCell();
    }

    private void SelectNewCell()
    {
        HighlightSelectedCell();

        if ( SelectedCell!.HasUserFacingValue ) HighlightCellsWithSameNumber( SelectedCell.UserFacingValue );
    }

    public void DeselectCell()
    {
        if ( SelectedCell == null ) return;

        SelectedCell.UnhighlightCellAndRelatedCells();
        SelectedCell = null;
    }

    public void HighlightSelectedCell()
    {
        if ( SelectedCell == null ) return;

        if ( settings.CanHighlightRelatedCells() ) SelectedCell.HighlightCellAndRelatedCells();
        else SelectedCell.HighlightCellAsSelected();
    }

    public void HighlightCellsWithSameNumber( int number )
    {
        if ( number <= 0 ) return;

        if ( highlightedNumber > 0 ) {
            UnhighlightCellsWithSameNumber();
        }

        highlightedNumber = number;

        GetCellsWithSameNumber( number ).ForEach( cell => cell.HighlightCellAsSelected() );

        GetCellsWithSameCandidateNumber( number ).ForEach( cell => cell.HighlightCandidate( number ) );
    }

    public void UnhighlightCellsWithSameNumber( int selectedNumber )
    {
        if ( highlightedNumber != selectedNumber || selectedNumber == 0 ) return;

        UnhighlightCellsWithSameNumber();
    }

    public void UnhighlightCellsWithSameNumber()
    {
        if ( highlightedNumber == 0 ) return;

        GetCellsWithSameNumber( highlightedNumber ).ForEach( cell => cell.UnhighlightCell() );

        GetCellsWithSameCandidateNumber( highlightedNumber ).ForEach( cell => cell.UnhighlightCandidate() );

        highlightedNumber = 0;
    }

    public void RemoveValueFromSelectedCell()
    {
        if ( SelectedCell == null ) {
            return;
        }

        ChangeSelectedCellValueAndNotify( 0, out _ );
        UnhighlightCellsWithSameNumber();
    }

    public void ChangeSelectedCellValueAndNotify( int newValue, out IEnumerable<GameGridCellVisualData> affectedCells )
    {
        affectedCells = Enumerable.Empty<GameGridCellVisualData>();
        if ( SelectedCell == null || SelectedCell.IsLockedForChanges ) {
            return;
        }

        int previousValue = SelectedCell.UserFacingValue;
        SelectedCell.UserFacingValue = newValue;

        if ( newValue == 0 ) {
            SelectedCell.ShowCandidates();
        } else {
            SelectedCell.ShowValue();
            NumberCountChanged?.Invoke( newValue, SelectedCell );

            SelectedCell.MakeCandidateInvisibleInRelatedCells( newValue, out affectedCells );
        }

        if ( previousValue != 0 ) NumberCountChanged?.Invoke( previousValue, SelectedCell );
    }

    public void HideAllCellValues() => allCells?.ForEach( cell => cell.SetTextColorToTransparent() );

    public void ShowAllCellValues() => allCells?.ForEach( cell => cell.SetTextColorToRegular() );

    public IEnumerable<GameGridCellVisualData> GetCellsWithSameNumber( int number )
        => allCells == null ? Enumerable.Empty<GameGridCellVisualData>() : allCells.Where( cell => cell.UserFacingValue == number );

    public IEnumerable<GameGridCellVisualData> GetCellsWithSameCandidateNumber( int number )
        => allCells == null
            ? Enumerable.Empty<GameGridCellVisualData>()
            : allCells.Where( cell => !cell.HasUserFacingValue && cell.HasNumberAsCandidate( number ) );

    public bool IsCellSelected( int cellIndex )
        => SelectedCell != null && GetCellByIndex( cellIndex ) == SelectedCell;

    public bool IsAnyCellSelected()
        => SelectedCell != null;

    private GameGridCellVisualData? GetCellByIndex( int cellIndex )
        => cellIndex < 0 || cellIndex >= allCells?.Count ? null : allCells?[ cellIndex ];
}