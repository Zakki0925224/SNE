using Prism.Mvvm;
using Reactive.Bindings;

namespace SNE.ViewModels
{
    public class GridViewControlViewModel : BindableBase
    {
        // reactive properties
        public ReactiveProperty<double> GridWidth { get; set; } = new ReactiveProperty<double>(5);
        public ReactiveProperty<double> GridHeight { get; set; } = new ReactiveProperty<double>(5);

        public GridViewControlViewModel()
        {
            
        }
    }
}
