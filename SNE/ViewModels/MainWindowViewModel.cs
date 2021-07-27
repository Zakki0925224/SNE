using Prism.Mvvm;
using Reactive.Bindings;

namespace SNE.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ReactiveProperty<string> Title { get; set; } = new ReactiveProperty<string>("SimpleNotesEditor");

        public MainWindowViewModel()
        {

        }
    }
}
