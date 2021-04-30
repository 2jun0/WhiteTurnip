using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using WhiteTurnip.Frameworks;

namespace WhiteTurnip
{
    public class DaisyMaeManager
    {
        public static NPC DaisyMae;
        public static bool IsAtHome;
        public static NPC createNPC()
        {
            DaisyMae = new NPC();
            DaisyMae.Name = "DaisyMae";

            DaisyMae.Age = NPC.child;
            DaisyMae.Manners = NPC.polite;
            DaisyMae.SocialAnxiety = NPC.outgoing;
            DaisyMae.Optimism = NPC.positive;
            DaisyMae.Gender = NPC.female;
            DaisyMae.homeRegion = NPC.other;
            DaisyMae.Birthday_Day = 5;
            DaisyMae.Birthday_Season = "summer";

            DaisyMae.DefaultMap = "BusStop";
            DaisyMae.DefaultPosition = new Vector2(22, 5) * Game1.tileSize;

            DaisyMae.DefaultFacingDirection = 2;
            DaisyMae.Schedule = DaisyMae.parseMasterSchedule("610 BusStop 22 5 2/800 Town 30 57 2/1200 BusStop -1 8 0");

            DaisyMae.Sprite = new AnimatedSprite(ModResource.daisyMaeTexturePath, 0, 16, 32);
            DaisyMae.Portrait = ModResource.daisyMaePortraitTexture;

            DaisyMae.displayName = ModResource.getTranslation("daisymae.name");
            IsAtHome = true;

            return DaisyMae;
        } 

        public static void Update()
        {
            if (Game1.stats.DaysPlayed % 7 != 0)
            {
                GoHome();
                return;
            }
            
            DaisyMae.currentLocation = Game1.getLocationFromName(DaisyMae.DefaultMap);
            DaisyMae.Position = DaisyMae.DefaultPosition;
            Game1.getLocationFromName("BusStop").addCharacter(DaisyMae);
            IsAtHome = false;
        }

        public static void GoHome()
        {
            Game1.getLocationFromName("BusStop").characters.Remove(DaisyMae);
            IsAtHome = true;
        }

        public static void TickUpdate()
        {
            if (!IsAtHome && DaisyMae.currentLocation.name == "Backwoods" && DaisyMae.Position.Y < 23 * Game1.tileSize)
                GoHome();
        }

        public static void DisplayDialogue(Farmer who)
        {
            who.Halt();
            who.faceGeneralDirection(DaisyMae.getStandingPosition(), 0, false, false);

            Greeting(who);
        }

        private static void Greeting(Farmer who)
        {
            if (who.modData.ContainsKey(ModEntry.DAISYMAE_INTRO_KEY) && who.modData[ModEntry.DAISYMAE_INTRO_KEY].ToLower() == "true")
            {
                DaisyMae.CurrentDialogue.Push(new Dialogue(GetDialogue("daisymae.spring1"), DaisyMae));
                Game1.drawDialogue(DaisyMae);
                AskQuestionAfterGreeting();
            }
            else
            {
                who.modData[ModEntry.DAISYMAE_INTRO_KEY] = "true";

                Game1.player.friendshipData.Add(DaisyMae.name, new Friendship(1));
                DaisyMae.CurrentDialogue.Push(new Dialogue(GetDialogue("daisymae.intro"), DaisyMae));
                Game1.drawDialogue(DaisyMae);
                AskQuestionAfterIntro();
            }
        }

        private static void Explain(int level)
        {
            switch (level)
            {
                case 1:
                    DaisyMae.CurrentDialogue.Push(new Dialogue(GetDialogue("daisymae.explain_turnip1"), DaisyMae));
                    Game1.drawDialogue(DaisyMae);
                    AskQuestionAfterExplain(level);
                    break;
                case 2:
                    DaisyMae.CurrentDialogue.Push(new Dialogue(GetDialogue("daisymae.explain_turnip2"), DaisyMae));
                    Game1.drawDialogue(DaisyMae);
                    AskQuestionAfterExplain(level);
                    break;
                case 3:
                    DaisyMae.CurrentDialogue.Push(new Dialogue(GetDialogue("daisymae.explain_turnip3"), DaisyMae));
                    Game1.drawDialogue(DaisyMae);
                    AskQuestionAfterExplain(level);
                    break;
            }
        }

        private static void AskQuestionAfterIntro()
        {
            Game1.afterDialogues = delegate ()
            {
                List<Response> responses = new List<Response>();
                responses.Add(new Response("explain1", GetTranslation("daisymae.intro_a1")));
                responses.Add(new Response("nothing", GetTranslation("daisymae.intro_a2")));
                Game1.currentLocation.createQuestionDialogue(GetDialogue("daisymae.intro_q"), responses.ToArray(), OnPlayerResponse, DaisyMae);
            };
        }

        private static void AskQuestionAfterGreeting()
        {
            Game1.afterDialogues = delegate ()
            {
                List<Response> responses = new List<Response>();
                responses.Add(new Response("showShopMenu", GetTranslation("daisymae.spring_a1", TurnipPrice.TURNIP_BUY_PRICE)));
                responses.Add(new Response("explain1", GetTranslation("daisymae.spring_a2")));
                responses.Add(new Response("nothing", GetTranslation("daisymae.spring_a3")));
                Game1.currentLocation.createQuestionDialogue(GetDialogue("daisymae.spring1_q"), responses.ToArray(), OnPlayerResponse, DaisyMae);
            };
        }

        private static void AskQuestionAfterExplain(int level)
        {
            Game1.afterDialogues = delegate ()
            {
                List<Response> responses = new List<Response>();
                switch (level)
                {
                    case 1:
                        responses.Add(new Response("explain2", GetTranslation("daisymae.explain_turnip1_a1")));
                        responses.Add(new Response("nothing", GetTranslation("daisymae.explain_turnip1_a2")));
                        Game1.currentLocation.createQuestionDialogue(GetDialogue("daisymae.explain_turnip1_q"), responses.ToArray(), OnPlayerResponse, DaisyMae);
                        break;
                    case 2:
                        responses.Add(new Response("explain3", GetTranslation("daisymae.explain_turnip2_a1")));
                        responses.Add(new Response("nothing", GetTranslation("daisymae.explain_turnip2_a2")));
                        Game1.currentLocation.createQuestionDialogue(GetDialogue("daisymae.explain_turnip2_q"), responses.ToArray(), OnPlayerResponse, DaisyMae);
                        break;
                    case 3:
                        responses.Add(new Response("showShopMenu", GetTranslation("daisymae.explain_turnip3_a1")));
                        responses.Add(new Response("nothing", GetTranslation("daisymae.explain_turnip3_a2")));
                        responses.Add(new Response("explain1", GetTranslation("daisymae.explain_turnip3_a3")));
                        Game1.currentLocation.createQuestionDialogue(GetDialogue("daisymae.explain_turnip3_q", TurnipPrice.TURNIP_BUY_PRICE), responses.ToArray(), OnPlayerResponse, DaisyMae);
                        break;
                }
            };
        }
        private static void ShowTurnipShopMenu(Farmer who)
        {
            Game1.activeClickableMenu = new TurnipShopMenu(who, 
                delegate (int count) {
                    DaisyMae.CurrentDialogue.Push(new Dialogue(GetDialogue("daisymae.thank"), DaisyMae));
                    Game1.drawDialogue(DaisyMae);

                    // Add Friendship data
                    if (who.friendshipData.ContainsKey(DaisyMae.name))
                        who.friendshipData[DaisyMae.name].Points += count / 10;
                    else
                        who.friendshipData.Add(DaisyMae.name, new Friendship(count / 10));
                },
                null);
        }

        private static void OnPlayerResponse(Farmer who, string answer)
        {
            switch(answer)
            {
                case "explain1":
                    Explain(1);
                    break;
                case "explain2":
                    Explain(2);
                    break;
                case "explain3":
                    Explain(3);
                    break;
                case "showShopMenu":
                    ShowTurnipShopMenu(who);
                    break;
                case "nothing":
                    Game1.activeClickableMenu = null;
                    who.canMove = true;
                    break;
            }
        }

        private static string GetDialogue(string key, object formatValue = null)
        {
            return string.Format(ModResource.getTranslation(key), formatValue);
        }

        private static string GetTranslation(string key, object formatValue = null)
        {
            return string.Format(ModResource.getTranslation(key), formatValue);
        }
    }
}
