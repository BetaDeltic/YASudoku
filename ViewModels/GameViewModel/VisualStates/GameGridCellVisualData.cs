using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Common;
using YASudoku.Controls.ControlBindings;
using YASudoku.Services.JournalingServices;
using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class GameGridCellVisualData : ObservableObject
{
    public event Action<bool>? HighlightChanged;
    public event Action<int>? ValueFilled;

    [ObservableProperty]
    public partial Color BackgroundColor { get; set; }

    [ObservableProperty]
    public partial FontAttributes FontAttribute { get; set; }

    [ObservableProperty]
    public partial bool IsLockedForChanges { get; set; }

    [ObservableProperty]
    public partial Color TextColor { get; set; }

    [ObservableProperty]
    public partial string UserFacingText { get; set; }

    [ObservableProperty]
    public partial int UserFacingValue { get; set; }

    [ObservableProperty]
    public partial bool IsShowingValue { get; set; } = true;

    [ObservableProperty]
    public partial bool IsShowingCandidates { get; set; }

    [ObservableProperty]
    public partial bool IsHighlightedAsSelected { get; set; }

    [ObservableProperty]
    public partial bool IsHighlightedAsRelated { get; set; }

    [ObservableProperty]
    public partial bool IsHidingAllValues { get; set; }

    public readonly int CellID;
    public int CorrectValue { get; private set; }

    public bool HasCorrectValue => UserFacingValue == CorrectValue;
    public bool HasUserFacingValue => UserFacingValue != 0;

    public List<LabelOrNumberedGridBinding> CandidatesGridProperties { get; private set; }

    internal readonly List<GameGridCellVisualData> relatedCells = new();

    private bool isMistakesHighlightingEnabled;
    private int highlightedCandidate;

    private readonly int initCandidatesCount;

    private readonly Color lockedColor;
    private readonly Color correctColor;
    private readonly Color incorrectColor;
    private readonly Color selectedCellBackgroundColor;
    private readonly Color highlightedAsRelatedBackgroundColor;
    private readonly Color regularCellBackgroundColor;

    private readonly ISettingsService settings;
    private readonly IPlayerJournalingService journal;
    private readonly IResourcesService resources;

    public GameGridCellVisualData( ISettingsService settingsService, IPlayerJournalingService journalingService,
        IResourcesService resourcesService, int candidatesCount, int id, bool isLocked, int value, int correctValue )
    {
        journal = journalingService;
        settings = settingsService;
        resources = resourcesService;
        settings.MistakesHighlightingChanged += Settings_MistakesHighlightingChanged;

        initCandidatesCount = candidatesCount;
        CellID = id;
        IsLockedForChanges = isLocked;
        UserFacingValue = value;
        CorrectValue = correctValue;

        InitializeCandidatesProperties( candidatesCount );

        highlightedAsRelatedBackgroundColor = settings.GetPrimaryColor();
        resources.TryGetColorByName( "SecondaryColor", out lockedColor );
        resources.TryGetColorByName( "CorrectCellValue", out correctColor );
        resources.TryGetColorByName( "IncorrectCellValue", out incorrectColor );
        resources.TryGetColorByName( "SelectedCellBackgroundColor", out selectedCellBackgroundColor );
        resources.TryGetColorByName( "RegularCellBackgroundColor", out regularCellBackgroundColor );

        UpdateFontAttribute();
        UpdateTextColor();
        UpdateUserFacingText();
        UpdateBackgroundColor();

        PropertyChanged += This_PropertyChanged;

        if ( TextColor == null ) throw new NullReferenceException( nameof( TextColor ) );
        if ( UserFacingText == null ) throw new NullReferenceException( nameof( UserFacingText ) );
        if ( BackgroundColor == null ) throw new NullReferenceException( nameof( BackgroundColor ) );
        if ( CandidatesGridProperties == null ) throw new NullReferenceException( nameof( relatedCells ) );
    }

    private void InitializeCandidatesProperties( int candidatesCount )
    {
        CandidatesGridProperties = new( candidatesCount );
        for ( int i = 0; i < candidatesCount; i++ ) {
            CandidatesGridProperties.Add( new LabelOrNumberedGridBinding() {
                IsVisible = true,
                BackgroundColor = regularCellBackgroundColor,
                TextColor = Colors.White,
            } );
        }
    }

    private void This_PropertyChanged( object? sender, System.ComponentModel.PropertyChangedEventArgs e )
    {
        if ( e.PropertyName == nameof( IsLockedForChanges ) ) {
            UpdateFontAttribute();
            UpdateTextColor();
        } else if ( e.PropertyName == nameof( UserFacingValue ) ) {
            UpdateUserFacingText();
            UpdateTextColor();
        } else if ( e.PropertyName == nameof( IsHidingAllValues ) ) {
            UpdateTextColor();
        } else if ( e.PropertyName == nameof( IsHighlightedAsSelected ) ) {
            UpdateBackgroundColor();
        } else if ( e.PropertyName == nameof( IsHighlightedAsRelated ) ) {
            UpdateBackgroundColor();
        }
    }

    private void UpdateFontAttribute()
        => FontAttribute = IsLockedForChanges ? FontAttributes.Bold : FontAttributes.None;

    private void UpdateTextColor()
    {
        TextColor = IsHidingAllValues ? Colors.Transparent
            : IsLockedForChanges ? lockedColor
            : HasCorrectValue || !isMistakesHighlightingEnabled ? correctColor
            : incorrectColor;
    }

    private void UpdateUserFacingText()
        => UserFacingText = !HasUserFacingValue ? string.Empty : UserFacingValue.ToString();

    private void UpdateBackgroundColor()
    {
        BackgroundColor =
        IsHighlightedAsSelected ? selectedCellBackgroundColor :
        IsHighlightedAsRelated ? highlightedAsRelatedBackgroundColor : regularCellBackgroundColor;
    }

    private void Settings_MistakesHighlightingChanged( bool canHighlightMistakes )
    {
        isMistakesHighlightingEnabled = canHighlightMistakes;
        UpdateTextColor();
    }

    public void AddCandidate( int candidateNumber, bool addToJournal = true )
        => ChangeCandidateVisibility( candidateNumber, setAsVisible: true, addToJournal );

    private void ChangeCandidateVisibility( int candidateNumber, bool setAsVisible, bool addToJournal = true )
    {
        if ( HasUserFacingValue ||
            ( HasNumberAsCandidate( candidateNumber ) && setAsVisible ) ||
            ( !HasNumberAsCandidate( candidateNumber ) && !setAsVisible ) ) {
            return;
        }

        GetCandidatePropertiesByNumber( candidateNumber ).Text =
            setAsVisible ? candidateNumber.ToString() : string.Empty;

        if ( !addToJournal ) return;
        PlayerTransactionTypes transactionType =
            setAsVisible ? PlayerTransactionTypes.CandidateAdded : PlayerTransactionTypes.CandidateRemoved;
        journal.AddTransaction( transactionType, this, candidateNumber );
    }

    public void AddRelatedCell( GameGridCellVisualData relatedCell )
    {
        relatedCells.Add( relatedCell );
        relatedCell.HighlightChanged += RelatedCell_HighlightChanged;
        relatedCell.ValueFilled += RelatedCell_ValueFilled;
    }

    private void RelatedCell_HighlightChanged( bool isHighlighted )
    {
        if ( isHighlighted )
            HighlightCellAsRelated();
        else
            UnhighlightCell();
    }

    private void RelatedCell_ValueFilled( int value )
    {
        if ( !HasNumberAsCandidate( value ) ) return;

        journal.AddTransaction( PlayerTransactionTypes.RelatedCandidateRemoved, this, value );

        RemoveCandidate( value, addToJournal: false );
    }

    public void DisplayValue()
    {
        if ( IsShowingValue ) return;

        IsShowingValue = true;
        IsShowingCandidates = false;

        CandidatesGridProperties.ForEach( x => x.IsVisible = false );
    }

    public void DisplayCandidates()
    {
        if ( HasUserFacingValue || IsShowingCandidates ) return;

        IsShowingValue = false;
        IsShowingCandidates = true;

        CandidatesGridProperties.ForEach( x => x.IsVisible = true );
    }

    private LabelOrNumberedGridBinding GetCandidatePropertiesByNumber( int candidateNumber )
    {
        return candidateNumber == 0 || candidateNumber > initCandidatesCount
            ? throw new ArgumentOutOfRangeException( nameof( candidateNumber ) )
            : CandidatesGridProperties[ candidateNumber - 1 ];
    }

    public IEnumerable<int> GetAllCandidateValues()
        => CandidatesGridProperties
        .Where( candidate => candidate.Text != string.Empty )
        .Select( candidate => int.Parse( candidate.Text ) );

    public bool HasNumberAsCandidate( int candidateNumber )
        => GetCandidatePropertiesByNumber( candidateNumber ).Text != string.Empty;

    public void HideAllValues()
    {
        CandidatesGridProperties.ForEach( candidate => {
            candidate.TextColor = Colors.Transparent;
        } );

        IsHidingAllValues = true;
    }

    public void HighlightCandidate( int candidateNumber )
    {
        if ( HasUserFacingValue || candidateNumber == 0 || candidateNumber > initCandidatesCount ) return;

        GetCandidatePropertiesByNumber( candidateNumber ).BackgroundColor = selectedCellBackgroundColor;

        highlightedCandidate = candidateNumber;
    }

    public void HighlightCellAsSelected()
    {
        IsHighlightedAsSelected = true;
        IsHighlightedAsRelated = false;
        SetCandidatesBackgroundToCellBackground();
    }

    private void SetCandidatesBackgroundToCellBackground()
        => CandidatesGridProperties.ForEach( candidate => candidate.BackgroundColor = BackgroundColor );

    public void HighlightCellAsRelated()
    {
        IsHighlightedAsSelected = false;
        IsHighlightedAsRelated = true;
        SetCandidatesBackgroundToCellBackground();
    }

    public void HighlightCellAsSelectedAndNotifyRelatedCells()
    {
        HighlightCellAsSelected();
        HighlightChanged?.Invoke( true );
    }

    internal void LockCellInternal() => IsLockedForChanges = true;

    public void RemoveAllCandidates()
        => GetAllCandidateValues().ForEach( candidate => RemoveCandidate( candidate ) );

    public void RemoveCandidate( int candidateNumber, bool addToJournal = true )
        => ChangeCandidateVisibility( candidateNumber, setAsVisible: false, addToJournal );

    public void ResetCellData( bool isLocked, int userFacingValue, int correctValue )
    {
        IsLockedForChanges = isLocked;
        UserFacingValue = userFacingValue;
        CorrectValue = correctValue;

        UnhighlightCell();
        DisplayValue();

        ResetGridProperties();
    }

    private void ResetGridProperties()
    {
        CandidatesGridProperties.ForEach( candidate => {
            candidate.Text = string.Empty;
            candidate.IsVisible = true;
            candidate.BackgroundColor = regularCellBackgroundColor;
        } );

        highlightedCandidate = 0;
    }

    public void RestartCell()
    {
        if ( IsLockedForChanges ) return;

        UserFacingValue = 0;

        UnhighlightCell();
        DisplayValue();

        ResetGridProperties();
    }

    public void RevealValues()
    {
        CandidatesGridProperties.ForEach( candidate => {
            candidate.TextColor = Colors.White;
        } );

        IsHidingAllValues = false;
    }

    public void SetUserFacingValueAndNotifyRelatedCells( int newValue, bool addToJournal )
    {
        if ( UserFacingValue == newValue ) return;

        if ( addToJournal ) {
            PlayerTransactionTypes transactionType = newValue == 0
            ? PlayerTransactionTypes.CellValueErased : HasUserFacingValue
            ? PlayerTransactionTypes.CellValueChanged : PlayerTransactionTypes.CellValueAdded;

            journal.AddTransaction( transactionType, this, UserFacingValue );
        }

        if ( newValue != 0 ) ValueFilled?.Invoke( newValue );

        UserFacingValue = newValue;
    }

    internal void SetUserFacingValueInternal( int newValue ) => UserFacingValue = newValue;

    public void UnhighlightCandidate()
    {
        if ( HasUserFacingValue || highlightedCandidate == 0 ) return;

        GetCandidatePropertiesByNumber( highlightedCandidate ).BackgroundColor = regularCellBackgroundColor;

        highlightedCandidate = 0;
    }

    public void UnhighlightCell()
    {
        IsHighlightedAsSelected = false;
        IsHighlightedAsRelated = false;
        SetCandidatesBackgroundToCellBackground();
    }

    public void UnhighlightCellAndNotifyRelatedCells()
    {
        UnhighlightCell();
        HighlightChanged?.Invoke( false );
    }
}
