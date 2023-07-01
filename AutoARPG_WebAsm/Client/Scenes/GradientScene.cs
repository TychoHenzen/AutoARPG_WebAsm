using System.Numerics;
using AutoARPG_Web.Data;
using SixLabors.ImageSharp.PixelFormats;

namespace AutoARPG_WebAsm.Scenes;

public class GradientScene : IScene
{
    public Rgb24 Render(Rgb24 pix, Vector2 position, int frame)
    {
        pix.R = (byte) (position.X * 255);
        pix.G = (byte) (position.Y * 255);
        pix.B = (byte) (frame / 15f * 255f);
        return pix;
    }

    public GameState Update(GameButton btn, GameState currentCount)
    {
        if (btn == GameButton.Btn_Act)
        {
            currentCount.unlockedFeatures[0] = !currentCount.unlockedFeatures[0];
            currentCount.ShouldRefresh = true;
        }
        
        if (btn == GameButton.Btn_Cancel)
        {
            currentCount.unlockedFeatures[1] = !currentCount.unlockedFeatures[1];
            currentCount.ShouldRefresh = true;
        }
        
        if (btn == GameButton.Btn_Options)
        {
            currentCount.unlockedFeatures[2] = !currentCount.unlockedFeatures[2];
            currentCount.ShouldRefresh = true;
        }
        return currentCount;
    }
}