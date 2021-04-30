using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewModdingAPI;

namespace WhiteTurnip
{
    class ModResource
    {
        public static string assetsFolderPath;
        public static string imgFolderPath;
        public static string daisyMaeTexturePath;

        public static Texture2D daisyMaePortraitTexture;
        public static Texture2D daisyMaeMiniPortraitsTexture;

        static IModHelper _helper;

        public static void InitAssets(IModHelper helper)
        {
            _helper = helper;

            assetsFolderPath = helper.Content.GetActualAssetKey("assets", ContentSource.ModFolder);
            imgFolderPath = Path.Combine(assetsFolderPath, "img");
            daisyMaeTexturePath = Path.Combine(imgFolderPath, "DaisyMae_sprite.png");

            daisyMaePortraitTexture = helper.Content.Load<Texture2D>(Path.Combine(imgFolderPath, "DaisyMae_portrait.png"));
            daisyMaeMiniPortraitsTexture = helper.Content.Load<Texture2D>(Path.Combine(imgFolderPath, "DaisyMae_mini_portrait.png"));
        }

        public static string getTranslation(string key)
        {
            return _helper.Translation.Get(key);
        }
    }
}
