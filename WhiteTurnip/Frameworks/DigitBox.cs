using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace WhiteTurnip.Frameworks
{
    internal class DigitBox : TextBox
    {
        public int _digits = 0;
        public int digitsLimit;
        public int Digits { get => _digits; set => setDigits(value); }
        public DigitBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor)
            : base(textBoxTexture, caretTexture, font, textColor)
        {
            
        }

        public override void RecieveTextInput(char inputChar)
        {
            if (int.TryParse(inputChar.ToString(), out _) && !(inputChar == '0' && this.Text.Trim() == ""))
            {
                int preDigits = this._digits;
                base.RecieveTextInput(inputChar);
                this._digits = getDigits();

                if (this._digits > this.digitsLimit)
                    this.Digits = preDigits;
            }
        }

        public override void RecieveTextInput(string text)
        {
            if (int.TryParse(text, out _) && !(text == "0" && this.Text.Trim() == ""))
            {
                int preDigits = this._digits;
                base.RecieveTextInput(text);
                this._digits = getDigits();

                if (this._digits > this.digitsLimit)
                    this.Digits = preDigits;
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
            this._digits = digits;
            this.Text = digits.ToString();
        }
    }
}
