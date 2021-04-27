using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace WhiteTurnip.Framework
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
        
        // ok button
        private ClickableTextureComponent OkButton;

        // portrait 관련
        private Vector2 PortraitPosition;
        private readonly Texture2D portrait;

        // 상수
        public const int ButtonBorderWidth = 4 * Game1.pixelZoom;
        public const int MaxCount = 100000;
        public readonly Texture2D TextTexture;

        public readonly Action<int> OnBuy;
        public readonly Action<string> OnFail;

        public TurnipShopMenu(Action<int> onBuy, Action<string> onFail)
            :base(
                Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2,
                Game1.viewport.Height / 3 * 2 - (250 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize,
                632 + IClickableMenu.borderWidth * 2,
                250 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            this.resetComponents();
            this.portrait = ModEntry.instance.Helper.Content.Load<Texture2D>("assets/img/DaisyMae.png", ContentSource.ModFolder);
            this.TextTexture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");

            this.OnBuy = onBuy;
            this.OnFail = onFail;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 3 * 2 - (250 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
            this.resetComponents();
        }

        private void resetComponents()
        {
            int left = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
            int top = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
            int right = this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder;
            int bottom = this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder;

            //Portrait
            this.PortraitPosition = new Vector2(left, top - Game1.tileSize);

            // Label
            string question = "1무에 " + TurnipPrice.TURNIP_BUY_PRICE + "원인데 얼마나 사실래?";

            this.QuestionBox = new TextBox(TextTexture, null, Game1.smallFont, Game1.textColor)
            {
                X = left + (int)(1.5 * Game1.tileSize),
                Y = top - Game1.tileSize/2,
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
                    Y = top + Game1.tileSize / 2,
                    Height = 0,
                    Width = maxWidth,
                    Text = this.count.ToString(),
                    textLimit = 5
                };

                this.CountBoxBounds = new Rectangle(this.CountBox.X, this.CountBox.Y + 4, this.CountBox.Width, 12 * Game1.pixelZoom);
                this.CountBoxArea = new ClickableComponent(new Rectangle(this.CountBoxBounds.X, this.CountBoxBounds.Y, this.CountBoxBounds.Width, 12 * Game1.pixelZoom), "");
            }

            // sum price box
            string sumPrice = "합계: " + TurnipPrice.TURNIP_BUY_PRICE * this.count + "원";

            this.SumPriceBox = new TextBox(TextTexture, null, Game1.smallFont, Game1.textColor)
            {
                X = left + (int)(1.5 * Game1.tileSize),
                Y = top + Game1.tileSize * 3 / 2,
                Height = 0,
                Width = this.width,
                Text = sumPrice
            };

            // ok button
            this.OkButton = new ClickableTextureComponent("OK", new Rectangle(right - Game1.tileSize, bottom, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
        }

        public override void update(GameTime time)
        {
            if (this.count != this.CountBox.Digits)
            {
                this.count = this.CountBox.Digits;
                this.UpdateSumPrice();
            }

            base.update(time);
        }

        public override void draw(SpriteBatch b)
        {

            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            b.Draw(this.portrait, this.PortraitPosition, Color.White);

            // draw question
            QuestionBox.Draw(b);
            
            // draw CountBox
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), 
                this.CountBoxBounds.X, 
                this.CountBoxBounds.Y - ButtonBorderWidth / 2, 
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
                    exitThisMenu();
                    break;

                case "CountBox":
                    if (!this.CountBox.Selected)
                        this.SelectCountBox(explicitly: true);
                    break;
            }
        }

        private void UpdateSumPrice()
        {
            string sumPrice = "합계: " + (TurnipPrice.TURNIP_BUY_PRICE * this.count).ToString("###,###,###,###") + "원";
            this.SumPriceBox.Text = sumPrice;
        }

        private void SelectCountBox(bool explicitly)
        {
            this.CountBox.Selected = true;
            //this.IsSearchBoxSelectedExplicitly = explicitly;
            this.CountBox.Width = this.CountBoxBounds.Width;
        }

        private void TryBuy()
        {
            int sumPrice = TurnipPrice.TURNIP_BUY_PRICE * this.count;

            //// check money
            //if (Game1.player.Money < sumPrice)
            //    if (this.OnFail != null)
            //        this.OnFail("Not enough money");

            //// check inventory

            //// buy
            //else
            //{
            //    Game1.player.Money = Game1.player.Money - sumPrice;
            //}
            
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
            if (this.CountBox.Selected)
            {

            }

            base.receiveKeyPress(key);
        }
    }
}
