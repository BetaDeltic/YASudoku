using Serilog;
using System.Diagnostics;

namespace YASudoku.Common;

public class ExceptionLogging
{
    public static void LogException( object e )
    {
        string log = e.ToString() ?? string.Empty;
        LogExceptionInternal( log );
    }

    private static void LogExceptionInternal( string log )
    {
        if ( log == string.Empty ) {
            return;
        }

        Debug.WriteLine( log );
        Log.Error( log );
    }
}
