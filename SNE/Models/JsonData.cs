using System.Collections.Generic;

namespace SNE.Models
{
    public class JsonData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int BPM { get; set; }
        public List<NoteData> NotesData { get; set; }
    }

    public class NoteData
    {
        public double Time { get; set; }
        public int LaneID { get; set; }
        public bool IsActionNote { get; set; }
        public int DifficultyLevel { get; set; }
    }
}
