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

        public static int wt_id;
        public static int sp_id;

        public const int INF = int.MaxValue;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
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

            this.lastestTimeData.daysPlayed = daysPlayed;
            this.lastestTimeData.timeOfDay = timeOfDay;
        }

        private void UpdateTurnipPrice(TimeData prevTimeData)
        {
            int daysPlayed = (int)Game1.stats.DaysPlayed;
            int timeOfDay = (int)Game1.timeOfDay;

            // 저장된 값과 주가 다를 경우 무 값 재 갱신
            if (prevTimeData.daysPlayed / 7 != daysPlayed / 7)
                this.turnipPrice.Update();

            if (daysPlayed != prevTimeData.daysPlayed || (timeOfDay < 1200) != (prevTimeData.timeOfDay < 1200))
                SetWhiteTurnipPrice(turnipPrice.GetTurnipPrice());
        }

        private void CheckRottable(TimeData prevTimeData)
        {
            int daysPlayed = (int)Game1.stats.DaysPlayed;
            int timeOfDay = (int)Game1.timeOfDay;

            // 지난 날짜로 돌아간 경우 -> 썩음
            if (prevTimeData.daysPlayed > daysPlayed) RotTurnips();
            // 오늘이지만 시간이 과거인 경우 -> 썩음
            else if (prevTimeData.daysPlayed == daysPlayed && prevTimeData.timeOfDay > timeOfDay) RotTurnips();
            // 다음 주 이상으로 넘어간 경우 -> 썩음
            else if (prevTimeData.daysPlayed / 7 != daysPlayed / 7) RotTurnips();
        }

        /*********
        ** Data relation methods
        *********/
        private void SaveModData()
        {
            Game1.player.modData["ejun0.WhiteTurnip.LastestTimeData"] = string.Format("{0},{1}", this.lastestTimeData.daysPlayed, this.lastestTimeData.timeOfDay);
        }

        private void LoadModData()
        {
            if (Game1.player.modData.ContainsKey("ejun0.WhiteTurnip.LastestTimeData"))
            {
                string[] tmps = Game1.player.modData["ejun0.WhiteTurnip.LastestTimeData"].Split(',');
                this.lastestTimeData.daysPlayed = int.Parse(tmps[0]);
                this.lastestTimeData.timeOfDay = int.Parse(tmps[1]);
            }
            else
            {
                this.lastestTimeData.daysPlayed = -INF;
                this.lastestTimeData.timeOfDay = -INF; 
            }
        }

        /*********
        ** Event methods
        *********/

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
            if (this.lastestTimeData.daysPlayed != -INF)
            {
                CheckRottable(this.lastestTimeData);
                UpdateTurnipPrice(this.lastestTimeData);
            }
            UpdateLastestTimeData();

            DaisyMaeManager.TickUpdate();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            LoadModData();
            this.turnipPrice = new TurnipPrice();

            DaisyMaeManager.createNPC();

            wt_id = jsonAssets.GetObjectId("White Turnip");
            sp_id = jsonAssets.GetObjectId("Spoiled Turnip");
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            SaveModData();

            DaisyMaeManager.GoHome();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (this.lastestTimeData.daysPlayed != -INF)
            {
                CheckRottable(this.lastestTimeData);
                UpdateTurnipPrice(this.lastestTimeData);
            }
            UpdateLastestTimeData();

            DaisyMaeManager.Update();
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