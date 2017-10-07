using System;
using System.Diagnostics.Contracts;
using System.Diagnostics.PerformanceData;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace DoggoCustomiser
{
    public class CustomiserMod : Mod, IAssetEditor
    {
        private ModConfig config;
        private IModHelper helper;
        
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Animals\dog");
        }

        private float ColorDistance(Color a, Color b)
        {
            return Vector3.Distance(a.ToVector3(), b.ToVector3());
        }

        private void FindAndLerpColorWithShading(ref Color[] pixels, Color originalColor, Color targetColor, Color featureColor, float colorTolerance, float shadingMultiplier)
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

        private void FindAndLerpColor(ref Color[] pixels, Color originalColor, Color targetColor, float colorTolerance, bool ignoreAlpha = true)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if ( ignoreAlpha && pixels[i].A < 255) continue;
                
                
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
            
            //Let's read the intended t arget colors from the config file.
            Color targetCoatColor = config.CoatColor;
            Color targetCollarColor = config.CollarColor;
            
            //Hardcoded values that make everything work.
            float coatTolerance = 0.8f;
            float collarTolerance = 0.5f;
            float shadingMultiplier = 1.0f;

            float tongueTolerance = 0.1f;
               
            //Lets lerp the feature color (gray) towards the target color - to make correct shading
            neutralFeatureColor = Color.Lerp(neutralFeatureColor, targetCoatColor, 0.02f);
            
            //Alright - lets see what we can do to it here.
            Color[] pixels = new Color[dogTexture2D.Height * dogTexture2D.Width];
            dogTexture2D.GetData(pixels);
                
            //DO NOT CHANGE THE ORDER OF THESE OPERATIONS. THEY MATTER!
            
            //First up lets find the tongue and set it to a specific color for the sake of preserving it.
            FindAndLerpColor(ref pixels, tongueColor, new Color(25, 25, 25, 25), tongueTolerance);
            FindAndLerpColor(ref pixels, collarColor, targetCollarColor, collarTolerance);
            FindAndLerpColorWithShading(ref pixels, litCoatColor, targetCoatColor, neutralFeatureColor, coatTolerance, shadingMultiplier);
            FindAndLerpColor(ref pixels, new Color(25, 25, 25, 25), tongueColor, 0.01f, false);
            
            dogTexture2D.SetData(pixels);
            
            //At this point we would a patch in the new ears textures. And overlay them.
            asset.AsImage().PatchImage(dogTexture2D);
        }


        
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            this.helper = helper;
            this.Monitor.Log("Loaded DoggoCustomiser configuration file.");
        }
    }
}