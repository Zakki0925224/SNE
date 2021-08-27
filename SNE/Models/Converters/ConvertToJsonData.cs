using Newtonsoft.Json;
using SNE.Models.Editor;
using SNE.Models.Editor.DataModels;
using SNE.Models.Editor.ObservableModels;
using System;
using System.Collections.Generic;

namespace SNE.Models.Converters
{
    public static class ConvertToJsonData
    {
        public static string ConvertToExportJson(string title,
                                                 string description,
                                                 List<Note> notes,
                                                 double gridHeight,
                                                 double BPM)
        {
            var data = new JsonDataModel();
            data.Title = title;
            data.Description = description;
            data.BPM = (int)Math.Round(BPM);
            data.NotesData = new List<NoteDataModel>();

            foreach (var note in notes)
            {
                var noteData = new NoteDataModel();
                noteData.Time = ConvertXPositionToSecond(note.XPosition, BPM);
                noteData.LaneID = ConvertYPositionToLaneID(note.YPosition, gridHeight);
                noteData.IsActionNote = false;
                noteData.DifficultyLevel = 0;

                data.NotesData.Add(noteData);
            }

            return JsonConvert.SerializeObject(data);
        }

        public static string ConvertToProjectDataJson(ExportDataModel model)
        {
            return JsonConvert.SerializeObject(model);
        }

        private static double ConvertXPositionToSecond(double xPos, double BPM)
        {
            return (6 / BPM) * xPos;
        }

        private static int ConvertYPositionToLaneID(double yPos, double gridHeigt)
        {
            return (int)(yPos / (gridHeigt * 2));
        }
    }
}
