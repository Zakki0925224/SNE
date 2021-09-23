using SNE.Models.Editor.ObservableModels;
using System.Collections.Generic;

namespace SNE.Models.Editor.DataModels
{
    public class ExportDataModel
    {
        public string AudioFilePath { get; set; }
        public int? BPM { get; set; }
        public int? LPB { get; set; }
        public int? Lane { get; set; }
        public int? Offset { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Note> Notes { get; set; }

        public ExportDataModel(string audioFilePath,
                               int bpm,
                               int lpb,
                               int lane,
                               int offset,
                               string title,
                               string description,
                               List<Note> notes)
        {
            this.AudioFilePath = audioFilePath;
            this.BPM = bpm;
            this.LPB = lpb;
            this.Lane = lane;
            this.Offset = offset;
            this.Title = title;
            this.Description = description;
            this.Notes = notes;
        }

        public ExportDataModel() { }

        /// <summary>
        /// データが正常にセットされているかどうか
        /// </summary>
        /// <returns>セットされている -> true, されていない -> false</returns>
        public bool IsSet()
        {
            if (this.AudioFilePath != null &&
                this.BPM != null &&
                this.LPB != null &&
                this.Lane != null &&
                this.Offset != null &&
                this.Title != null &&
                this.Description != null &&
                this.Notes != null)
                return true;

            else
                return false;
        }
    }
}
