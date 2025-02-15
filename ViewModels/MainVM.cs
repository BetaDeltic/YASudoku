using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using YASudoku.Views;
using YASudoku.ViewModels.GameViewModel;
using YASudoku.Services.SettingsService;
using YASudoku.Services.ResourcesService;

namespace YASudoku.ViewModels;

public partial class MainVM : VMsBase
{
    private readonly IServiceProvider serviceProvider;

    public MainVM( IServiceProvider provider )
        : base( provider.GetService<ISettingsService>()!, provider.GetService<IResourcesService>()! )
    {
        serviceProvider = provider;
    }

    [RelayCommand]
    public async Task StartGame( bool generateNew )
    {
        Debug.WriteLine( "About to start the game" );
        await Shell.Current.GoToAsync( nameof( LoadingPage ) );

        GameVM? gameVM = serviceProvider.GetService<GameVM>();
        await Task.WhenAll( // To prevent loading screen from flashing in case of too fast load
            Task.Delay( 1000 ), // Wait for at least a second
            Task.Run( () => gameVM?.PrepareGameView( generateNew ) ) // While simultaneously preparing the board
        );

        await Shell.Current.GoToAsync( nameof( GamePage ) );
    }
}
