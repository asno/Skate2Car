public static class Global
{
    private const int PIXELS_PER_UNIT = 33;
    public static int ScreenWidthInPixel => 704;
    public static float ScreenWidthInUUnit => (float)ScreenWidthInPixel / (float)PIXELS_PER_UNIT;
    public static float ScreenLeft => 0 - ScreenWidthInUUnit / 2;
    public static float ScreenRight => 0 + ScreenWidthInUUnit / 2;
    public static float ScrollingSpeed { get;  set; }
}
