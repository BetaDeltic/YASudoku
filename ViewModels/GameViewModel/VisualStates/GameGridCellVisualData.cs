using System.ComponentModel;
using YASudoku.Common;
using YASudoku.Controls.ControlBindings;
using YASudoku.Services.JournalingServices;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public class GameGridCellVisualData : INotifyPropertyChanged
{
    public event Action<bool>? HighlightChanged;
    public event Action<int>? ValueFilled;
    public event PropertyChangedEventHandler? PropertyChanged;

    public readonly int CellID;

    public bool IsLockedForChanges
    {
        get => _isLocked;
        private set {
            _isLocked = value;
            Notify( nameof( FontAttribute ) );
            Notify( nameof( TextColor ) );
        }
    }

    public int UserFacingValue
    {
        get => _userFacingValue;
        private set {
            _userFacingValue = value;
            Notify( nameof( UserFacingValue ) );
            Notify( nameof( UserFacingText ) );
            Notify( nameof( TextColor ) );
        }
    }

    public string UserFacingText => !HasUserFacingValue ? string.Empty : UserFacingValue.ToString();

    public Color TextColor =>
        IsHidingAllValues ? Colors.Transparent :
        IsLockedForChanges ? lockedColor :
        HasCorrectValue || !settings.CanHighlightMistakes() ? correctColor : incorrectColor;

    public Color BackgroundColor =>
        IsHighlightedAsSelectedInternal ? highlightedAsSelectedColor :
        IsHighlightedAsRelatedInternal ? highlightedAsRelatedColor : regularColor;

    internal bool IsHighlightedAsSelectedInternal
    {
        get => _isHighlightedAsSelected;
        private set {
            _isHighlightedAsSelected = value;
            Notify( nameof( BackgroundColor ) );
        }
    }

    internal bool IsHighlightedAsRelatedInternal
    {
        get => _isHighlightedAsRelated;
        private set {
            _isHighlightedAsRelated = value;
            Notify( nameof( BackgroundColor ) );
        }
    }

    internal bool IsHidingAllValues
    {
        get => _isHidingAllValues;
        private set {
            _isHidingAllValues = value;
            Notify( nameof( TextColor ) );
        }
    }

    internal readonly List<GameGridCellVisualData> relatedCells = new();

    public FontAttributes FontAttribute => IsLockedForChanges ? FontAttributes.Bold : FontAttributes.None;
    public int CorrectValue { get; private set; }

    public bool IsShowingValue { get; private set; } = true;
    public bool IsShowingCandidates => !IsShowingValue;
    public List<LabelOrNumberedGridBinding> CandidatesGridProperties { get; }

    public bool HasCorrectValue => UserFacingValue == CorrectValue;
    public bool HasUserFacingValue => UserFacingValue != 0;

    private readonly int initCandidatesCount;

    private readonly ISettingsService settings;
    private readonly IPlayerJournalingService journal;

    private int _userFacingValue;
    private bool _isHighlightedAsSelected;
    private bool _isHighlightedAsRelated;
    private bool _isHidingAllValues;
    private bool _isLocked;

    private int highlightedCandidate;

    private readonly Color lockedColor = Colors.White;
    private readonly Color incorrectColor = Colors.Red;
    private readonly Color correctColor = Colors.LightBlue;
    private readonly Color highlightedAsSelectedColor = Colors.Black;
    private readonly Color highlightedAsRelatedColor = Colors.DarkMagenta;
    private readonly Color regularColor = Color.FromUint( 0xFF2F2F2F );

    public GameGridCellVisualData( ISettingsService settingsService, IPlayerJournalingService journalingService,
        int candidatesCount, int id, bool isLocked, int value, int correctValue )
    {
        journal = journalingService;
        settings = settingsService;
        settings.MistakesHighlightingChanged += Settings_MistakesHighlightingChanged;

        initCandidatesCount = candidatesCount;
        CellID = id;
        IsLockedForChanges = isLocked;
        UserFacingValue = value;
        CorrectValue = correctValue;

        CandidatesGridProperties = new( candidatesCount );
        for ( int i = 0; i < candidatesCount; i++ ) {
            CandidatesGridProperties.Add( new LabelOrNumberedGridBinding() {
                IsVisible = true,
                BackgroundColor = regularColor,
                TextColor = Colors.White,
            } );
        }
    }

    private void Settings_MistakesHighlightingChanged( bool canHighlightMistakes )
    {
        if ( HasUserFacingValue && !HasCorrectValue ) Notify( nameof( TextColor ) );
    }

    private void Notify( string propertyName )
        => PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );

    public void AddCandidate( int candidateNumber, bool addToJournal = true )
        => ChangeCandidateVisibility( candidateNumber, setAsVisible: true, addToJournal );

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

    public void HighlightCandidate( int candidateNumber )
    {
        if ( HasUserFacingValue || candidateNumber == 0 || candidateNumber > initCandidatesCount ) return;

        GetCandidatePropertiesByNumber( candidateNumber ).BackgroundColor = highlightedAsSelectedColor;

        highlightedCandidate = candidateNumber;
    }

    public void HighlightCellAsSelected()
    {
        IsHighlightedAsSelectedInternal = true;
        IsHighlightedAsRelatedInternal = false;
        SetCandidatesBackgroundToCellBackground();
    }

    private void SetCandidatesBackgroundToCellBackground()
        => CandidatesGridProperties.ForEach( candidate => candidate.BackgroundColor = BackgroundColor );

    public void HighlightCellAsRelated()
    {
        IsHighlightedAsSelectedInternal = false;
        IsHighlightedAsRelatedInternal = true;
        SetCandidatesBackgroundToCellBackground();
    }

    public void HighlightCellAndNotifyRelated()
    {
        HighlightCellAsSelected();
        HighlightChanged?.Invoke( true );
    }

    private LabelOrNumberedGridBinding GetCandidatePropertiesByNumber( int candidateNumber )
    {
        return candidateNumber == 0 || candidateNumber > initCandidatesCount
            ? throw new ArgumentOutOfRangeException( nameof( candidateNumber ) )
            : CandidatesGridProperties[ candidateNumber - 1 ];
    }

    public void UnhighlightCellAndNotifyRelated()
    {
        UnhighlightCell();
        HighlightChanged?.Invoke( false );
    }

    public void DisplayValue()
    {
        if ( IsShowingValue ) return;

        IsShowingValue = true;

        CandidatesGridProperties.ForEach( x => x.IsVisible = false );

        Notify( nameof( IsShowingValue ) );
        Notify( nameof( IsShowingCandidates ) );
    }

    public void DisplayCandidates()
    {
        if ( HasUserFacingValue || IsShowingCandidates ) return;

        IsShowingValue = false;

        CandidatesGridProperties.ForEach( x => x.IsVisible = true );

        Notify( nameof( IsShowingValue ) );
        Notify( nameof( IsShowingCandidates ) );
    }

    public IEnumerable<int> GetAllCandidateValues() =>
        CandidatesGridProperties
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
            candidate.BackgroundColor = regularColor;
        } );

        highlightedCandidate = 0;
    }

    public void RestartValue()
    {
        if ( IsLockedForChanges ) return;

        UserFacingValue = 0;
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

    public void UnhighlightCandidate()
    {
        if ( HasUserFacingValue || highlightedCandidate == 0 ) return;

        GetCandidatePropertiesByNumber( highlightedCandidate ).BackgroundColor = regularColor;

        highlightedCandidate = 0;
    }

    public void UnhighlightCell()
    {
        IsHighlightedAsSelectedInternal = false;
        IsHighlightedAsRelatedInternal = false;
        SetCandidatesBackgroundToCellBackground();
    }
}
