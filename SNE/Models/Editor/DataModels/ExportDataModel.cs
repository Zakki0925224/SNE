using SNE.Models.Editor.ObservableModels;
using System.Collections.Generic;

namespace SNE.Models.Editor.DataModels
{
    public class ExportDataModel
    {
        public string AudioFilePath { get; set; } = null;
        public double? BPM { get; set; } = null;
        public int? Lane { get; set; } = null;
        public string Title { get; set; } = null;
        public string Description { get; set; } = null;
        public List<Note> Notes { get; set; } = null;

        public ExportDataModel(string audioFilePath,
                               double bpm,
                               int lane,
                               string title,
                               string description,
                               List<Note> notes)
        {
            this.AudioFilePath = audioFilePath;
            this.BPM = bpm;
            this.Lane = lane;
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
                this.Lane != null &&
                this.Title != null &&
                this.Description != null &&
                this.Notes != null)
                return true;

            else
                return false;
        }
    }
}
