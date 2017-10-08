using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DoggoCustomiser.UI
{
    public class ClickableButtonComponent : ClickableComponent
    {

        public float scale;
        public float baseScale;

        public ClickableButtonComponent(string name, Rectangle bounds, float scale) : base(bounds, name)
        {
            this.scale = scale;
            this.baseScale = scale;
        }

        public void draw(SpriteBatch b)
        {
            this.draw(b, Color.White);  
        }
        
        public void draw(SpriteBatch b, Color c)
        {
            if (!this.visible)
                return;
            if (string.IsNullOrEmpty(this.name))
                return;
            b.DrawString(Game1.smallFont, this.name, new Vector2((float) this.bounds.X, (float) this.bounds.Y + ((float) (this.bounds.Height / 2) - Game1.smallFont.MeasureString(this.name).Y / 2f)), Game1.textColor);

        }
        
    }
}