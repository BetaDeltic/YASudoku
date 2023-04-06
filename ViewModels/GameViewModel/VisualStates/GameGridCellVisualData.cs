using System.ComponentModel;
using YASudoku.Controls.ControlBindings;
using YASudoku.Common;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public class GameGridCellVisualData : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public readonly int CellID;

    public bool IsLockedForChanges { get; private set; }

    public int UserFacingValue
    {
        get => _userFacingValue;
        set {
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

    public FontAttributes FontAttribute => IsLockedForChanges ? FontAttributes.Bold : FontAttributes.None;
    public int CorrectValue { get; private set; }

    public bool IsShowingValue { get; private set; } = true;
    public bool IsShowingCandidates => !IsShowingValue;
    public List<LabelOrNumberedGridBinding> CandidatesGridProperties { get; private set; }

    public bool HasCorrectValue => UserFacingValue == CorrectValue;
    public bool HasUserFacingValue => UserFacingValue != 0;

    private readonly int initCandidatesCount;
    internal readonly List<GameGridCellVisualData> relatedCells = new();

    private readonly ISettingsService settings;

    private int _userFacingValue;
    private bool _isHighlightedAsSelected;
    private bool _isHighlightedAsRelated;
    private int highlightedCandidate;
    private bool _isHidingAllValues;

    private readonly Color lockedColor = Colors.White;
    private readonly Color incorrectColor = Colors.Red;
    private readonly Color correctColor = Colors.LightBlue;
    private readonly Color highlightedAsSelectedColor = Colors.Black;
    private readonly Color highlightedAsRelatedColor = Colors.DarkMagenta;
    private readonly Color regularColor = Color.FromUint( 0xFF2F2F2F );

    public GameGridCellVisualData( ISettingsService settingsService, int candidatesCount, int id, bool isLocked, int value, int correctValue )
    {
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

    public void HighlightCellAsSelected()
    {
        IsHighlightedAsSelectedInternal = true;
        IsHighlightedAsRelatedInternal = false;
        Notify( nameof( BackgroundColor ) );
        SetCandidatesBackgroundToCellBackground();
    }

    public void HighlightCellAsRelated()
    {
        IsHighlightedAsSelectedInternal = false;
        IsHighlightedAsRelatedInternal = true;
        Notify( nameof( BackgroundColor ) );
        SetCandidatesBackgroundToCellBackground();
    }

    private void SetCandidatesBackgroundToCellBackground()
        => CandidatesGridProperties.ForEach( candidate => candidate.BackgroundColor = BackgroundColor );

    public void HighlightCellAndRelatedCells()
    {
        HighlightCellAsSelected();
        relatedCells.ForEach( relatedCell => relatedCell.HighlightCellAsRelated() );
    }

    public void HighlightCandidate( int candidateNumber )
    {
        if ( HasUserFacingValue || candidateNumber == 0 || candidateNumber > initCandidatesCount )
            return;

        GetCandidatePropertiesByNumber( candidateNumber ).BackgroundColor = highlightedAsSelectedColor;

        highlightedCandidate = candidateNumber;
    }

    public void UnhighlightCell()
    {
        IsHighlightedAsSelectedInternal = false;
        IsHighlightedAsRelatedInternal = false;
        Notify( nameof( BackgroundColor ) );
        SetCandidatesBackgroundToCellBackground();
    }

    public void UnhighlightCandidate()
    {
        if ( HasUserFacingValue || highlightedCandidate == 0 )
            return;

        GetCandidatePropertiesByNumber( highlightedCandidate ).BackgroundColor = regularColor;

        highlightedCandidate = 0;
    }

    private LabelOrNumberedGridBinding GetCandidatePropertiesByNumber( int candidateNumber )
    {
        return candidateNumber == 0 || candidateNumber > initCandidatesCount
            ? throw new ArgumentOutOfRangeException( nameof( candidateNumber ) )
            : CandidatesGridProperties[ candidateNumber - 1 ];
    }

    public void UnhighlightCellAndRelatedCells()
    {
        UnhighlightCell();
        relatedCells.ForEach( relatedCell => relatedCell.UnhighlightCell() );
    }

    public void ShowValue()
    {
        IsShowingValue = true;

        CandidatesGridProperties.ForEach( x => x.IsVisible = false );

        Notify( nameof( IsShowingValue ) );
        Notify( nameof( IsShowingCandidates ) );
    }

    public void ShowCandidates()
    {
        if ( HasUserFacingValue )
            return;

        IsShowingValue = false;

        CandidatesGridProperties.ForEach( x => x.IsVisible = true );

        Notify( nameof( IsShowingValue ) );
        Notify( nameof( IsShowingCandidates ) );
    }

    public void HideAllCandidates()
        => CandidatesGridProperties.ForEach( candidate => candidate.Text = string.Empty );

    public IEnumerable<int> GetAllCandidateValues() =>
        CandidatesGridProperties
        .Where( candidate => candidate.Text != string.Empty )
        .Select( candidate => int.Parse( candidate.Text ) );

    public void ChangeCandidateVisibility( int candidateNumber, bool setAsVisible )
    {
        if ( HasUserFacingValue )
            return;

        GetCandidatePropertiesByNumber( candidateNumber ).Text =
            setAsVisible ? candidateNumber.ToString() : string.Empty;
    }

    public void MakeCandidateInvisibleInRelatedCells( int candidateNumber, out IEnumerable<GameGridCellVisualData> affectedCells )
    {
        if ( candidateNumber == 0 ) {
            affectedCells = Enumerable.Empty<GameGridCellVisualData>();
            return;
        }

        affectedCells = relatedCells.Where( relatedCell => relatedCell.HasNumberAsCandidate( candidateNumber ) ).ToList();
        affectedCells.ForEach( cell => cell.ChangeCandidateVisibility( candidateNumber, false ) );
    }

    public bool HasNumberAsCandidate( int candidateNumber )
        => GetCandidatePropertiesByNumber( candidateNumber ).Text != string.Empty;

    public void ResetCellData( bool isLocked, int userFacingValue, int correctValue )
    {
        IsLockedForChanges = isLocked;
        UserFacingValue = userFacingValue;
        CorrectValue = correctValue;

        UnhighlightCell();
        IsShowingValue = true;

        ResetGridProperties();
    }

    public void RestartValue()
    {
        if ( IsLockedForChanges )
            return;

        UserFacingValue = 0;
    }

    public void SetTextColorToTransparent()
    {
        CandidatesGridProperties.ForEach( candidate => {
            candidate.TextColor = Colors.Transparent;
        } );

        IsHidingAllValues = true;
    }

    public void SetTextColorToRegular()
    {
        CandidatesGridProperties.ForEach( candidate => {
            candidate.TextColor = Colors.White;
        } );

        IsHidingAllValues = false;
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
}
