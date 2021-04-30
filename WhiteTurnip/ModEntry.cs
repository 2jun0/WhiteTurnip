using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SpaceShared.APIs;
using SObject = StardewValley.Object;

namespace WhiteTurnip
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        // Mod data related
        public const string DAISYMAE_INTRO_KEY = "ejun0.WhiteTurnip.DaisyMae_Introduction";

        public static JsonAssetsAPI jsonAssets;

        private TurnipPrice turnipPrice;
        private TimeData lastestTimeData;

        public static ModEntry instance;

        private int wt_id;
        private int sp_id;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;
            turnipPrice = new TurnipPrice();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            ModResource.InitAssets(helper);
        }

        /*********
        ** Private methods
        *********/
        private void RotTurnips()
        {
            // Rot all turnips
            Utility.iterateAllItems(
                delegate (Item item)
                {
                    if(item.parentSheetIndex == wt_id)
                    {
                        item.ParentSheetIndex = sp_id;
                        item.Stack = 1;
                    }
                });
        }

        private void SetWhiteTurnipPrice(int price)
        {
            // Set all turnip price
            Utility.iterateAllItems(
                delegate (Item item)
                {
                    if (item.parentSheetIndex == wt_id)
                    {
                        ((SObject)item).Price = price;
                    }
                });
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

            if (daysPlayed != lastestTimeData.daysPlayed || (timeOfDay < 1200) != (lastestTimeData.timeOfDay < 1200))
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
            jsonAssets.IdsAssigned += delegate(object sender2, EventArgs e2)
            {
                wt_id = jsonAssets.GetObjectId("White Turnip");
                sp_id = jsonAssets.GetObjectId("Spoiled Turnip");
            };
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            CheckRottable();
            UpdateTurnipPrice();
            UpdateLastestTimeData();
            DaisyMaeManager.TickUpdate();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            UpdateLastestTimeData();
            turnipPrice.Update(this.lastestTimeData.daysPlayed / 7);
            SetWhiteTurnipPrice(turnipPrice.GetTurnipPrice(lastestTimeData));

            DaisyMaeManager.createNPC();

            wt_id = jsonAssets.GetObjectId("White Turnip");
            sp_id = jsonAssets.GetObjectId("Spoiled Turnip");
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            DaisyMaeManager.GoHome();

            var nextDay = lastestTimeData.daysPlayed + 1;
            if (nextDay / 7 != lastestTimeData.daysPlayed / 7)
            {
                RotTurnips();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            DaisyMaeManager.Update();
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            foreach (Item item in e.Added)
            {
                if (item.Name != "White Turnip")
                    continue;
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                if (e.Button != SButton.MouseLeft)
                    return;
                if (e.Cursor.GrabTile != e.Cursor.Tile)
                    return;
            }
            else if (!e.Button.IsActionButton())
                return;

            if (Game1.currentLocation != DaisyMaeManager.DaisyMae.currentLocation)
                return;

            if (this.Helper.Input.GetCursorPosition().GrabTile != DaisyMaeManager.DaisyMae.getTileLocation())
                return;

            DaisyMaeManager.DisplayDialogue(Game1.player);
        }
    }
}