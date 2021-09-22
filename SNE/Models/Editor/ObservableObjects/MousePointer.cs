namespace SNE.Models.Editor.ObservableObjects
{
    public class MousePointer
    {
        public double XPosition { get; set; }
        public double YPosition { get; set; }
        public double Size { get; private set; } = Const.NoteSize;
    }
}
