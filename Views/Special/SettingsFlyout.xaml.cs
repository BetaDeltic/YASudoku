using YASudoku.Controls;
using YASudoku.ViewModels;

namespace YASudoku.Views.Special;

public partial class SettingsFlyout : ContentView
{
    public static readonly BindableProperty IsHighlightingRelatedEnabledProperty = BindableProperty.Create( nameof( IsHighlightingRelatedEnabled ), typeof( bool ), typeof( SettingsFlyout ), false );

    public static readonly BindableProperty IsFlyoutVisibleProperty = BindableProperty.Create( nameof( IsFlyoutVisible ), typeof( bool ), typeof( SettingsFlyout ), false, BindingMode.TwoWay,
        propertyChanging: ( bindable, oldValue, newValue ) => ( (SettingsFlyout)bindable ).FlyoutVisibilityChanging( (bool)oldValue, (bool)newValue ),
        propertyChanged: ( bindable, oldValue, newValue ) => ( (SettingsFlyout)bindable ).FlyoutVisibilityChanged( (bool)oldValue, (bool)newValue )
    );

    public static readonly BindableProperty IsFlyoutVisibleInternalProperty = BindableProperty.Create( nameof( IsFlyoutInternalVisible ), typeof( bool ), typeof( SettingsFlyout ), false, BindingMode.TwoWay,
        propertyChanging: ( bindable, oldValue, newValue ) => ( (SettingsFlyout)bindable ).FlyoutVisibilityChanging( (bool)oldValue, (bool)newValue ),
        propertyChanged: ( bindable, oldValue, newValue ) => ( (SettingsFlyout)bindable ).FlyoutVisibilityChanged( (bool)oldValue, (bool)newValue )
    );

    // Workaround to be able to have dependencies like VM until injectable custom controls are supported
    public static readonly BindableProperty ServiceProviderProperty = BindableProperty.Create( nameof( ServiceProvider ), typeof( IServiceProvider ), typeof( SettingsFlyout ),
        propertyChanged: ( bindable, oldValue, newValue ) => ( (SettingsFlyout)bindable ).Init( (IServiceProvider)newValue )
    );

    // Actually used visibility property in the XAML, reacts to the changes to IsFlyoutVisible,
    // this is necessary to keep flyout from disappearing immediately upon setting IsFlyoutVisible to false from the outside
    private new static readonly BindableProperty IsVisibleProperty = BindableProperty.Create( nameof( IsVisible ), typeof( bool ), typeof( SettingsFlyout ), false );

    public IServiceProvider ServiceProvider
    {
        get => (IServiceProvider)GetValue( ServiceProviderProperty );
        set => SetValue( ServiceProviderProperty, value );
    }

    public bool IsFlyoutVisible
    {
        get => (bool)GetValue( IsFlyoutVisibleProperty );
        set => SetValue( IsFlyoutVisibleProperty, value );
    }

    public bool IsFlyoutInternalVisible
    {
        get => (bool)GetValue( IsFlyoutVisibleInternalProperty );
        set => SetValue( IsFlyoutVisibleInternalProperty, value );
    }

    public bool IsHighlightingRelatedEnabled
    {
        get => (bool)GetValue( IsHighlightingRelatedEnabledProperty );
        set => SetValue( IsHighlightingRelatedEnabledProperty, value );
    }

    private new bool IsVisible
    {
        get => (bool)GetValue( IsVisibleProperty );
        set => SetValue( IsVisibleProperty, value );
    }

    private const uint AnimationLengthInMilliseconds = 250;

    private SettingsFlyoutVM? settingsFlyoutVM;

    public SettingsFlyout()
    {
        InitializeComponent();
    }

    public void Init( IServiceProvider serviceProvider )
    {
        settingsFlyoutVM = serviceProvider.GetService<SettingsFlyoutVM>()!;

        BindingContext = settingsFlyoutVM;

        this.SetBinding( IsFlyoutVisibleInternalProperty, nameof( SettingsFlyoutVM.IsFlyoutVisibleInternal ) );

        RelatedCellsSwitch.BindingContext = settingsFlyoutVM;

        RelatedCellsSwitch.SetBinding( SwitchWithLabel.CommandProperty, nameof( SettingsFlyoutVM.ToggleRelatedCellsSwitchCommand ) );
        RelatedCellsSwitch.SetBinding( SwitchWithLabel.IsSwitchToggledProperty, nameof( SettingsFlyoutVM.IsHighlightRelatedEnabled ) );
        RelatedCellsSwitch.SetBinding( SwitchWithLabel.SwitchOnColorProperty, nameof( SettingsFlyoutVM.AccentColor ) );

        MistakesSwitch.BindingContext = settingsFlyoutVM;

        MistakesSwitch.SetBinding( SwitchWithLabel.CommandProperty, nameof( SettingsFlyoutVM.ToggleHighlightMistakesSwitchCommand ) );
        MistakesSwitch.SetBinding( SwitchWithLabel.IsSwitchToggledProperty, nameof( SettingsFlyoutVM.IsHighlightMistakesEnabled ) );
        MistakesSwitch.SetBinding( SwitchWithLabel.SwitchOnColorProperty, nameof( SettingsFlyoutVM.AccentColor ) );

        HideBtn.SetBinding( Button.CommandProperty, nameof( SettingsFlyoutVM.HideSettingsCommand ) );
        HideBtn.SetBinding( BackgroundColorProperty, nameof( SettingsFlyoutVM.AccentColor ) );
        HideBtn.SetBinding( Button.TextColorProperty, nameof( SettingsFlyoutVM.ForegroundColor ) );
    }

    public async void SetInitialFlyoutPosition()
    {
        await RollOutAsync( 0 ); // Immediately roll out of the screen
        IsVisible = true; // Enable visibility outside of screen
        await Dispatcher.DispatchAsync( () => { } ); // Wait for UI to be able to measure the flyout width
        await RollOutAsync( 0 ); // Move the X position to just outside the screen
        IsVisible = false; // Disable visibility again
    }

    private async void FlyoutVisibilityChanging( bool oldVisibility, bool newVisibility )
    {
        if ( settingsFlyoutVM == null || oldVisibility == newVisibility || oldVisibility == false ) return;

        settingsFlyoutVM.IsFlyoutVisibleInternal = newVisibility;
        IsFlyoutVisible = newVisibility;
        await RollOutAsync( AnimationLengthInMilliseconds );
        IsVisible = newVisibility;
    }

    private void FlyoutVisibilityChanged( bool oldVisibility, bool newVisibility )
    {
        if ( settingsFlyoutVM == null || oldVisibility == newVisibility || oldVisibility == true ) return;

        IsVisible = newVisibility;
        settingsFlyoutVM.IsFlyoutVisibleInternal = newVisibility;
        IsFlyoutVisible = newVisibility;
        RollIn();
    }

    private void RollIn()
    {
        if ( settingsFlyoutVM == null ) return;

        this.TranslateTo( 0, 0, AnimationLengthInMilliseconds );
        this.FadeTo( 1 );
    }

    private async Task RollOutAsync( uint lengthInMilliseconds )
    {
        if ( settingsFlyoutVM == null ) return;

        double hidingDistance = GetSufficientDistanceToHideTheFlyout();
        await this.TranslateTo( -hidingDistance, 0, lengthInMilliseconds );
        await this.FadeTo( 0, lengthInMilliseconds );
    }

    private double GetSufficientDistanceToHideTheFlyout()
    {
        double fallback = DeviceDisplay.Current.MainDisplayInfo.Width;
        double hidingDistance = SettingsPane.Width > 0 ? SettingsPane.Width : fallback;

        return hidingDistance;
    }
}
