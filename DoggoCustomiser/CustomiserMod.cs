using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Diagnostics.PerformanceData;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using DoggoCustomiser.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;


namespace DoggoCustomiser
{
    public class CustomiserMod : Mod, IAssetEditor
    {
        private ModConfig config;
        private IModHelper helper;

        private Color coatColor;
        private Color collarColor;

        private static CustomiserMod _instance;

        public static CustomiserMod Instance => _instance ?? (_instance = new CustomiserMod());

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Animals\dog");
        }

        private float ColorDistance(Color a, Color b)
        {
            return Vector3.Distance(a.ToVector3(), b.ToVector3());
        }

        private void FindAndLerpColorWithShading(ref Color[] pixels, Color originalColor, Color targetColor,
            Color featureColor, float colorTolerance, float shadingMultiplier)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                //Don't try and recolor transparency.
                if (pixels[i].A < 255) continue;

                var distance = ColorDistance(pixels[i], originalColor);
                if (distance < colorTolerance)
                {
                    pixels[i] = Color.Lerp(targetColor, featureColor, distance * shadingMultiplier);
                }
            }
        }

        private void FindAndLerpColor(ref Color[] pixels, Color originalColor, Color targetColor, float colorTolerance,
            bool ignoreAlpha = true)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if (ignoreAlpha && pixels[i].A < 255) continue;


                var distance = ColorDistance(pixels[i], originalColor);
                if (pixels[i] == new Color(1, 1, 1, 1))
                {
                    this.Monitor.Log("Distance: " + distance);
                }
                if (distance < colorTolerance)
                {
                    pixels[i] = targetColor;
                }
            }


        }

        private void ReadConfig()
        {
            config = this.Helper.ReadJsonFile<ModConfig>("data/{Constants.SaveFolderName}_dog.json") ?? new ModConfig();
            collarColor = config.CollarColor;
            coatColor = config.CoatColor;
        }



        public void Edit<T>(IAssetData asset)
        {
            this.Monitor.Log("Processing dog sprite.");

            //Colors of the original dog.
            Color collarColor = new Color(97, 141, 248);
            Color litCoatColor = new Color(255, 198, 0);
            Color tongueColor = new Color(255, 81, 0);

            //Colors for use throughout modification
            Color neutralFeatureColor = new Color(60, 60, 60);

            //Lets get the actual dog texture. 
            Texture2D dogTexture2D = asset.AsImage().Data;

            //Hardcoded values that make everything work.
            float coatTolerance = 0.7f;
            float collarTolerance = 0.5f;
            float shadingMultiplier = 1.0f;

            float tongueTolerance = 0.1f;

            //Lets lerp the feature color (gray) towards the target color - to make correct shading
            neutralFeatureColor = Color.Lerp(neutralFeatureColor, coatColor, 0.02f);

            //Alright - lets see what we can do to it here.
            Color[] pixels = new Color[dogTexture2D.Height * dogTexture2D.Width];
            dogTexture2D.GetData(pixels);

            //DO NOT CHANGE THE ORDER OF THESE OPERATIONS. THEY MATTER!

            //First up lets find the tongue and set it to a specific color for the sake of preserving it.
            FindAndLerpColor(ref pixels, collarColor, this.collarColor, collarTolerance);
            FindAndLerpColor(ref pixels, tongueColor, new Color(25, 25, 25, 25), tongueTolerance);
            FindAndLerpColorWithShading(ref pixels, litCoatColor, coatColor, neutralFeatureColor, coatTolerance,
                shadingMultiplier);
            FindAndLerpColor(ref pixels, new Color(25, 25, 25, 25), tongueColor, 0.01f, false);

            dogTexture2D.SetData(pixels);

            //At this point we would a patch in the new ears textures. And overlay them.
            asset.AsImage().PatchImage(dogTexture2D);
        }

        void ButtonDown(object sender, EventArgsInput e)
        {
            if (e.Button == SButton.P)
            {
                //This should reload the dog? Hopefully.
                ResetDog();
            }
            else if (e.Button == SButton.L)
            {
                //Lets show the menu
                if (Game1.activeClickableMenu == null)
                {
                    Game1.activeClickableMenu = new CustomiseMenu();
                }

            }
            else if (e.Button == SButton.I)
            {
                this.Monitor.Log("DogPos: " + GetDog().position);
            }
        }

        public void ChangeCoatColor(Color color)
        {
            coatColor = color;
        }

        public void ChangeCollarColor(Color color)
        {
            collarColor = color;
        }

        public Dog GetDog()
        {
            for (var index = 0; index < Game1.currentLocation.characters.Count; index++)
            {
                NPC character = Game1.currentLocation.characters[index];
                if (character.GetType() == typeof(Dog))
                {
                    return (Dog) character;
                }
            }
            
            //Otherwise, lets add the dog to the character list:
            Dog dog = new Dog(Game1.player.getTileX(), Game1.player.getTileY());
            Game1.currentLocation.characters.Add(dog);
            return dog;
        } 

        public void ResetDog()
        {
            config = helper.ReadConfig<ModConfig>();
            helper.Content.InvalidateCache("Animals/dog.xnb");

            Dog dog = GetDog();

            Vector2 position = dog.position;

            Game1.removeCharacterFromItsLocation(dog.name);

            dog = GetDog();
            dog.position = position;
        }


        public void Save(object sender, EventArgs e)
        {
            config.CoatColor = coatColor;
            config.CollarColor = collarColor;
            this.helper.WriteJsonFile("data/{Constants.SaveFolderName}_dog.json", config);
        }
        
        public override void Entry(IModHelper helper)
        {
            _instance = this;
            this.helper = helper;
            this.Monitor.Log("Loaded DoggoCustomiser configuration file.");
            InputEvents.ButtonPressed += ButtonDown;
            SaveEvents.BeforeSave += Save;
            ReadConfig();
        }
    }
}