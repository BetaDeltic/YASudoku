using System.ComponentModel;
using YASudoku.ViewModels.GameViewModel;
using YASudoku.ViewModels.GameViewModel.VisualStates;
using YASudoku.Controls;
using YASudoku.Views.Special;
using System.Reactive.Linq;

namespace YASudoku.Views;

public partial class GamePage : ContentPage
{
    private const int MaxGridSize = 9;
    private const int LeftCellMargin = 5;
    private const int TopCellMargin = 5;
    private const int DefaultCellMargin = 2;

    private readonly GameVM gameVM;

    private readonly int gridSize;

    public GamePage( GameVM gameVM )
    {
        if ( gameVM.VisualState == null )
            throw new ApplicationException( $"Attempted to initialize {nameof( GamePage )} while gameVM is not ready." );

        InitializeComponent();

        this.gameVM = gameVM;
        BindingContext = gameVM;
        gridSize = gameVM.gridSize;

        InitializeGameGrid();
        InitializeNumberPad();

        InitializeButtons();

        SettingsFlyout.ServiceProvider = gameVM.serviceProvider;
        SettingsFlyout.SetBinding( SettingsFlyout.IsFlyoutVisibleProperty,
            new Binding( nameof( SettingsVisualState.AreSettingsVisible ), BindingMode.TwoWay, source: gameVM.VisualState.SettingsVS ) );

        TimerLbl.SetBinding( Label.TextProperty, new Binding( nameof( TimerVisualState.TimerText ), source: gameVM.VisualState.TimerVS ) );

        gameVM.VisualState.Victory += GameVM_Victory;
        gameVM.VisualState.SignalWhenWipingGameBoard.Subscribe( _ => RunAbortedGameAnimation() );
        gameVM.VisualState.SignalWhenStartingNewGame.Subscribe( _ => RunStartingNewGameAnimation() );

        PropertyChanged += GamePage_PropertyChanged;
        Appearing += GamePage_Appearing;
    }

    private void InitializeGameGrid()
    {
        if ( gridSize > MaxGridSize ) {
            throw new ArgumentOutOfRangeException( $"Attempted to create a game with more than {MaxGridSize}x{MaxGridSize} fields." );
        }

        SetColumnAndRowDefinitions( GameGrid, InitGridDefinitions );
        InitializeIndividualGridCells();

        GameGrid.PropertyChanged += GameGrid_PropertyChanged;
    }

    private static void SetColumnAndRowDefinitions( Grid grid, Action<ColumnDefinitionCollection, RowDefinitionCollection> action )
    {
        ColumnDefinitionCollection columns = new();
        RowDefinitionCollection rows = new();

        action( columns, rows );

        grid.ColumnDefinitions = columns;
        grid.RowDefinitions = rows;
    }

    private void InitGridDefinitions( ColumnDefinitionCollection columns, RowDefinitionCollection rows )
    {
        for ( int i = 0; i < gridSize; i++ ) {
            columns.Add( new ColumnDefinition( GridLength.Star ) );
            rows.Add( new RowDefinition( GridLength.Star ) );
        }
    }

    private void InitializeIndividualGridCells()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new ApplicationException( "Trying to initialize grid cells without game data." );

        var lineLength = gridSize switch {
            9 => 3,
            _ => throw new ArgumentOutOfRangeException( nameof( gridSize ), "Unsupported grid size" ),
        };

        for ( int column = 0; column < gridSize; column++ ) {
            for ( int row = 0; row < gridSize; row++ ) {
                LabelOrNumberedGrid gridCell = CreateNewCell( row, column, lineLength );
                InitCellBindings( gridCell );

                GameGrid.SetColumn( gridCell, column );
                GameGrid.SetRow( gridCell, row );
                GameGrid.Children.Add( gridCell );
            }
        }
    }

    private LabelOrNumberedGrid CreateNewCell( int row, int column, int lineLength )
    {
        int cellId = row * gridSize + column;
        bool hasLeftEdge = column != 0 && column % 3 == 0;
        bool hasTopEdge = row != 0 && row % 3 == 0;
        LabelOrNumberedGrid gridCell = new( lineLength, gameVM!.VisualState!.GameData[ cellId ].CandidatesGridProperties ) {
            Margin = new Thickness {
                Left = hasLeftEdge ? LeftCellMargin : DefaultCellMargin,
                Top = hasTopEdge ? TopCellMargin : DefaultCellMargin
            },
            BindingContext = gameVM.VisualState.GameData[ cellId ],
            CommandParameter = cellId,
            AutomationId = cellId.ToString(),
        };
        return gridCell;
    }

    private void InitCellBindings( LabelOrNumberedGrid gridCell )
    {
        gridCell.SetBinding( LabelOrNumberedGrid.TextProperty, nameof( GameGridCellVisualData.UserFacingText ) );
        gridCell.SetBinding( LabelOrNumberedGrid.FontAttributesProperty, nameof( GameGridCellVisualData.FontAttribute ) );
        gridCell.SetBinding( LabelOrNumberedGrid.TextColorProperty, nameof( GameGridCellVisualData.TextColor ) );
        gridCell.SetBinding( LabelOrNumberedGrid.BackgroundColorProperty, nameof( GameGridCellVisualData.BackgroundColor ) );
        gridCell.SetBinding( LabelOrNumberedGrid.IsLabelVisibleProperty, nameof( GameGridCellVisualData.IsShowingValue ) );
        gridCell.SetBinding( LabelOrNumberedGrid.IsGridVisibleProperty, nameof( GameGridCellVisualData.IsShowingCandidates ) );
        gridCell.SetBinding( LabelOrNumberedGrid.GridBindingListProperty, nameof( GameGridCellVisualData.CandidatesGridProperties ) );
        gridCell.SetBinding( LabelOrNumberedGrid.CommandProperty, new Binding( nameof( GameVM.SelectCellCommand ) ) { Source = gameVM } );
    }

    private async void GameGrid_PropertyChanged( object? sender, PropertyChangedEventArgs e )
    {
        // Workaround for having Grid of same Height and Width, until https://github.com/dotnet/maui/issues/11789 is fixed.
        if ( e.PropertyName != nameof( GameGrid.Width ) ) return;

        await Dispatcher.DispatchAsync( () => { } ); // Wait for UI to be ready before measuring the width

        if ( !( GameGrid.Width > 0 ) ) return;

        GameGrid.HeightRequest = GameGrid.Width;
        GameGrid.PropertyChanged -= GameGrid_PropertyChanged;
    }

    private void InitializeNumberPad()
    {
        SetColumnAndRowDefinitions( NumberPad, InitNumPadDefinitions );

        for ( int number = 0; number < gridSize; number++ ) {
            ButtonWithSubText button = new() {
                Text = $"{number + 1}",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                CommandParameter = number + 1,
                BindingContext = gameVM.VisualState!.NumPadButtons[ number ],
            };

            button.SetBinding( ButtonWithSubText.BackgroundColorProperty, nameof( NumPadButton.BackgroundColor ) );
            button.SetBinding( ButtonWithSubText.TextColorProperty, nameof( NumPadButton.TextColor ) );
            button.SetBinding( ButtonWithSubText.SubTextProperty, nameof( NumPadButton.RemainingCount ) );
            button.SetBinding( ButtonWithSubText.CommandProperty, new Binding() { Path = nameof( gameVM.PressNumberCommand ), Source = gameVM } );

            NumberPad.SetRow( button, 0 );
            NumberPad.SetColumn( button, number );
            NumberPad.Children.Add( button );
        }
    }

    private void InitNumPadDefinitions( ColumnDefinitionCollection columns, RowDefinitionCollection rows )
    {
        for ( int i = 0; i < gridSize; i++ ) {
            columns.Add( new ColumnDefinition( GridLength.Star ) );
        }
        rows.Add( new RowDefinition( GridLength.Star ) );
    }

    private void InitializeButtons()
    {
        CommonButtonVisualState commonButtonVS = gameVM.VisualState!.OtherButtonsVS;
        SetCommonButtonBindings( NewGameBtn, commonButtonVS );
        SetButtonCommandBinding( NewGameBtn, nameof( GameVM.StartNewGameCommand ) );
        SetCommonButtonBindings( RestartBtn, commonButtonVS );
        SetButtonCommandBinding( RestartBtn, nameof( GameVM.RestartGameCommand ) );
        SetCommonButtonBindings( SettingsBtn, commonButtonVS );
        SetButtonCommandBinding( SettingsBtn, nameof( GameVM.OpenSettingsCommand ) );
        SetCommonButtonBindings( UndoBtn, commonButtonVS );
        SetButtonCommandBinding( UndoBtn, nameof( GameVM.UndoLastActionCommand ) );

        PauseBtn.BindingContext = gameVM.VisualState.PauseVS;
        SetCommonButtonBindings( PauseBtn );
        SetButtonCommandBinding( PauseBtn, nameof( GameVM.PauseGameCommand ), gameVM );

        PencilBtn.BindingContext = gameVM.VisualState.PencilVS;
        SetCommonButtonBindings( PencilBtn );
        SetButtonCommandBinding( PencilBtn, nameof( GameVM.SwitchPenAndPencilCommand ), gameVM );

        EraseBtn.BindingContext = gameVM.VisualState.EraserVS;
        SetCommonButtonBindings( EraseBtn );
        SetButtonCommandBinding( EraseBtn, nameof( GameVM.SelectEraserCommand ), gameVM );
    }

    private void SetCommonButtonBindings( Button button, object? bindingSource = null )
    {
        if ( gameVM.VisualState == null ) throw new ApplicationException( "GameVM visual state is not initialized during bindings." );

        if ( bindingSource == null ) {
            button.SetBinding( Button.TextColorProperty, nameof( CommonButtonVisualState.TextColor ) );
            button.SetBinding( BackgroundColorProperty, nameof( CommonButtonVisualState.BackgroundColor ) );
            return;
        }

        button.SetBinding( Button.TextColorProperty, new Binding( nameof( CommonButtonVisualState.TextColor ), source: bindingSource ) );
        button.SetBinding( BackgroundColorProperty, new Binding( nameof( CommonButtonVisualState.BackgroundColor ), source: bindingSource ) );
    }

    private static void SetButtonCommandBinding( Button button, string commandPath, object? source = null )
    {
        if ( source == null )
            button.SetBinding( Button.CommandProperty, commandPath );
        else
            button.SetBinding( Button.CommandProperty, new Binding( commandPath, source: source ) );
    }

    private void GameVM_Victory() => RunVictoryAnimation();

    private void GamePage_Appearing( object? sender, EventArgs e ) => gameVM.OnPageAppearing();

    private async void GamePage_PropertyChanged( object? sender, PropertyChangedEventArgs e )
    {
        if ( e.PropertyName != nameof( Width ) ) return;

        await Dispatcher.DispatchAsync( () => { } ); // Wait for UI to be ready before measuring the width

        if ( !( Width > 0 ) ) return;

        SettingsFlyout.SetInitialFlyoutPosition();
        PropertyChanged -= GamePage_PropertyChanged;
    }
}
