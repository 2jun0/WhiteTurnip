using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace WhiteTurnip.Framework
{
    internal class DigitBox : TextBox
    {
        public int Digits { get; set; }
        public DigitBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor)
            : base(textBoxTexture, caretTexture, font, textColor)
        {
            
        }

        public override void RecieveTextInput(char inputChar)
        {
            if (int.TryParse(inputChar.ToString(), out _) && !(inputChar == '0' && this.Text.Trim() == ""))
            {
                base.RecieveTextInput(inputChar);
                this.Digits = getDigits();
            }
        }

        public override void RecieveTextInput(string text)
        {
            if (int.TryParse(text, out _) && !(text == "0" && this.Text.Trim() == ""))
            {
                base.RecieveTextInput(text);
                this.Digits = getDigits();
            }
        }

        private int getDigits()
        {
            if (this.Text.Trim() == "")
                return 0;
            else
                return int.Parse(this.Text.Trim());
        }

        private void setDigits(int digits)
        {
            this.Text = digits.ToString();
        }
    }
}
