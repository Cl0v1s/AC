using Avalonia;
using Avalonia.Markup.Xaml;

using System;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace AnimalCrossing.Client
{
    
    public partial class App : Application
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public App()
        {
        }
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            this.DataContext = new AppViewModel((this.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!, this._cancellationTokenSource);

            (this.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.ShutdownRequested +=
                this.OnShutdown;
            
            this.Talk(this._cancellationTokenSource.Token);
        }
        
        
        private async void Talk(CancellationToken token)
        {
            AppViewModel ctx = (this.DataContext as AppViewModel)!;
            try
            {
                while (token.IsCancellationRequested == false)
                {
                    try
                    {
                        ctx.State = "<Connecting...>";
                        Server server = new Server(ctx);
                        ctx.State = "<Connected>";
                        ctx.Icon = new WindowIcon(AppViewModel.Connected);
                        await server.Start(token);
                    }
                    catch (SocketException e)
                    {
                        ctx.State = "<Unable to reach server...>";
                        ctx.Icon = new WindowIcon(AppViewModel.Disconnected);
                    }

                    await Task.Delay(5000, token);
                }
            }
            catch (OperationCanceledException e)
            {
                ctx.State = "<Not connected>";
                ctx.Icon = new WindowIcon(AppViewModel.Disconnected);
            }
        }

        private void OnShutdown(object? sender, ShutdownRequestedEventArgs shutdownRequestedEventArgs)
        {
            this._cancellationTokenSource.Cancel();
        }

    }
}