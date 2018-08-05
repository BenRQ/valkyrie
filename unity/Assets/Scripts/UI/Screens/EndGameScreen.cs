using Assets.Scripts.Content;
﻿using Assets.Scripts.UI.Screens;
using System.Collections.Generic;
using UnityEngine;
using ValkyrieTools;

namespace Assets.Scripts.UI.Screens
{

    // Class for creation and management of the main menu
    public class EndGameScreen
    {
        private static readonly string IMG_BG_INVESTIGATORS_PHASE = "ImageGreenBG";
        private StringKey STATS_WELCOME         = new StringKey("val", "STATS_WELCOME");
        private StringKey STATS_ASK_VICTORY     = new StringKey("val", "STATS_ASK_VICTORY");
        private StringKey STATS_ASK_VICTORY_YES = new StringKey("val", "STATS_ASK_VICTORY_YES");
        private StringKey STATS_ASK_VICTORY_NO  = new StringKey("val", "STATS_ASK_VICTORY_NO");
        private StringKey STATS_ASK_RATING      = new StringKey("val", "STATS_ASK_RATING");
        private StringKey STATS_ASK_COMMENTS    = new StringKey("val", "STATS_ASK_COMMENTS");
        private StringKey STATS_SEND_BUTTON     = new StringKey("val", "STATS_SEND_BUTTON");
        private StringKey STATS_MENU_BUTTON     = new StringKey("val", "STATS_MENU_BUTTON");
        private float TextWidth = 28;
        private float ButtonWidth = 4;
        private UIElement button_yes = null;
        private UIElement button_no  = null;
        private bool game_won=false;

        // Create a menu which will take up the whole screen and have options.  All items are dialog for destruction.
        public EndGameScreen()
        {
            Game game = Game.Get();

            // Investigator picture in background full screen
            UIElement bg = new UIElement(Game.ENDGAME);
            Texture2D bgTex;
            bgTex = ContentData.FileToTexture(game.cd.images[IMG_BG_INVESTIGATORS_PHASE].image);
            bg.SetImage(bgTex);
            bg.SetLocation(0, 0, UIScaler.GetWidthUnits(), UIScaler.GetHeightUnits());
            
            // Banner, to show we are out of the scenario
          /*  UIElement ui = new UIElement(Game.ENDGAME);
            ui.SetLocation(2, 1, UIScaler.GetWidthUnits() - 4, 3);
            ui.SetText("Valkyrie");
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetLargeFont());
            */

            // Welcome text
            UIElement ui = new UIElement(Game.ENDGAME);
            ui.SetLocation((UIScaler.GetWidthUnits() - TextWidth) / 2, 1, TextWidth, 4);
            ui.SetText(STATS_WELCOME);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSmallFont());
            ui.SetButton(SendStats);
            ui.SetBGColor(new Color(0, 0.03f, 0f));
            new UIElementBorder(ui);

            int offset = 5;

            // First question : player has won ?
            ui = new UIElement(Game.ENDGAME);
            ui.SetLocation(2, offset + 2, TextWidth, 2);
            ui.SetText(STATS_ASK_VICTORY);
            ui.SetTextAlignment(TextAnchor.MiddleLeft);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSmallFont());
            ui.SetBGColor(new Color(0, 0.03f, 0f, 0.2f));

            // yes button
            button_yes = new UIElement(Game.ENDGAME);
            button_yes.SetLocation(((UIScaler.GetWidthUnits() - ButtonWidth) / 10) * 6, offset + 2.5f, ButtonWidth, 1);
            button_yes.SetText(STATS_ASK_VICTORY_YES);
            button_yes.SetFont(game.gameType.GetHeaderFont());
            button_yes.SetFontSize(UIScaler.GetSmallFont());
            button_yes.SetButton(PressVictoryYes);
            button_yes.SetBGColor(new Color(0, 0.03f, 0f));
            new UIElementBorder(button_yes);

            // no button
            button_no = new UIElement(Game.ENDGAME);
            button_no.SetLocation(((UIScaler.GetWidthUnits() - ButtonWidth) / 10) * 8, offset + 2.5f, ButtonWidth, 1);
            button_no.SetText(STATS_ASK_VICTORY_NO);
            button_no.SetFont(game.gameType.GetHeaderFont());
            button_no.SetFontSize(UIScaler.GetSmallFont());
            button_no.SetButton(PressVictoryNo);
            button_no.SetBGColor(new Color(0, 0.03f, 0f));
            new UIElementBorder(button_no);


            offset += 4;

            // Second question : rating ?
            ui = new UIElement(Game.ENDGAME);
            // Draw text
            ui.SetLocation(2, offset + 2, TextWidth, 2);
            ui.SetText(STATS_ASK_RATING);
            ui.SetTextAlignment(TextAnchor.MiddleLeft);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSmallFont());
            ui.SetBGColor(new Color(0, 0.03f, 0f, 0.2f));


            offset += 4;

            /*                        // Third question : comments ?
                        new UIElementBorder(ui);
                        ui.SetLocation(2, offset + 2, TextWidth, 2);
                        ui.SetText(STATS_ASK_COMMENTS);
                        ui.SetFont(game.gameType.GetHeaderFont());
                        ui.SetFontSize(UIScaler.GetMediumFont());
                        ui.SetBGColor(new Color(0, 0.03f, 0f));
                        new UIElementBorder(ui);

                        offset += 4;

                        // Go back to menu button
                        ui = new UIElement(Game.ENDGAME);
                        ui.SetLocation(((UIScaler.GetWidthUnits() - ButtonWidth) / 5) * 2, offset, ButtonWidth, 2);
                        ui.SetText(STATS_MENU_BUTTON);
                        ui.SetFont(game.gameType.GetHeaderFont());
                        ui.SetFontSize(UIScaler.GetMediumFont());
                        ui.SetButton(MainMenu);
                        ui.SetBGColor(new Color(0, 0.03f, 0f));
                        new UIElementBorder(ui);

                        // Publish button
                        ui = new UIElement(Game.ENDGAME);
                        ui.SetLocation(((UIScaler.GetWidthUnits() - ButtonWidth) / 5) * 4 , offset, ButtonWidth, 2);
                        ui.SetText(STATS_SEND_BUTTON);
                        ui.SetFont(game.gameType.GetHeaderFont());
                        ui.SetFontSize(UIScaler.GetMediumFont());
                        ui.SetButton(SendStats);
                        ui.SetBGColor(new Color(0, 0.03f, 0f));
                        new UIElementBorder(ui);
                        */
        }

        // Send data to Google forms
        private void SendStats()
        {
            StatsManager stats = new StatsManager();
            stats.PrepareStats(game_won, 8, "my comments");
            stats.PublishData();

            // todo: manage the result / error with a callback
            Destroyer.MainMenu();
        }

        private void MainMenu()
        {
            Destroyer.MainMenu();
        }


        private void PressVictoryYes()
        {
            button_yes.SetBGColor(new Color(0.8f, 0.8f, 0.8f));
            button_yes.SetText(STATS_ASK_VICTORY_YES, Color.black);
            game_won = true;

            button_no.SetBGColor(new Color(0, 0.03f, 0f));
            button_no.SetText(STATS_ASK_VICTORY_NO, Color.white);
        }

        private void PressVictoryNo()
        {
            button_no.SetBGColor(new Color(0.9f, 0.9f, 0.9f));
            button_no.SetText(STATS_ASK_VICTORY_NO, Color.black);
            game_won = false;

            button_yes.SetBGColor(new Color(0, 0.03f, 0f));
            button_yes.SetText(STATS_ASK_VICTORY_YES,Color.white);
        }

    }
}
