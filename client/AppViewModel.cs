using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace AnimalCrossing.Client;




public class AppViewModel : ReactiveObject
{
    public const string Connected = "resources/connected.png";
    public const string Disconnected = "resources/disconnected.png";

    private WindowIcon _icon = new WindowIcon(Disconnected);

    public WindowIcon Icon
    {
        get => this._icon;
        set => this.RaiseAndSetIfChanged(ref this._icon, value);
    }
    
    private string _state = "<Connecting to server...>";

    public string State
    {
        get => this._state;
        set => this.RaiseAndSetIfChanged(ref this._state, value);
    }

    private readonly IClassicDesktopStyleApplicationLifetime _lifetime;
    
    public AppViewModel(IClassicDesktopStyleApplicationLifetime lifetime, CancellationTokenSource tokenSource)
    {
        this._lifetime = lifetime;
    }

    private void Exit()
    {
        this._lifetime.TryShutdown();
    }
}