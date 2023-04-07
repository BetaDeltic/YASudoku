using YASudoku.Controls;

namespace YASudoku.Views;

public partial class GamePage
{
    private readonly TimeSpan minAnimationSpan = TimeSpan.FromMilliseconds( 500 );
    private readonly TimeSpan maxAnimationSpan = TimeSpan.FromMilliseconds( 1000 );

    private static void ReturnElementToOriginalPositionAndScale( VisualElement element, TimeSpan animationLength )
    {
        element.ScaleTo( 1, (uint)animationLength.TotalMilliseconds );
        element.TranslateTo( 0, 0, (uint)animationLength.TotalMilliseconds );
    }

    private async void RunVictoryAnimation()
    {
        _ = Dispatcher.DispatchAsync( () => RunAnimationOnAllCellsAtRandomOrder( BlackHoleAnimation ) );
        _ = Dispatcher.DispatchAsync( MoveIrrelevantButtonsAwayAfterVictory );
        await Task.Delay( maxAnimationSpan );
        _ = Dispatcher.DispatchAsync( MoveTimerToVictoryPosition );
        _ = Dispatcher.DispatchAsync( MoveRelevantButtonsBelowTimerAfterVictory );
    }

    private async void RunNewGameAnimation()
    {
        await ResetPageElementsPosition();
        RunAnimationOnAllCellsAtRandomOrder( ReturnElementToOriginalPositionAndScale );
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

    private int GetRandomAnimationLength( Random random )
        => random.Next( (int)minAnimationSpan.TotalMilliseconds, (int)maxAnimationSpan.TotalMilliseconds );
}
