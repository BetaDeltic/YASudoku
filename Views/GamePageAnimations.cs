using YASudoku.Controls;
using YASudoku.ViewModels.GameViewModel;

namespace YASudoku.Views;

public partial class GamePage
{
    private readonly TimeSpan minAnimationSpan = TimeSpan.FromMilliseconds( 500 );
    private readonly TimeSpan maxAnimationSpan = TimeSpan.FromMilliseconds( 1000 );

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
        await Task.WhenAll(
            ReturnElementToOriginalPositionAndScaleAsync( PauseBtn, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( BottomButtonsStack, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( NumberPad, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( NewGameBtn, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( RestartBtn, minAnimationSpan ),
            ReturnElementToOriginalPositionAndScaleAsync( TimerLbl, minAnimationSpan )
        );
    }

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
        await Task.WhenAll(
            TranslateButtonToAbsolutePositionAsync( NewGameBtn, GameGrid.Width / 2 - NewGameBtn.Width / 2, GameGrid.Height ),
            TranslateButtonToAbsolutePositionAsync( RestartBtn, GameGrid.Width / 2 + RestartBtn.Width / 2, GameGrid.Height )
        );
    }

    private async Task TranslateButtonToAbsolutePositionAsync( Button button, double newX, double newY )
    {
        double buttonXOffset = NewGameAndRestart.X + button.Width;
        double buttonYOffset = NewGameAndRestart.Y + button.Height;
        await button.TranslateTo( -buttonXOffset + newX, -buttonYOffset + newY );
    }

    private int GetRandomAnimationLength( Random random )
        => random.Next( (int)minAnimationSpan.TotalMilliseconds, (int)maxAnimationSpan.TotalMilliseconds );
}
