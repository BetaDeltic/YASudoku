using YASudoku.Controls;
using YASudoku.ViewModels.GameViewModel;

namespace YASudoku.Views;

// Some animations here are split into Android specific code as a workaround for https://github.com/xamarin/Xamarin.Forms/issues/6760 issue

public partial class GamePage
{
    private readonly TimeSpan minAnimationSpan = TimeSpan.FromMilliseconds( 500 );
    private readonly TimeSpan maxAnimationSpan = TimeSpan.FromMilliseconds( 1000 );

    private const double gameOverButtonsXOffsetFromMiddle = 40;
    private const double gameOverButtonsYOffsetFromMiddle = 100;

    private void RunVictoryAnimation()
        => RunAnimationAndNotifyGameVM( RunVictoryAnimationAsync, AnimationTypes.Victory );
    private void RunStartingNewGameAnimation()
        => RunAnimationAndNotifyGameVM( RunStartingNewGameAnimationAsync, AnimationTypes.NewGame );
    private void RunAbortedGameAnimation()
        => RunAnimationAndNotifyGameVM( RunAbortGameAnimationAsync, AnimationTypes.AbortingGame );

    private async void RunAnimationAndNotifyGameVM( Func<Task> animation, AnimationTypes animationType )
    {
        gameVM.OnAnimationStarted();
        await animation();
        gameVM.OnAnimationEnded( animationType );
    }

    private async Task RunVictoryAnimationAsync()
    {
        await Task.WhenAll(
            RunAnimationOnAllCellsAtRandomOrderAsync( BlackHoleAnimationAsync ),
            MoveIrrelevantButtonsAwayAfterVictoryAsync()
        );
        await Task.WhenAll(
            MoveTimerToVictoryPositionAsync(),
            MoveRelevantButtonsBelowTimerAfterVictoryAsync()
        );
    }

    private async Task RunStartingNewGameAnimationAsync()
    {
        await Task.WhenAll(
            ResetPageElementsPositionAsync(),
            RunAnimationOnAllCellsAtRandomOrderAsync( ReturnElementToOriginalPositionAndScaleAsync )
        );
    }

    private async Task RunAbortGameAnimationAsync()
        => await RunAnimationOnAllCellsAtRandomOrderAsync( BlackHoleAnimationAsync );

    private static async Task ReturnElementToOriginalPositionAndScaleAsync( VisualElement element, TimeSpan animationLength )
    {
        await Task.WhenAll(
            element.ScaleTo( 1, (uint)animationLength.TotalMilliseconds ),
            element.TranslateTo( 0, 0, (uint)animationLength.TotalMilliseconds ),
            element.FadeTo( 1, (uint)animationLength.TotalMilliseconds )
        );
    }

    private async Task RunAnimationOnAllCellsAtRandomOrderAsync( Func<LabelOrNumberedGrid, TimeSpan, Task> animation )
    {
        List<int> cellIndexes = new( Enumerable.Range( 0, GameGrid.Children.Count ) );
        List<Task> animationTasks = new();

        Random random = new();
        while ( cellIndexes.Count > 0 ) {
            int cellIndex = random.Next( cellIndexes.Count );
            IView child = GameGrid.Children[ cellIndexes[ cellIndex ] ];
            LabelOrNumberedGrid cell = (LabelOrNumberedGrid)child;
            int animationLength = GetRandomAnimationLength( random );
            animationTasks.Add( Task.Run( () => animation( cell, TimeSpan.FromMilliseconds( animationLength ) ) ) );
            cellIndexes.RemoveAt( cellIndex );
        }

        await Task.WhenAll( animationTasks );
    }

    private async Task BlackHoleAnimationAsync( LabelOrNumberedGrid element, TimeSpan animationLength )
    {
        uint animationLengthInMilliseconds = (uint)animationLength.TotalMilliseconds;
        double middleOfGameGridX = GameGrid.Width / 2;
        double middleOfGameGridY = GameGrid.Height / 2;
        await Task.WhenAll(
            element.ScaleTo( 0, animationLengthInMilliseconds, Easing.SpringIn ),
            TranslateCellToAbsolutePositionAsync( element, middleOfGameGridX, middleOfGameGridY, animationLengthInMilliseconds )
        );
    }

    private static async Task TranslateCellToAbsolutePositionAsync( LabelOrNumberedGrid cell, double newX, double newY, uint animationLength )
    {
        double cellXOffset = cell.X + cell.Width / 2;
        double cellYOffset = cell.Y + cell.Height / 2;
        await cell.TranslateTo( -cellXOffset + newX, -cellYOffset + newY, animationLength, Easing.SpringIn );
    }

    private async Task MoveIrrelevantButtonsAwayAfterVictoryAsync()
    {
        await Task.WhenAll(
            PauseBtn.TranslateTo( 0, -TopButtonsStack.Height ),
            PauseBtn.FadeTo( 0 ),
            BottomButtonsStack.TranslateTo( 0, BottomButtonsStack.Height ),
            BottomButtonsStack.FadeTo( 0 ),
            NumberPad.TranslateTo( 0, -NumberPad.Height - NumberPad.X + Height ),
            NumberPad.FadeTo( 0 )
        );
    }

    private async Task ResetPageElementsPositionAsync()
    {
#if ANDROID
        await HideTemporaryButtonsShowRegularOnesAsync();
#endif

        await Task.WhenAll(
            ReturnElementToOriginalPositionAndScaleAsync( PauseBtn, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( BottomButtonsStack, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( NumberPad, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( NewGameBtn, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( RestartBtn, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( TimerLbl, minAnimationSpan )
        );
    }

#if ANDROID
    private async Task HideTemporaryButtonsShowRegularOnesAsync()
    {
        ChangeTemporaryButtonsVisibility( makeVisible: false );
        await ChangeRegularButtonsVisibilityAsync( makeVisible: true );
    }

    private async Task ChangeRegularButtonsVisibilityAsync( bool makeVisible )
    {
        double opacity = makeVisible ? 1 : 0;
        await Task.WhenAll(
            NewGameBtn.FadeTo( opacity, 0 ),
            RestartBtn.FadeTo( opacity, 0 )
        );
    }

    private void ChangeTemporaryButtonsVisibility( bool makeVisible )
    {
        NewGameAndroidTempBtn.IsVisible = makeVisible;
        RestartAndroidTempBtn.IsVisible = makeVisible;
    }
#endif

    private async Task MoveTimerToVictoryPositionAsync()
    {
        await Task.WhenAll(
            TimerLbl.ScaleTo( 2 ),
            TranslateTimerToAbsolutePositionAsync( GameGrid.Width / 2, GameGrid.Height * 0.75 )
        );
    }

    private async Task TranslateTimerToAbsolutePositionAsync( double newX, double newY )
    {
        double timerXOffset = TimerLbl.X + PauseAndTimer.X + TimerLbl.Width / 2;
        double timerYOffset = TimerLbl.Y + PauseAndTimer.Y + TimerLbl.Height / 2;
        await TimerLbl.TranslateTo( -timerXOffset + newX, -timerYOffset + newY );
    }

    private async Task MoveRelevantButtonsBelowTimerAfterVictoryAsync()
    {
        double buttonsYPosition = GetGameOverButtonsYPosition();
        await Task.WhenAll(
            TranslateButtonToAbsolutePositionAsync( NewGameBtn, GetGameOverNewGameButtonXPosition(), buttonsYPosition ),
            TranslateButtonToAbsolutePositionAsync( RestartBtn, GetGameOverRestartButtonXPosition(), buttonsYPosition )
        );
#if ANDROID
        await MoveTemporaryButtonsInCorrectPositionAsync();
        await ShowTemporaryButtonsHideRegularOnesAsync();
#endif
    }
#if ANDROID
    private async Task ShowTemporaryButtonsHideRegularOnesAsync()
    {
        await ChangeRegularButtonsVisibilityAsync( makeVisible: false );
        ChangeTemporaryButtonsVisibility( makeVisible: true );
    }

    private async Task MoveTemporaryButtonsInCorrectPositionAsync()
    {
        double buttonsYPosition = GetGameOverButtonsYPosition();
        await Task.WhenAll(
            TranslateTempButtonToAbsolutePositionAsync( NewGameAndroidTempBtn, GetGameOverNewGameButtonXPosition(), buttonsYPosition ),
            TranslateTempButtonToAbsolutePositionAsync( RestartAndroidTempBtn, GetGameOverRestartButtonXPosition(), buttonsYPosition )
        );
    }

    private static async Task TranslateTempButtonToAbsolutePositionAsync( Button button, double newX, double newY )
    {
        double buttonXOffset = button.X;
        double buttonYOffset = button.Y;
        await button.TranslateTo( -buttonXOffset + newX, -buttonYOffset + newY, 0 );
    }
#endif
    private async Task TranslateButtonToAbsolutePositionAsync( Button button, double newX, double newY )
    {
        double buttonXOffset = TopButtonsStack.Padding.Left + NewGameAndRestart.X + button.X;
        double buttonYOffset = TopButtonsStack.Padding.Top + NewGameAndRestart.Y + button.Y;
        await button.TranslateTo( -buttonXOffset + newX, -buttonYOffset + newY );
    }

    private int GetRandomAnimationLength( Random random )
        => random.Next( (int)minAnimationSpan.TotalMilliseconds, (int)maxAnimationSpan.TotalMilliseconds );

    private double GetGameOverNewGameButtonXPosition()
    {
        double gridXMiddle = MainGrid.Width / 2;
        double newGameButtonXOffset = NewGameBtn.Width + gameOverButtonsXOffsetFromMiddle;
        return gridXMiddle - newGameButtonXOffset;
    }

    private double GetGameOverRestartButtonXPosition()
    {
        double gridXMiddle = MainGrid.Width / 2;
        return gridXMiddle + gameOverButtonsXOffsetFromMiddle;
    }

    private double GetGameOverButtonsYPosition()
    {
        double gridYMiddle = MainGrid.Height / 2;
        return gridYMiddle + gameOverButtonsYOffsetFromMiddle;
    }
}
