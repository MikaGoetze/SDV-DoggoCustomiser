using System;
using DoggoCustomiser.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace DoggoCustomiser.Menus
{
    public class CustomiseMenu : IClickableMenu
    {
        private ColorPicker coatColorPicker, collarColorPicker;

        private ClickableTextureComponent okButton;
        private ClickableButtonComponent previewButton;

        private ClickableComponent coatLabel, collarLabel, previewLabel;
        private Rectangle backgroundRectangle;
        private Vector2 dogPos;
        
            
        private void setUpPositions()
        {
            coatLabel = new ClickableComponent(
                new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize * 3, Game1.tileSize),
                "coatLabel", "Coat Color");
           
            collarLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder +  (this.width / 2) - ((int) (1.5f * Game1.tileSize)), this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize * 3, Game1.tileSize),
                "collarLabel", "Collar Color");

            previewLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - Game1.tileSize * 3, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize * 3, Game1.tileSize), "previewLabel", "Preview" );
            
            backgroundRectangle = new Rectangle(
                this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder -
                IClickableMenu.borderWidth - Game1.tileSize * 3,
                previewLabel.bounds.Y + Game1.tileSize / 2, Game1.tileSize * 3, Game1.tileSize * 3);
            
            
            dogPos = new Vector2(this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - Game1.tileSize * 2.5f, backgroundRectangle.Y + Game1.tileSize * 1.5f);
            dogPos += new Vector2(Game1.viewport.X, Game1.viewport.Y);
            
            
            //We want the coatColorPicker all the way on the left. - And everything centered vertically (with a small offset).
            coatColorPicker = new ColorPicker(coatLabel.bounds.X, coatLabel.bounds.Y + (int) (Game1.tileSize * 1.5f));
            collarColorPicker = new ColorPicker(collarLabel.bounds.X, collarLabel.bounds.Y + (int) (Game1.tileSize * 1.5f));
           
            okButton = new ClickableTextureComponent("OK",
                new Rectangle(
                    this.xPositionOnScreen + (this.width / 2) - IClickableMenu.borderWidth -
                    IClickableMenu.spaceToClearSideBorder - Game1.tileSize,
                    this.yPositionOnScreen + this.height - IClickableMenu.borderWidth -
                    IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize),
                (string) null, (string) null, Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);

            previewButton = new ClickableButtonComponent( "Preview",
                new Rectangle(
                    this.xPositionOnScreen + this.width - IClickableMenu.borderWidth -
                    IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3,
                    this.yPositionOnScreen + this.height - IClickableMenu.borderWidth -
                    IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize * 3, Game1.tileSize),
                1f 
            );

        }

        public CustomiseMenu() : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (300 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize,
            632 + IClickableMenu.borderWidth * 2, 300 + IClickableMenu.borderWidth * 2 + Game1.tileSize, false)
        {
            setUpPositions();
            
        }

        private void DrawLabel(ClickableComponent label, SpriteBatch b)
        {
            Utility.drawTextWithShadow(b, label.label, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), Color.DarkSlateGray); 
        } 
    

        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, (string) null, false);
            //This draws the daybackground over which we can show the dog (while you customise it).
            this.okButton.draw(b, Color.White, 0.75f);
            this.previewButton.draw(b);
            this.coatColorPicker.draw(b);
            this.collarColorPicker.draw(b);
            DrawLabel(this.coatLabel, b); 
            DrawLabel(this.collarLabel, b);
            DrawLabel(this.previewLabel, b);
           
            b.Draw(Game1.daybg, backgroundRectangle, Color.White);
            
            Dog dog = CustomiserMod.Instance.GetDog();
            //Now lets set up the dog's position so that he renders in the preview field.
            dog.position = dogPos;
            dog.draw(b); 
            this.drawMouse(b);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            okButton.scale = okButton.containsPoint(x, y) ? Math.Min(okButton.scale + 0.02f, okButton.baseScale + 0.1f) : Math.Max(okButton.scale - 0.02f, okButton.baseScale);
            previewButton.scale = previewButton.containsPoint(x, y)
                ? Math.Min(previewButton.scale + 0.02f, previewButton.baseScale + 0.1f)
                : Math.Max(previewButton.scale - 0.02f, previewButton.baseScale);
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (coatColorPicker.containsPoint(x, y)) CustomiserMod.Instance.ChangeCoatColor(coatColorPicker.clickHeld(x, y));
            if (collarColorPicker.containsPoint(x, y)) CustomiserMod.Instance.ChangeCollarColor(collarColorPicker.click(x, y));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (okButton.containsPoint(x, y))
            {
                //Then we have clicked the okay button, so lets reset the dog, and close the menu.
                CustomiserMod.Instance.ResetDog();
                Game1.activeClickableMenu = null;
            }
            
            if(previewButton.containsPoint(x, y)) CustomiserMod.Instance.ResetDog();
            
            if (coatColorPicker.containsPoint(x, y)) CustomiserMod.Instance.ChangeCoatColor(coatColorPicker.click(x, y));
            if (collarColorPicker.containsPoint(x, y)) CustomiserMod.Instance.ChangeCollarColor(collarColorPicker.click(x, y));
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}