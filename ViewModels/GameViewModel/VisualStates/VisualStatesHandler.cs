﻿using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Common;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public enum GameStates { Starting, Running, Finished };

public partial class VisualStatesHandler : ObservableObject, IDisposable
{
    public event Action? Victory;
    public event Action? NewGameData;
    public event Action? NewGameAfterFinishedOne;

    public readonly GameGridVisualState GameGridVS;
    public readonly CommonButtonVisualState PencilVS;
    public readonly CommonButtonVisualState EraserVS;
    public readonly CommonButtonVisualState PauseVS;
    public readonly NumPadVisualState NumPadVS;
    public readonly CommonButtonVisualState OtherButtonsVS;
    public readonly TimerVisualState TimerVS = new();
    public readonly SettingsVisualState SettingsVS = new();

    public GameGridVisualDataCollection GameData
    {
        get => _gamedata!;
        set {
            _gamedata = value;
            NewGameData?.Invoke();
        }
    }

    private GameGridVisualDataCollection? _gamedata;

    public GameGridCellVisualData? SelectedCell => GameGridVS.SelectedCell;
    public NumPadButton? SelectedNumber => NumPadVS.SelectedButton;

    public List<NumPadButton> NumPadButtons => NumPadVS.NumPadButtons;

    public bool IsPencilActive => PencilVS.IsActive;
    public bool IsEraserActive => EraserVS.IsActive;
    public bool IsPaused => PauseVS.IsActive;

    public GameStates CurrentGameState = GameStates.Starting;

    private readonly int gridSize;

    public VisualStatesHandler( int gridSize, GameGridVisualDataCollection gameData, IServiceProvider serviceProvider )
    {
        this.gridSize = gridSize;
        GameData = gameData;
        ISettingsService settingsService = serviceProvider.GetService<ISettingsService>()!;

        PencilVS = new( settingsService );
        EraserVS = new( settingsService );
        PauseVS = new( settingsService );
        OtherButtonsVS = new( settingsService );
        NumPadVS = new( gridSize );
        GameGridVS = new( settingsService, gameData );

        NumPadVS.NewNumberSelected += NumPadVS_NewNumberSelected;
        GameGridVS.NumberCountChanged += GameGridVS_NumberCountChanged;

        NewGameData += VisualStatesHandler_NewGameData;
    }

    public void Dispose()
    {
        TimerVS?.Dispose();
        GC.SuppressFinalize( this );
    }

    private void NumPadVS_NewNumberSelected( int newNumberSelected )
        => GameGridVS.HighlightCellsWithSameNumber( newNumberSelected );

    private void GameGridVS_NumberCountChanged( int changedNumber, GameGridCellVisualData changedCell )
    {
        UpdateButtonRemainingCount( changedNumber );
        CheckVictoryConditionsAndNotifyOnVictory( changedCell );
    }

    private void CheckVictoryConditionsAndNotifyOnVictory( GameGridCellVisualData recentlyChangedCell )
    {
        if ( !recentlyChangedCell.HasCorrectValue || ThereAreEmptyCells() || ThereAreIncorrectCells() ) return;

        CurrentGameState = GameStates.Finished;
        TimerVS.StopTimer();
        Victory?.Invoke();
    }

    private bool ThereAreEmptyCells()
        => GameData.Where( cell => !cell.HasUserFacingValue ).Any();

    private bool ThereAreIncorrectCells()
        => GameData.Where( cell => cell.HasUserFacingValue && !cell.HasCorrectValue ).Any();

    private void VisualStatesHandler_NewGameData()
        => GameGridVS.ChangeCellData( GameData );

    public void StartGame()
    {
        TimerVS.StartTimer();
        CurrentGameState = GameStates.Running;
    }

    public void UpdateAllButtonRemainingCounts()
        => Enumerable.Range( 1, gridSize ).ForEach( UpdateButtonRemainingCount );

    public void UpdateButtonRemainingCount( int buttonNumber )
    {
        if ( buttonNumber <= 0 ) {
            return;
        }

        int numberCount = GameGridVS.GetCellsWithSameNumber( buttonNumber ).Count();
        int remainingCount = gridSize - numberCount;

        NumPadVS.UpdateButtonRemainingCount( buttonNumber, remainingCount );
    }

    public void ResetVisualStatesToDefault()
    {
        if ( IsPaused ) {
            GameGridVS.ShowAllCellValues();
            PauseVS.DeactivateButton();
        }

        NumPadVS.DeselectCurrentNumber();
        GameGridVS.DeselectCell();
        GameGridVS.UnhighlightCellsWithSameNumber();

        if ( IsPencilActive )
            PencilVS.DeactivateButton();
    }

    public void PrepareUIForNewGame()
    {
        if ( CurrentGameState == GameStates.Finished ) {
            NewGameAfterFinishedOne?.Invoke();
        }

        StartGame();
    }
}
