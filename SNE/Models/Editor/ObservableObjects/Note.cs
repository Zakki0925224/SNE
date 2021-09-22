using System.Windows.Media;

namespace SNE.Models.Editor.ObservableModels
{
    public class Note
    {
        public double XPosition { get; set; }
        public double YPosition { get; set; }

        private int _difficultyLevel;
        public int DifficultyLevel
        {
            get { return _difficultyLevel; }
            set
            {
                _difficultyLevel = value;

                if (_difficultyLevel == 0)
                    this.NoteColor = new SolidColorBrush(Colors.LightBlue);

                else if (_difficultyLevel == 1)
                    this.NoteColor = new SolidColorBrush(Colors.LightGreen);

                else if (_difficultyLevel == 2)
                    this.NoteColor = new SolidColorBrush(Colors.LightCoral);
            }
        }

        public Brush NoteColor { get; set; }
        public int Size { get; set; }
    }
}
