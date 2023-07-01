using System.Numerics;
using AutoARPG_Web.Data;
using SixLabors.ImageSharp.PixelFormats;

namespace AutoARPG_WebAsm.Scenes;

public interface IScene
{
    Rgb24 Render(Rgb24 pix, Vector2 position, int frame);
    GameState Update(GameButton btn, GameState currentCount);
}