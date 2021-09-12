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
                    this.NoteColor = Colors.LightBlue;

                else if (_difficultyLevel == 1)
                    this.NoteColor = Colors.LightGreen;

                else if (_difficultyLevel == 2)
                    this.NoteColor = Colors.LightCoral;
            }
        }

        public Color NoteColor { get; set; }
    }
}
