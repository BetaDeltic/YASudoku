using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace YASudoku.Controls;

public partial class SwitchWithLabel : ContentView
{
    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create( nameof( LabelText ), typeof( string ), typeof( SwitchWithLabel ), string.Empty );
    public static readonly BindableProperty SwitchOnColorProperty = BindableProperty.Create( nameof( SwitchOnColor ), typeof( Color ), typeof( SwitchWithLabel ) );
    public static readonly BindableProperty CommandProperty = BindableProperty.Create( nameof( Command ), typeof( ICommand ), typeof( SwitchWithLabel ) );
    public static readonly BindableProperty IsSwitchToggledProperty = BindableProperty.Create( nameof( IsSwitchToggled ), typeof( bool ), typeof( SwitchWithLabel ), false );

    public string LabelText
    {
        get => (string)GetValue( LabelTextProperty );
        set => SetValue( LabelTextProperty, value );
    }

    public Color SwitchOnColor
    {
        get => (Color)GetValue( SwitchOnColorProperty );
        set => SetValue( SwitchOnColorProperty, value );
    }

    public ICommand Command
    {
        get => (ICommand)GetValue( CommandProperty );
        set => SetValue( CommandProperty, value );
    }

    public bool IsSwitchToggled
    {
        get => (bool)GetValue( IsSwitchToggledProperty );
        set => SetValue( IsSwitchToggledProperty, value );
    }

    public SwitchWithLabel()
    {
        InitializeComponent();
        BindingContext = this;
        TheLabel.BindingContext = this;
        TheLabel.SetBinding( Label.TextProperty, nameof( LabelText ) );
        TheSwitch.BindingContext = this;
        TheSwitch.SetBinding( Switch.OnColorProperty, nameof( SwitchOnColor ) );
        TheSwitch.SetBinding( Switch.IsToggledProperty, nameof( IsSwitchToggled ) );
        TheSwitch.Toggled += TheSwitch_Toggled;
        GridWrapper.GestureRecognizers.Add( new TapGestureRecognizer() { Command = SwitchToggledCommand } );
    }

    ~SwitchWithLabel()
    {
        TheSwitch.Toggled -= TheSwitch_Toggled;
    }

    private void TheSwitch_Toggled( object? sender, ToggledEventArgs e ) => Command?.Execute( e.Value );

    [RelayCommand]
    private void SwitchToggled() => IsSwitchToggled = !IsSwitchToggled;
}
