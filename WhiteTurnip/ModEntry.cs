using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using SpaceShared.APIs;
using SObject = StardewValley.Object;

namespace WhiteTurnip
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        private static JsonAssetsAPI jsonAssets;

        private TurnipPrice turnipPrice;
        private TimeData lastestTimeData;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            turnipPrice = new TurnipPrice();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                foreach (int itemID in data.Keys)
                {
                    string[] fields = data[itemID].Split('/');
                    if (fields[0] == "White Turnip" && lastestTimeData.daysPlayed != 0)
                    {
                        fields[1] = turnipPrice.GetTurnipPrice(lastestTimeData).ToString();
                        data[itemID] = string.Join("/", fields);

                        this.Monitor.Log($"{fields[0]}의 가격은 {fields[1]}로 결정되었다.", LogLevel.Debug);
                        return;
                    }
                }
            }
        }

        /*********
        ** Private methods
        *********/
        private void RotTurnips()
        {
            // Rot turnips in inventory
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] != null && Game1.player.Items[i].Name == "White Turnip")
                {
                    Game1.player.Items[i] = new SObject(jsonAssets.GetObjectId("Spoiled Turnip"), 1);
                }
            }
        }

        private void SetWhiteTurnipPrice(int price)
        {
            this.Helper.Content.InvalidateCache("Data/ObjectInformation");
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] != null && Game1.player.Items[i].Name == "White Turnip")
                {
                    Game1.player.Items[i] = new SObject(Game1.player.Items[i].ParentSheetIndex, Game1.player.Items[i].Stack, price:price);
                }
            }
        }

        private void UpdateLastestTimeData()
        {
            int daysPlayed = (int)Game1.stats.DaysPlayed;
            int timeOfDay = (int)Game1.timeOfDay;

            if (lastestTimeData.daysPlayed / 7 != daysPlayed / 7)
                turnipPrice.Update(daysPlayed / 7);

            this.lastestTimeData.daysPlayed = daysPlayed;
            this.lastestTimeData.timeOfDay = timeOfDay;
        }

        private void UpdateTurnipPrice()
        {
            int daysPlayed = (int)Game1.stats.DaysPlayed;
            int timeOfDay = (int)Game1.timeOfDay;

            if(daysPlayed != lastestTimeData.daysPlayed || (timeOfDay < 1200) != (lastestTimeData.timeOfDay < 1200))
                SetWhiteTurnipPrice(turnipPrice.GetTurnipPrice(lastestTimeData));
        }

        private void CheckRottable()
        {
            int daysPlayed = (int)Game1.stats.DaysPlayed;
            int timeOfDay = (int)Game1.timeOfDay;

            // 지난 날짜로 돌아간 경우 -> 썩음
            if (lastestTimeData.daysPlayed > daysPlayed) RotTurnips();
            // 오늘이지만 시간이 과거인 경우 -> 썩음
            else if (lastestTimeData.daysPlayed == daysPlayed && lastestTimeData.timeOfDay > timeOfDay) RotTurnips();
            // 다음 주 이상으로 넘어간 경우 -> 썩음
            else if (lastestTimeData.daysPlayed / 7 != daysPlayed / 7) RotTurnips();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            jsonAssets = this.Helper.ModRegistry.GetApi<JsonAssetsAPI>("spacechase0.JsonAssets");
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            CheckRottable();
            UpdateTurnipPrice();
            UpdateLastestTimeData();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            UpdateLastestTimeData();
            turnipPrice.Update(this.lastestTimeData.daysPlayed / 7);
            SetWhiteTurnipPrice(turnipPrice.GetTurnipPrice(lastestTimeData));
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            var nextDay = lastestTimeData.daysPlayed + 1;
            if (nextDay / 7 != lastestTimeData.daysPlayed / 7)
            {
                RotTurnips();
            }
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            foreach(Item item in e.Added)
            {
                 if(item.Name != "White Turnip")
                {
                    continue;
                }

                for(var i = 0; i < Game1.player.Items.Count; i++)
                {
                    if(Game1.player.Items[i] == item)
                    {
                        new SObject(item.ParentSheetIndex, item.Stack);
                    }
                }
            }
        }
    }
}