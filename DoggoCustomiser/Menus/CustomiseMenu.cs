using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DoggoCustomiser.Menus
{
    public class CustomiseMenu : IClickableMenu
    {
        private ClickableComponent coatColorPicker, collarColorPicker;


        public CustomiseMenu()
        {
            Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(950, 700, 0, 0);
        }

        public override void draw(SpriteBatch b)
        {
            Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(950, 700, 0, 0);
        }
        
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            throw new System.NotImplementedException();
        }
    }
}