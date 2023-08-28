using System.Windows.Input;
using YASudoku.Controls.ControlBindings;

namespace YASudoku.Controls;

public partial class LabelOrNumberedGrid : ContentView
{
    public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create( nameof( BackgroundColor ), typeof( Color ), typeof( LabelOrNumberedGrid ) );

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create( nameof( TextColor ), typeof( Color ), typeof( LabelOrNumberedGrid ) );
    public static readonly BindableProperty TextProperty = BindableProperty.Create( nameof( Text ), typeof( string ), typeof( LabelOrNumberedGrid ), string.Empty );
    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create( nameof( FontAttributes ), typeof( FontAttributes ), typeof( LabelOrNumberedGrid ), FontAttributes.None );
    public static readonly BindableProperty CommandProperty = BindableProperty.Create( nameof( Command ), typeof( ICommand ), typeof( LabelOrNumberedGrid ) );
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create( nameof( CommandParameter ), typeof( object ), typeof( LabelOrNumberedGrid ) );
    public static readonly BindableProperty IsLabelVisibleProperty = BindableProperty.Create( nameof( IsLabelVisible ), typeof( bool ), typeof( LabelOrNumberedGrid ), true );
    public static readonly BindableProperty IsGridVisibleProperty = BindableProperty.Create( nameof( IsGridVisible ), typeof( bool ), typeof( LabelOrNumberedGrid ), false );
    public static readonly BindableProperty GridBindingListProperty = BindableProperty.Create( nameof( GridBindingList ), typeof( List<LabelOrNumberedGridBinding> ), typeof( LabelOrNumberedGrid ) );

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

    public string Text
    {
        get => (string)GetValue( TextProperty );
        set => SetValue( TextProperty, value );
    }

    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue( FontAttributesProperty );
        set => SetValue( FontAttributesProperty, value );
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

    public bool IsLabelVisible
    {
        get => (bool)GetValue( IsLabelVisibleProperty );
        set {
            SetValue( IsLabelVisibleProperty, value );
            SetValue( IsGridVisibleProperty, !value );
        }
    }

    public bool IsGridVisible
    {
        get => (bool)GetValue( IsGridVisibleProperty );
        set {
            SetValue( IsGridVisibleProperty, value );
            SetValue( IsLabelVisibleProperty, !value );
        }
    }

    public List<LabelOrNumberedGridBinding> GridBindingList
    {
        get => (List<LabelOrNumberedGridBinding>)GetValue( GridBindingListProperty );
        set => SetValue( GridBindingListProperty, value );
    }

    private VisualElement? lastElement;

    public LabelOrNumberedGrid( int lineLength, List<LabelOrNumberedGridBinding> gridElementProperties )
    {
        InitializeComponent();

        GridBindingList = gridElementProperties;

        InitializeGrid( lineLength );

        PanGestureRecognizer panGestureRecognizer = new();
        panGestureRecognizer.PanUpdated += PanGestureRecognizer_PanUpdated;
        GestureRecognizers.Add( panGestureRecognizer );

        TapGestureRecognizer tapGestureRecognizer = new();
        tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
        GestureRecognizers.Add( tapGestureRecognizer );
    }

    private void InitializeGrid( int lineLength )
    {
        SetColumnAndRowGridDefinitions( lineLength );

        for ( int column = 0; column < lineLength; column++ ) {
            for ( int row = 0; row < lineLength; row++ ) {
                int cellNumber = row * lineLength + column;
                Label label = new() {
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    BindingContext = GridBindingList[ cellNumber ]
                };

                label.SetBinding( Label.TextProperty, nameof( LabelOrNumberedGridBinding.Text ) );
                label.SetBinding( IsVisibleProperty, nameof( LabelOrNumberedGridBinding.IsVisible ) );
                label.SetBinding( VisualElement.BackgroundColorProperty, nameof( LabelOrNumberedGridBinding.BackgroundColor ) );
                label.SetBinding( Label.TextColorProperty, nameof( LabelOrNumberedGridBinding.TextColor ) );

                TheGrid.SetColumn( label, column );
                TheGrid.SetRow( label, row );
                TheGrid.Children.Add( label );
            }
        }
    }

    private void SetColumnAndRowGridDefinitions( int lineLength )
    {
        ColumnDefinitionCollection columns = new();
        RowDefinitionCollection rows = new();
        for ( int i = 0; i < lineLength; i++ ) {
            columns.Add( new ColumnDefinition() { Width = GridLength.Star } );
            rows.Add( new RowDefinition() { Height = GridLength.Star } );
        }

        TheGrid.ColumnDefinitions = columns;
        TheGrid.RowDefinitions = rows;
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
