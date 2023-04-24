using System.Diagnostics;
using YASudoku.Common;
using YASudoku.Services.JournalingServices;

namespace YASudoku.Models;

[DebuggerDisplay( "ID:{CellID}, {ToString()}" )]
public class GameGridCell
{
    public event Action<GameGridCell>? CellInitialized;
    public event Action<GameGridCell>? CellCandidateRemoved;

    public readonly int CellID;

    public bool IsInitialized { get; private set; }
    public bool IsLockedForChanges { get; private set; }
    public int UserFacingValue { get; private set; }
    public int CorrectValue { get; private set; }

    public bool HasCorrectValue => UserFacingValue == CorrectValue;
    public bool HasUserFacingValue => UserFacingValue != 0;
    public string UserFacingText => !HasUserFacingValue ? string.Empty : UserFacingValue.ToString();

    public int CandidatesCount => candidates.Count;
    public string CandidatesText => string.Join( ',', candidates );
    public IEnumerable<int> Candidates => candidates.Select( candidate => candidate );

    private readonly int initCandidatesCount;
    internal readonly List<GameGridCell> relatedCells = new();

    private readonly IGeneratorJournalingService commandJournal;

    private List<int> candidates;

    public GameGridCell( int candidatesCount, int id, IGeneratorJournalingService commandJournal )
    {
        initCandidatesCount = candidatesCount;
        CellID = id;

        this.commandJournal = commandJournal;

        candidates = new( Enumerable.Range( 1, candidatesCount ) );
    }

    public new string ToString() => !HasUserFacingValue ? $"Candidates: {CandidatesText}" : UserFacingText;

    public void AddRelatedCell( GameGridCell relatedCell )
    {
        if ( relatedCell == this || relatedCells.Contains( relatedCell ) ) {
            return;
        }

        relatedCells.Add( relatedCell );
    }

    public void AddToCandidates( int newCandidate ) => candidates.Add( newCandidate );

    public void Initialize( int initValue )
    {
        if ( IsInitialized ) {
            if ( !HasUserFacingValue ) {
                InitializationCommon( initValue );
            }
            return;
        }

        commandJournal.AddSubTransaction( GeneratorTransactionTypes.CellValueInitialized, this, initValue );

        InitializationCommon( initValue );

        CellInitialized?.Invoke( this );
    }

    private void InitializationCommon( int initValue )
    {
        IsInitialized = true;
        UserFacingValue = initValue;

        relatedCells.ForEach( relatedCell => {
            relatedCell.RemoveFromCandidates( initValue );
        } );
    }

    public void InitializeWithLastRemainingCandidate()
    {
        if ( IsInitialized && HasUserFacingValue ) {
            return;
        }

        if ( CandidatesCount > 1 ) {
            throw new ApplicationException( "Cell has more than one candidate left!" );
        }

        Initialize( candidates[ 0 ] );
    }

    public void InitializeWithRandomCandidate( Random random )
    {
        if ( IsInitialized && HasUserFacingValue ) {
            return;
        }

        int candidateValue;
        if ( CandidatesCount > 1 ) {
            int randomCandidateIndex = random.Next( CandidatesCount );
            candidateValue = candidates[ randomCandidateIndex ];
        } else {
            candidateValue = candidates[ 0 ];
        }

        Initialize( candidateValue );
    }

    public void LockForUserChanges() => IsLockedForChanges = true;

    public bool RemoveFromCandidates( int candidate )
    {
        if ( CandidatesCount == 1 || ( IsInitialized && HasUserFacingValue ) ) {
            return false;
        }

        bool success = candidates.Remove( candidate );

        if ( !IsInitialized && success ) {
            commandJournal.AddSubTransaction( GeneratorTransactionTypes.CandidateRemoved, this, candidate );
            CellCandidateRemoved?.Invoke( this );
        }

        if ( CandidatesCount == 1 ) {
            ReserveLastCandidate();
        }

        return success;
    }

    private void ReserveLastCandidate()
    {
        if ( IsInitialized && HasUserFacingValue ) {
            return;
        }

        relatedCells.Where( relatedCell => !relatedCell.IsInitialized
            || ( relatedCell.IsInitialized && !relatedCell.HasUserFacingValue ) )
            .ForEach( relatedCell => {
                relatedCell.RemoveFromCandidates( candidates[ 0 ] );
            } );
    }

    public int RemoveFromCandidates( IEnumerable<int> candidatesToBeRemoved )
    {
        return CandidatesCount == 1 || ( IsInitialized && HasUserFacingValue )
            ? 0
            : candidatesToBeRemoved.Count( RemoveFromCandidates );
    }

    public void RemoveUserFacingValue() => UserFacingValue = 0;

    public void ResetCell()
    {
        candidates = new( Enumerable.Range( 1, initCandidatesCount ) );
        CorrectValue = 0;
        UserFacingValue = 0;
        IsInitialized = false;
        IsLockedForChanges = false;
    }

    public void ResetUserFacingValueToCorrectValue() => UserFacingValue = CorrectValue;

    public void ReverseInitialization()
    {
        UserFacingValue = 0;
        IsInitialized = false;
    }

    public void RecalculateCandidates()
    {
        candidates = new( Enumerable.Range( 1, initCandidatesCount ) );
        relatedCells
            .Where( relatedCell => relatedCell.HasUserFacingValue )
            .ForEach( relatedCell => candidates.Remove( relatedCell.UserFacingValue ) );

        if ( CandidatesCount == 0 ) {
            throw new ApplicationException( "There are no candidates left for this cell" );
        }
    }

    public void SetCorrectValueToUserFacingValue() => CorrectValue = UserFacingValue;
}
