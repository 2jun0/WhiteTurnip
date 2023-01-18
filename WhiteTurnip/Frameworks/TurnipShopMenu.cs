using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using SObject = StardewValley.Object;
using WhiteTurnip;
using WhiteTurnip.Turnip;

namespace WhiteTurnip.Frameworks
{
    class TurnipShopMenu: IClickableMenu
    {
        /*********
        ** Fields
        *********/
        // text box 관련 
        private TextBox QuestionBox;
        private TextBox SumPriceBox;

        // count box 관련
        private DigitBox CountBox;
        private Rectangle CountBoxBounds;
        private ClickableComponent CountBoxArea;
        private int count = 0;
        private int buyableCount = 0;

        // ok button
        private ClickableTextureComponent OkButton;

        // portrait 관련
        private Vector2 PortraitPosition;

        // 상수
        public const int ButtonBorderWidth = 4 * Game1.pixelZoom;
        public const int MaxCount = 100000;
        public readonly Texture2D TextTexture;

        public readonly Action<int> OnBuy;
        public readonly Action<string> OnFail;
        public readonly Farmer who;

        public TurnipShopMenu(Farmer who, Action<int> onBuy, Action<string> onFail)
            :base(
                (int)((Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2) * (Game1.options.zoomLevel / Game1.options.uiScale)),
                (int)((Game1.viewport.Height / 3 * 2 - (250 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize) * (Game1.options.zoomLevel / Game1.options.uiScale)),
                (int)((632 + IClickableMenu.borderWidth * 2)*(Game1.options.zoomLevel / Game1.options.uiScale)),
                (int)((250 + IClickableMenu.borderWidth * 2 + Game1.tileSize)*(Game1.options.zoomLevel / Game1.options.uiScale)))
        {
            this.who = who;
            this.TextTexture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");

            this.OnBuy = onBuy;
            this.OnFail = onFail;

            this.buyableCount = who.Money / TurnipContext.TURNIP_BUY_PRICE;

            this.initComponents();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = (int)((Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2) *(Game1.options.zoomLevel / Game1.options.uiScale));
            this.yPositionOnScreen = (int)((Game1.viewport.Height / 3 * 2 - (250 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize) * (Game1.options.zoomLevel / Game1.options.uiScale));
            this.reBoundComponent();
        }

        private void initComponents()
        {
            int left = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
            int top = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
            int right = this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder;
            int bottom = this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder;

            //Portrait
            this.PortraitPosition = new Vector2(left, top - Game1.tileSize);

            // Question
            string question = ModResource.getTranslation("turnipshopmenu.question", TurnipContext.TURNIP_BUY_PRICE.ToString("###,###,###,###"));

            this.QuestionBox = new TextBox(TextTexture, null, Game1.smallFont, Game1.textColor)
            {
                X = left + (int)(1.5 * Game1.tileSize),
                Y = top - Game1.tileSize / 2,
                Height = 0,
                Width = (int)Game1.dialogueFont.MeasureString(question).X,
                Text = question
            };

            //CountBox
            {
                int maxWidth = (int)Game1.dialogueFont.MeasureString(MaxCount.ToString()).X;

                this.CountBox = new DigitBox(TextTexture, null, Game1.smallFont, Game1.textColor)
                {
                    X = (left + right - maxWidth) / 2,
                    Y = top + Game1.tileSize,
                    Height = 0,
                    Width = maxWidth,
                    Text = this.count.ToString(),
                    textLimit = 5,
                    digitsLimit = this.buyableCount
                };

                this.CountBoxBounds = new Rectangle(this.CountBox.X, this.CountBox.Y+8, this.CountBox.Width, 12 * Game1.pixelZoom);
                this.CountBoxArea = new ClickableComponent(new Rectangle(this.CountBoxBounds.X, this.CountBoxBounds.Y, this.CountBoxBounds.Width, 12 * Game1.pixelZoom), "");
            }

            // sum price box
            int sumPrice = TurnipContext.TURNIP_BUY_PRICE * this.count;

            this.SumPriceBox = new TextBox(TextTexture, null, Game1.smallFont, Game1.textColor)
            {
                X = left + (int)(1.5 * Game1.tileSize),
                Y = top + Game1.tileSize * 2,
                Height = 0,
                Width = this.width,
                Text = ModResource.getTranslation("turnipshopmenu.sumprice", sumPrice > 0 ? sumPrice.ToString("###,###,###,###") : "0")
        };

            // ok button
            this.OkButton = new ClickableTextureComponent("OK", new Rectangle(right - Game1.tileSize, bottom + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
        }

        private void reBoundComponent()
        {
            int left = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
            int top = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
            int right = this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder;
            int bottom = this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder;

            //Portrait
            this.PortraitPosition = new Vector2(left, top - Game1.tileSize);

            // Question
            {
                this.QuestionBox.X = left + (int)(1.5 * Game1.tileSize);
                this.QuestionBox.Y = top - Game1.tileSize / 2;
            };

            //CountBox
            {
                int maxWidth = (int)Game1.dialogueFont.MeasureString(MaxCount.ToString()).X;
                {
                    this.CountBox.X = (left + right - maxWidth) / 2;
                    this.CountBox.Y = top + Game1.tileSize;
                    this.CountBox.Height = 0;
                    this.CountBox.Width = maxWidth;
                    this.CountBox.Text = this.count.ToString();
                };

                this.CountBoxBounds = new Rectangle(this.CountBox.X, this.CountBox.Y+8, this.CountBox.Width, 12 * Game1.pixelZoom);
                this.CountBoxArea.bounds = new Rectangle(this.CountBoxBounds.X, this.CountBoxBounds.Y, this.CountBoxBounds.Width, 12 * Game1.pixelZoom);
            }

            // sum price box
            {
                this.SumPriceBox.X = left + (int)(1.5 * Game1.tileSize);
                this.SumPriceBox.Y = top + Game1.tileSize * 2;
                this.SumPriceBox.Height = 0;
                this.SumPriceBox.Width = this.width;
            };

            // ok button
            this.OkButton.bounds = new Rectangle(right - Game1.tileSize, bottom + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize);
        }

        public override void update(GameTime time)
        {
            if (this.count != this.CountBox.Digits)
            {
                this.UpdatePrice();
            }

            base.update(time);
        }

        public override void draw(SpriteBatch b)
        {

            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            b.Draw(ModResource.daisyMaeMiniPortraitsTexture, this.PortraitPosition, Color.White);

            // draw question
            QuestionBox.Draw(b);
            
            // draw CountBox
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), 
                this.CountBoxBounds.X, 
                this.CountBoxBounds.Y - ButtonBorderWidth, 
                this.CountBoxBounds.Width + ButtonBorderWidth * 2,
                this.CountBoxBounds.Height + ButtonBorderWidth,
                Color.White, drawShadow:false);
            this.CountBox.Draw(b);
            Utility.drawTextWithShadow(b, "무", Game1.smallFont, new Vector2(this.CountBoxBounds.X + this.CountBoxBounds.Width - 10, this.CountBox.Y+10), Game1.textColor);

            // draw sum price label
            this.SumPriceBox.Draw(b);

            this.OkButton.draw(b);

            this.drawMouse(b);
        }

        private void onClick(string name)
        {
            if (name == null)
                return;

            switch (name)
            {
                case "OK":
                    this.TryBuy();
                    break;

                case "CountBox":
                    if (!this.CountBox.Selected)
                        this.SelectCountBox(explicitly: true);
                    break;
            }
        }

        private void UpdatePrice()
        {
            // update price
            this.count = this.CountBox.Digits;

            // update sum price
            int sumPrice = TurnipContext.TURNIP_BUY_PRICE * this.count;
            this.SumPriceBox.Text = ModResource.getTranslation("turnipshopmenu.sumprice", sumPrice > 0 ? sumPrice.ToString("###,###,###,###") : "0");
        }

        private void SelectCountBox(bool explicitly)
        {
            this.CountBox.Selected = true;
            this.CountBox.Width = this.CountBoxBounds.Width;
        }

        private void TryBuy()
        {
            int sumPrice = TurnipContext.TURNIP_BUY_PRICE * this.count;

            // prevent zero count
            if (this.count <= 0)
                return;

            // check money
            if (this.who.Money < sumPrice)
                return;

            // check inventory

            // buy
            else
            {
                this.who.Money = this.who.Money - sumPrice;
                this.who.addItemByMenuIfNecessary(new SObject(ModEntry.wt_id, this.count));
                this.who.currentLocation.playSound("coin");

                exitThisMenu();
            }

        }

        public override void performHoverAction(int x, int y)
        {
            this.OkButton.scale = this.OkButton.containsPoint(x, y)
                ? Math.Min(this.OkButton.scale + 0.2f, this.OkButton.baseScale + 0.1f)
                : Math.Max(this.OkButton.scale - 0.2f, this.OkButton.baseScale);

            base.performHoverAction(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // ok button
            if (this.OkButton.containsPoint(x, y))
                onClick("OK");

            // countbox
            if (this.CountBoxBounds.Contains(x, y))
                onClick("CountBox");

            else
                base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
        }
    }
}
