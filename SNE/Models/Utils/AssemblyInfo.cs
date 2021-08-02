using System.Reflection;

namespace SNE.Models.Utils
{
    public static class AssemblyInfo
    {
        private static Assembly asm = Assembly.GetExecutingAssembly();
        public static string GetAssemblyTitle()
        {
            return asm.GetCustomAttribute<AssemblyTitleAttribute>().Title;
        }

        public static string GetAssemblyDescription()
        {
            return asm.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        }

        public static string GetAssemblyCopyright()
        {
            return asm.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        }

        public static string GetAssembryVersion()
        {
            return asm.GetName().Version.ToString();
        }
    }
}
