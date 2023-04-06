using System.Windows.Input;

namespace YASudoku.Controls;

public partial class ButtonWithSubText : ContentView
{
    public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create( nameof( BackgroundColor ), typeof( Color ), typeof( ButtonWithSubText ) );

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create( nameof( TextColor ), typeof( Color ), typeof( ButtonWithSubText ) );
    public static readonly BindableProperty CommandProperty = BindableProperty.Create( nameof( Command ), typeof( ICommand ), typeof( ButtonWithSubText ) );
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create( nameof( CommandParameter ), typeof( object ), typeof( ButtonWithSubText ) );
    public static readonly BindableProperty TextProperty = BindableProperty.Create( nameof( Text ), typeof( string ), typeof( ButtonWithSubText ), string.Empty );
    public static readonly BindableProperty SubTextProperty = BindableProperty.Create( nameof( SubText ), typeof( string ), typeof( ButtonWithSubText ), string.Empty );

    public new Color BackgroundColor
    {
        get => (Color)GetValue( BackgroundColorProperty );
        set => SetValue( BackgroundColorProperty, value );
    }

    public Color TextColor
    {
        get => (Color)GetValue( TextColorProperty );
        set => SetValue( TextColorProperty, value );
    }

    public ICommand Command
    {
        get => (ICommand)GetValue( CommandProperty );
        set => SetValue( CommandProperty, value );
    }

    public object CommandParameter
    {
        get => GetValue( CommandParameterProperty );
        set => SetValue( CommandParameterProperty, value );
    }

    public string Text
    {
        get => (string)GetValue( TextProperty );
        set => SetValue( TextProperty, value );
    }

    public string SubText
    {
        get => (string)GetValue( SubTextProperty );
        set => SetValue( SubTextProperty, value );
    }

    private VisualElement? lastElement;

    public ButtonWithSubText()
    {
        InitializeComponent();

        PanGestureRecognizer panGestureRecognizer = new();
        panGestureRecognizer.PanUpdated += PanGestureRecognizer_PanUpdated;
        GestureRecognizers.Add( panGestureRecognizer );

        TapGestureRecognizer tapGestureRecognizer = new();
        tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
        GestureRecognizers.Add( tapGestureRecognizer );
    }

    private void TapGestureRecognizer_Tapped( object? sender, TappedEventArgs e )
        => ExecuteAllowedCommand();

    private void PanGestureRecognizer_PanUpdated( object? sender, PanUpdatedEventArgs e )
    {
        if ( sender is null or not VisualElement ) {
            return;
        }

        VisualElement callingObject = ( sender as VisualElement )!;
        if ( e.StatusType == GestureStatus.Started ) {
            lastElement = callingObject;
        } else if ( e.StatusType == GestureStatus.Completed ) {
            if ( lastElement == callingObject ) {
                ExecuteAllowedCommand();
            }
        }
    }

    private void ExecuteAllowedCommand()
    {
        if ( Command.CanExecute( CommandParameter ) ) {
            Command.Execute( CommandParameter );
        }
    }
}
