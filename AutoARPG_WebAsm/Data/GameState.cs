using System.Numerics;

namespace AutoARPG_Web.Data;

[Serializable]
public class GameState
{
    public const int width = 200;
    public const int height = 150;
    public int TagWidth => width * 4;
    public int TagHeight => height * 4;

    public bool ShouldRefresh = false;
    
    public bool[] unlockedFeatures = new bool[15];
    
    public Vector3 FacingDirection;
}