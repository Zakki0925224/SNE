using Newtonsoft.Json;
using SNE.Models.Editor;
using SNE.Models.Shell;
using System;

namespace SNE.Models.Converters
{
    public static class ConvertToExportDataModel
    {
        public static ExportDataModel Convert(string jsonString)
        {
            var model = new ExportDataModel();

            try
            {
                model = JsonConvert.DeserializeObject<ExportDataModel>(jsonString);
            }
            catch (Exception e)
            {
                MessageBox.ShowErrorMessageBox(e.Message);
            }

            return model;
        }
    }
}
