﻿using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class NumPadButton : ObservableObject
{
    public bool IsActive
    {
        get => _isActive;
        private set {
            bool oldValue = _isActive;
            _isActive = value;
            ColorAffectingPropertyChanged?.Invoke( oldValue, value );
        }
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        private set {
            bool oldValue = _isEnabled;
            _isEnabled = value;
            ColorAffectingPropertyChanged?.Invoke( oldValue, value );
        }
    }

    [ObservableProperty]
    private Color _backgroundColor;

    [ObservableProperty]
    private Color _textColor;

    [ObservableProperty]
    private int _remainingCount;

    private event Action<bool, bool>? ColorAffectingPropertyChanged;

    private bool _isActive;
    private bool _isEnabled;

    private readonly Color activeBackgroundColor;
    private readonly Color inactiveBackgroundColor;
    private readonly Color activeTextColor;
    private readonly Color inactiveTextColor;
    private readonly Color disabledActiveBackgroundColor;
    private readonly Color disabledInactiveBackgroundColor;
    private readonly Color disabledActiveTextColor;
    private readonly Color disabledInactiveTextColor;

    public NumPadButton( ISettingsService settings, IResourcesService resources )
    {
        Color primaryColor = settings.GetPrimaryColor();
        bool success = resources.TryGetColorByName( "SecondaryColor", out Color secondaryColor );
        success &= resources.TryGetColorByName( "DisabledPrimaryColor", out Color disabledPrimaryColor );
        success &= resources.TryGetColorByName( "DisabledSecondaryColor", out Color disabledSecondaryColor );
        if ( !success ) throw new ApplicationException( "Failed to obtain colors from resources" );

        activeBackgroundColor = secondaryColor;
        inactiveBackgroundColor = primaryColor;
        activeTextColor = primaryColor;
        inactiveTextColor = secondaryColor;
        disabledActiveBackgroundColor = disabledPrimaryColor;
        disabledInactiveBackgroundColor = disabledSecondaryColor;
        disabledActiveTextColor = disabledSecondaryColor;
        disabledInactiveTextColor = disabledPrimaryColor;

        ColorAffectingPropertyChanged += NumPadButton_ColorAffectingPropertyChanged;

        TextColor = inactiveTextColor;
        BackgroundColor = inactiveBackgroundColor;

        if ( _backgroundColor == null || _textColor == null )
            throw new NullReferenceException( "Color properties are not filled in constructor" );
    }

    private void NumPadButton_ColorAffectingPropertyChanged( bool oldValue, bool newValue )
    {
        if ( oldValue == newValue ) return;

        if ( IsActive ) {
            BackgroundColor = IsEnabled ? activeBackgroundColor : disabledActiveBackgroundColor;
            TextColor = IsEnabled ? activeTextColor : disabledActiveTextColor;
        } else {
            BackgroundColor = IsEnabled ? inactiveBackgroundColor : disabledInactiveBackgroundColor;
            TextColor = IsEnabled ? inactiveTextColor : disabledInactiveTextColor;
        }
    }

    public void SetActive() => IsActive = true;

    public void SetInactive() => IsActive = false;

    public void UpdateRemainingCount( int remainingCount )
    {
        RemainingCount = remainingCount;
        IsEnabled = remainingCount > 0;
    }
}
