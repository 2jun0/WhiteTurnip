using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SpaceShared.APIs;
using SObject = StardewValley.Object;
using WhiteTurnip.turnip;
using WhiteTurnip.turnip.price;
using WhiteTurnip.common.utils;
using WhiteTurnip.common.core;

namespace WhiteTurnip
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        // Mod data related
        public const string DAISYMAE_INTRO_KEY = "ejun0.WhiteTurnip.DaisyMae_Introduction";
        public static JsonAssetsAPI jsonAssets;

        private readonly TurnipContext turnipContext;
        private readonly ModData modData;

        private TurnipWeekPrice weekPrice;
        private TimeData lastestTimeData = null;

        public static ModEntry instance;

        public static int wt_id;
        public static int sp_id;

        public ModEntry()
        {
            turnipContext = new TurnipContext();
            modData = new ModData();
        }

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
            SObject spoiledTurnip = new SObject(sp_id, 1);
            // Rot all turnips
            Utility.iterateAllItems(
                delegate (Item item)
                {
                    if(item.parentSheetIndex == wt_id)
                    {
                        SObject sobj = (SObject)item;
                        sobj.ParentSheetIndex = spoiledTurnip.ParentSheetIndex;
                        sobj.Category = spoiledTurnip.Category;
                        sobj.Price = spoiledTurnip.Price;
                        sobj.DisplayName = spoiledTurnip.DisplayName;
                        sobj.Name = spoiledTurnip.Name;
                        sobj.name = spoiledTurnip.name;
                        sobj.Stack = spoiledTurnip.Stack;
                        sobj.Type = spoiledTurnip.Type;
                        sobj.Edibility = spoiledTurnip.Edibility;
                    }
                });
        }

        private void SetWhiteTurnipPrice(int price)
        {
            float multiplier = 1.1f * Game1.MasterPlayer.difficultyModifier;
            int realPrice = (int)Math.Ceiling(price / multiplier);

            // Set all turnip price
            Utility.iterateAllItems(
                delegate (Item item)
                {
                    if (item.parentSheetIndex == wt_id)
                    {
                        (item as SObject).Price = realPrice;
                    }
                });
        }

        private void UpdateLastestTimeData()
        {
            int daysPlayed = (int)Game1.stats.DaysPlayed;
            int timeOfDay = (int)Game1.timeOfDay;

            lastestTimeData = new TimeData(daysPlayed, timeOfDay);
        }

        private void UpdateTurnipPrice(TimeData prevTimeData)
        {
            // 저장된 값과 주가 다를 경우 무 값 재 갱신
            if (prevTimeData.Week() != Days.Week())
                resetWeekPrice();

            if (Days.Day() != prevTimeData.DaysPlayed || Days.IsAfternoon() != prevTimeData.isAfternoon())
                SetWhiteTurnipPrice(weekPrice.GetPrice(Days.DayOfWeek(), Days.IsAfternoon()));
        }

        private void CheckRottable(TimeData prevTimeData)
        {
            // 지난 날짜로 돌아간 경우 -> 썩음
            if (prevTimeData.DaysPlayed > Days.Day()) 
            {
                RotTurnips();
            }
            // 오늘이지만 시간이 과거인 경우 -> 썩음
            if (prevTimeData.DaysPlayed == Days.Day() &&
                prevTimeData.TimeOfDay > Days.Time())
            {
                RotTurnips();
            }
            // 다음 주 이상으로 넘어간 경우 -> 썩음
            if (prevTimeData.Week() != Days.Week())
            {
                RotTurnips();
            }
        }

        private void resetWeekPrice()
        {
            weekPrice = turnipContext.WeekPriceFactory().Create();
        }

        /*********
        ** Data relation methods
        *********/
        private void SaveModData()
        {
            modData.SetTimeData(lastestTimeData);
        }

        private void LoadModData()
        {
            lastestTimeData = modData.TimeData();
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
            if (this.lastestTimeData.DaysPlayed >= 0)
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
            resetWeekPrice();

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
            if (this.lastestTimeData.DaysPlayed >= 0)
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