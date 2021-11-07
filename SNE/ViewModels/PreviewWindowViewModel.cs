using Prism.Mvvm;
using Reactive.Bindings;
using System;
using SNE.Models.Editor;
using SNE.Models.Shell;

namespace SNE.ViewModels
{
    public class PreviewWindowViewModel : BindableBase
    {
        public ReactiveProperty<string> Title { get; set; } = new ReactiveProperty<string>($"Preview - {Const.AppName}");
        public ReactiveProperty<AudioPlayer> AudioPlayer { get; set; } = new ReactiveProperty<AudioPlayer>();
        public ReactiveProperty<string> JsonString { get; set; } = new ReactiveProperty<string>();

        public PreviewWindowViewModel()
        {
            SubscribeCommands();
        }

        private void SubscribeCommands()
        {
            this.AudioPlayer.Subscribe(_ =>
            {
                var player = this.AudioPlayer.Value;

                if (player != null)
                    MessageBox.ShowInfoMessageBox(player.ToString());
            });

            this.JsonString.Subscribe(_ =>
            {
                var str = this.JsonString.Value;

                if (str != null)
                    MessageBox.ShowInfoMessageBox(str);
            });
        }
    }
}
