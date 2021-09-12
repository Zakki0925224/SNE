namespace SNE.Models.Converters
{
    public static class ConvertXcoordinateToSecond
    {
        public static double Convert(double xPos)
        {
            return xPos / 100;
        }
    }
}
