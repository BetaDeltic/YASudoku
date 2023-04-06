using System.ComponentModel;
using YASudoku.Models;
using YASudoku.ViewModels.GameViewModel;
using YASudoku.ViewModels.GameViewModel.VisualStates;
using YASudoku.Controls;
using YASudoku.Views.Special;

namespace YASudoku.Views;

public partial class GamePage : ContentPage
{
    private const int MaxGridSize = 9;

    private readonly GameVM gameVM;

    private readonly TimeSpan minAnimationSpan = TimeSpan.FromMilliseconds( 500 );
    private readonly TimeSpan maxAnimationSpan = TimeSpan.FromMilliseconds( 1000 );

    public GamePage( GameVM gameVM )
    {
        InitializeComponent();
        this.gameVM = gameVM;
        BindingContext = gameVM;

        InitializeGameGrid( gameVM.gridSize );
        InitializeNumberPad( gameVM.gridSize );

        NewGameBtn.SetBinding( Button.CommandProperty, nameof( GameVM.StartNewGameCommand ) );
        SetCommonButtonBindings( NewGameBtn );
        RestartBtn.SetBinding( Button.CommandProperty, nameof( GameVM.RestartGameCommand ) );
        SetCommonButtonBindings( RestartBtn );

        PauseBtn.BindingContext = gameVM.VisualState!.PauseVS;
        PauseBtn.SetBinding( Button.TextColorProperty, nameof( CommonButtonVisualState.TextColor ) );
        PauseBtn.SetBinding( BackgroundColorProperty, nameof( CommonButtonVisualState.BackgroundColor ) );
        PauseBtn.SetBinding( Button.CommandProperty, new Binding( nameof( GameVM.PauseGameCommand ), source: gameVM ) );

        PencilBtn.BindingContext = gameVM.VisualState!.PencilVS;
        PencilBtn.SetBinding( Button.TextColorProperty, nameof( CommonButtonVisualState.TextColor ) );
        PencilBtn.SetBinding( BackgroundColorProperty, nameof( CommonButtonVisualState.BackgroundColor ) );
        PencilBtn.SetBinding( Button.CommandProperty, new Binding( nameof( GameVM.SwitchPenAndPencilCommand ), source: gameVM ) );

        EraseBtn.BindingContext = gameVM.VisualState!.EraserVS;
        EraseBtn.SetBinding( Button.TextColorProperty, nameof( CommonButtonVisualState.TextColor ) );
        EraseBtn.SetBinding( BackgroundColorProperty, nameof( CommonButtonVisualState.BackgroundColor ) );
        EraseBtn.SetBinding( Button.CommandProperty, new Binding( nameof( GameVM.SelectEraserCommand ), source: gameVM ) );

        SettingsBtn.SetBinding( Button.CommandProperty, nameof( GameVM.OpenSettingsCommand ) );
        SetCommonButtonBindings( SettingsBtn );

        SettingsFlyout.ServiceProvider = gameVM.serviceProvider;
        SettingsFlyout.SetBinding( SettingsFlyout.IsFlyoutVisibleProperty, new Binding( nameof( VisualStatesHandler.AreSettingsVisible ), BindingMode.TwoWay, source: gameVM.VisualState ) );

        TimerLbl.SetBinding( Label.TextProperty, new Binding( nameof( VisualStatesHandler.TimerText ), source: gameVM.VisualState ) );

        UndoBtn.SetBinding( Button.CommandProperty, nameof( GameVM.UndoLastActionCommand ) );

        SetCommonButtonBindings( UndoBtn );

        gameVM.VisualState.Victory += GameVM_Victory;
        gameVM.NewGameAfterFinishedOne += GameVM_NewGameAfterFinishedOne;
    }

    private void InitializeGameGrid( int gridSize )
    {
        if ( gridSize > MaxGridSize ) {
            throw new ArgumentOutOfRangeException( $"Attempted to create a game with more than {MaxGridSize}x{MaxGridSize} fields." );
        }

        SetColumnAndRowGridDefinitions( gridSize );
        InitializeIndividualGridCells( gridSize );

        GameGrid.PropertyChanged += GameGrid_PropertyChanged;
    }

    private void SetColumnAndRowGridDefinitions( int gridSize )
    {
        ColumnDefinitionCollection columns = new();
        RowDefinitionCollection rows = new();
        for ( int i = 0; i < gridSize; i++ ) {
            columns.Add( new ColumnDefinition( GridLength.Star ) );
            rows.Add( new RowDefinition( GridLength.Star ) );
        }

        GameGrid.ColumnDefinitions = columns;
        GameGrid.RowDefinitions = rows;
    }

    private void InitializeIndividualGridCells( int gridSize )
    {
        if ( gameVM?.VisualState?.GameData == null ) throw new ApplicationException( "Trying to initialize grid cells without game data." );

        const int leftMargin = 5;
        const int topMargin = 5;
        const int defaultMargin = 2;

        var lineLength = gridSize switch {
            9 => 3,
            _ => throw new ArgumentOutOfRangeException( nameof( gridSize ), "Unsupported grid size" ),
        };

        for ( int column = 0; column < gridSize; column++ ) {
            for ( int row = 0; row < gridSize; row++ ) {
                int cellId = row * gridSize + column;
                bool hasLeftEdge = column != 0 && column % 3 == 0;
                bool hasTopEdge = row != 0 && row % 3 == 0;
                LabelOrNumberedGrid gridCell = new( lineLength, gameVM!.VisualState!.GameData![ cellId ].CandidatesGridProperties ) {
                    Margin = new Thickness {
                        Left = hasLeftEdge ? leftMargin : defaultMargin,
                        Top = hasTopEdge ? topMargin : defaultMargin
                    },
                    BindingContext = gameVM?.VisualState.GameData?[ cellId ],
                    CommandParameter = cellId,
                    AutomationId = cellId.ToString(),
                };
                gridCell.SetBinding( LabelOrNumberedGrid.TextProperty, nameof( GameGridCellVisualData.UserFacingText ) );
                gridCell.SetBinding( LabelOrNumberedGrid.FontAttributesProperty, nameof( GameGridCellVisualData.FontAttribute ) );
                gridCell.SetBinding( LabelOrNumberedGrid.TextColorProperty, nameof( GameGridCellVisualData.TextColor ) );
                gridCell.SetBinding( LabelOrNumberedGrid.BackgroundColorProperty, nameof( GameGridCellVisualData.BackgroundColor ) );
                gridCell.SetBinding( LabelOrNumberedGrid.IsLabelVisibleProperty, nameof( GameGridCellVisualData.IsShowingValue ) );
                gridCell.SetBinding( LabelOrNumberedGrid.IsGridVisibleProperty, nameof( GameGridCellVisualData.IsShowingCandidates ) );
                gridCell.SetBinding( LabelOrNumberedGrid.GridBindingListProperty, nameof( GameGridCellVisualData.CandidatesGridProperties ) );

                gridCell.SetBinding( LabelOrNumberedGrid.CommandProperty, new Binding( nameof( gameVM.SelectCellCommand ) ) { Source = gameVM } );

                GameGrid.SetColumn( gridCell, column );
                GameGrid.SetRow( gridCell, row );
                GameGrid.Children.Add( gridCell );
            }
        }
    }

    private async void GameGrid_PropertyChanged( object? sender, PropertyChangedEventArgs e )
    {
        // Workaround for having Grid of same Height and Width, until https://github.com/dotnet/maui/issues/11789 is fixed.
        if ( e.PropertyName != nameof( GameGrid.Width ) ) return;

        await Dispatcher.DispatchAsync( () => { } ); // Wait for UI to be ready before measuring the width
        if ( GameGrid.Width > 0 ) {
            GameGrid.HeightRequest = GameGrid.Width;
        }
    }


    private void SetCommonButtonBindings( Button button )
    {
        button.SetBinding( Button.TextColorProperty, new Binding( nameof( CommonButtonVisualState.TextColor ), source: gameVM.VisualState!.OtherButtonsVS ) );
        button.SetBinding( BackgroundColorProperty, new Binding( nameof( CommonButtonVisualState.BackgroundColor ), source: gameVM.VisualState!.OtherButtonsVS ) );
    }

    private async void GameVM_Victory()
    {
        _ = Dispatcher.DispatchAsync( () => RunAnimationOnAllCellsAtRandomOrder( BlackHoleAnimation ) );
        _ = Dispatcher.DispatchAsync( MoveIrrelevantButtonsAwayAfterVictory );
        await Task.Delay( maxAnimationSpan );
        _ = Dispatcher.DispatchAsync( MoveTimerToVictoryPosition );
        _ = Dispatcher.DispatchAsync( MoveRelevantButtonsBelowTimerAfterVictory );
    }

    private void RunAnimationOnAllCellsAtRandomOrder( Action<LabelOrNumberedGrid, TimeSpan> animation )
    {
        List<int> cellIndexes = new( Enumerable.Range( 0, GameGrid.Children.Count ) );

        Random random = new();
        while ( cellIndexes.Count > 0 ) {
            int cellIndex = random.Next( cellIndexes.Count );
            IView child = GameGrid.Children[ cellIndexes[ cellIndex ] ];
            LabelOrNumberedGrid cell = (LabelOrNumberedGrid)child;
            int animationLength = GetRandomAnimationLength( random );
            Dispatcher.DispatchAsync( () => animation( cell, TimeSpan.FromMilliseconds( animationLength ) ) );
            cellIndexes.RemoveAt( cellIndex );
        }
    }

    private int GetRandomAnimationLength( Random random )
        => random.Next( (int)minAnimationSpan.TotalMilliseconds, (int)maxAnimationSpan.TotalMilliseconds );

    private void BlackHoleAnimation( LabelOrNumberedGrid element, TimeSpan animationLength )
    {
        uint animationLengthInMilliseconds = (uint)animationLength.TotalMilliseconds;
        element.ScaleTo( 0, animationLengthInMilliseconds );
        double middleOfGameGridX = GameGrid.Width / 2;
        double middleOfGameGridY = GameGrid.Height / 2;
        TranslateCellToAbsolutePosition( element, middleOfGameGridX, middleOfGameGridY, animationLengthInMilliseconds );
    }

    private static void TranslateCellToAbsolutePosition( LabelOrNumberedGrid cell, double newX, double newY, uint animationLength )
    {
        double cellXOffset = cell.X + cell.Width / 2;
        double cellYOffset = cell.Y + cell.Height / 2;
        cell.TranslateTo( -cellXOffset + newX, -cellYOffset + newY, animationLength );
    }

    private void MoveIrrelevantButtonsAwayAfterVictory()
    {
        PauseBtn.TranslateTo( 0, -TopButtonsStack.Height );
        BottomButtonsStack.TranslateTo( 0, BottomButtonsStack.Height );
        NumberPad.TranslateTo( 0, -NumberPad.Height - NumberPad.X + Height );
    }

    private async Task ResetPageElementsPosition()
    {
        _ = Dispatcher.DispatchAsync( () => ReturnElementToOriginalPositionAndScale( PauseBtn, minAnimationSpan ) );
        _ = Dispatcher.DispatchAsync( () => ReturnElementToOriginalPositionAndScale( BottomButtonsStack, minAnimationSpan ) );
        _ = Dispatcher.DispatchAsync( () => ReturnElementToOriginalPositionAndScale( NumberPad, minAnimationSpan ) );
        _ = Dispatcher.DispatchAsync( () => ReturnElementToOriginalPositionAndScale( NewGameBtn, minAnimationSpan ) );
        _ = Dispatcher.DispatchAsync( () => ReturnElementToOriginalPositionAndScale( RestartBtn, minAnimationSpan ) );
        _ = Dispatcher.DispatchAsync( () => ReturnElementToOriginalPositionAndScale( TimerLbl, minAnimationSpan ) );
        await Task.Delay( minAnimationSpan );
    }

    private void MoveTimerToVictoryPosition()
    {
        TimerLbl.ScaleTo( 2 );
        TranslateTimerToAbsolutePosition( GameGrid.Width / 2, GameGrid.Height * 0.75 );
    }

    private void TranslateTimerToAbsolutePosition( double newX, double newY )
    {
        double timerXOffset = TimerLbl.X + PauseAndTimer.X + TimerLbl.Width / 2;
        double timerYOffset = TimerLbl.Y + PauseAndTimer.Y + TimerLbl.Height / 2;
        TimerLbl.TranslateTo( -timerXOffset + newX, -timerYOffset + newY );
    }

    private void MoveRelevantButtonsBelowTimerAfterVictory()
    {
        TranslateButtonToAbsolutePosition( NewGameBtn, GameGrid.Width / 2 - NewGameBtn.Width / 2, GameGrid.Height );
        TranslateButtonToAbsolutePosition( RestartBtn, GameGrid.Width / 2 + RestartBtn.Width / 2, GameGrid.Height );
    }

    private void TranslateButtonToAbsolutePosition( Button button, double newX, double newY )
    {
        double buttonXOffset = NewGameAndRestart.X + button.Width;
        double buttonYOffset = NewGameAndRestart.Y + button.Height;
        button.TranslateTo( -buttonXOffset + newX, -buttonYOffset + newY );
    }

    private async void GameVM_NewGameAfterFinishedOne()
    {
        await ResetPageElementsPosition();
        RunAnimationOnAllCellsAtRandomOrder( ReturnElementToOriginalPositionAndScale );
    }

    private static void ReturnElementToOriginalPositionAndScale( VisualElement element, TimeSpan animationLength )
    {
        element.ScaleTo( 1, (uint)animationLength.TotalMilliseconds );
        element.TranslateTo( 0, 0, (uint)animationLength.TotalMilliseconds );
    }

    private void InitializeNumberPad( int gridSize )
    {
        SetColumnAndRowNumberPadDefinitions( gridSize );

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

        PropertyChanged += GamePage_PropertyChanged;
        Appearing += GamePage_Appearing;
    }

    private void GamePage_Appearing( object? sender, EventArgs e ) => gameVM.OnPageAppearing();

    private async void GamePage_PropertyChanged( object? sender, PropertyChangedEventArgs e )
    {
        if ( e.PropertyName != nameof( Width ) ) return;

        await Dispatcher.DispatchAsync( () => { } ); // Wait for UI to be ready before measuring the width

        if ( !( Width > 0 ) ) return;

        SettingsFlyout.SetInitialFlyoutPosition();
        PropertyChanged -= GamePage_PropertyChanged;
    }

    private void SetColumnAndRowNumberPadDefinitions( int gridSize )
    {
        ColumnDefinitionCollection columns = new();
        RowDefinitionCollection rows = new();
        for ( int i = 0; i < gridSize; i++ ) {
            columns.Add( new ColumnDefinition( GridLength.Star ) );
        }
        rows.Add( new RowDefinition( GridLength.Star ) );

        NumberPad.ColumnDefinitions = columns;
        NumberPad.RowDefinitions = rows;
    }
}
