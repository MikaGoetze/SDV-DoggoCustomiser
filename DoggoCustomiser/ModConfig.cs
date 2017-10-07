using Microsoft.Xna.Framework;

namespace DoggoCustomiser
{
    public class ModConfig
    {
        public Color CoatColor { get; set; } = new Color(139, 69, 19);
        public Color CollarColor { get; set; } = new Color(97, 141, 248);
        public float CoatTolerance  { get; set; } = 0.8f;
        public float CollarTolerance  { get; set; } = 0.5f;
        public float ShadingMultiplier  { get; set; } = 1.0f;
    }
}