namespace AutoARPG_Web.Data;

public struct MyColor
{
    public readonly byte Red;
    public readonly byte Green;
    public readonly byte Blue;

    public MyColor(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    public static MyColor FromFloat(float red, float green, float blue)
    {
        return new MyColor((byte) (red * 255),
            (byte) (green * 255),
            (byte) (blue * 255));
    }
}