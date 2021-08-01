using System;
using Reactive.Bindings.Interactivity;

namespace SNE.Models.Converters
{
    public class EventConverter : DelegateConverter<EventArgs, (object sender, EventArgs args)>
    {
        protected override (object sender, EventArgs args) OnConvert(EventArgs source) => (AssociateObject, source);
    }
}
