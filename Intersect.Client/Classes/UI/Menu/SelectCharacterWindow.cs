﻿using System;
using System.Collections.Generic;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Client.UI.Game;
using Intersect.Client.UI.Game.Chat;

namespace Intersect.Client.UI.Menu
{
    public class SelectCharacterWindow
    {
        private ImagePanel mCharacterContainer;
        private ImagePanel mCharacterPortrait;

        //Image
        private string mCharacterPortraitImg = "";

        private Label mCharacterSelectionHeader;
        private ImagePanel mCharacterSelectionPanel;
        private Label mCharnameLabel;
        private Button mDeleteButton;
        private Label mInfoLabel;

        //Parent
        private MainMenu mMainMenu;

        private Button mNewButton;

        //Controls
        private Button mNextCharButton;

        private ImagePanel[] mPaperdollPortraits;
        private Button mPlayButton;
        private Button mPrevCharButton;
        public List<Character> Characters = new List<Character>();
        private Button mLogoutButton;

        //Selected Char
        private int mSelectedChar = 0;

        public bool IsHidden => mCharacterSelectionPanel.IsHidden;

        //Init
        public SelectCharacterWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            mMainMenu = mainMenu;

            //Main Menu Window
            mCharacterSelectionPanel = new ImagePanel(parent, "CharacterSelectionWindow");

            //Menu Header
            mCharacterSelectionHeader = new Label(mCharacterSelectionPanel, "CharacterSelectionHeader");
            mCharacterSelectionHeader.SetText(Strings.CharacterSelection.title);

            //Character Name
            mCharnameLabel = new Label(mCharacterSelectionPanel, "CharacterNameLabel");
            mCharnameLabel.SetText(Strings.CharacterSelection.empty);

            //Character Info
            mInfoLabel = new Label(mCharacterSelectionPanel, "CharacterInfoLabel");
            mInfoLabel.SetText(Strings.CharacterSelection.New);

            //Character Container
            mCharacterContainer = new ImagePanel(mCharacterSelectionPanel, "CharacterContainer");

            //Next char Button
            mNextCharButton = new Button(mCharacterContainer, "NextCharacterButton");
            mNextCharButton.Clicked += _nextCharButton_Clicked;

            //Prev Char Button
            mPrevCharButton = new Button(mCharacterContainer, "PreviousCharacterButton");
            mPrevCharButton.Clicked += _prevCharButton_Clicked;

            //Play Button
            mPlayButton = new Button(mCharacterSelectionPanel, "PlayButton");
            mPlayButton.SetText(Strings.CharacterSelection.play);
            mPlayButton.Clicked += _playButton_Clicked;
            mPlayButton.Hide();

            //Delete Button
            mDeleteButton = new Button(mCharacterSelectionPanel, "DeleteButton");
            mDeleteButton.SetText(Strings.CharacterSelection.delete);
            mDeleteButton.Clicked += _deleteButton_Clicked;
            mDeleteButton.Hide();

            //Create new char Button
            mNewButton = new Button(mCharacterSelectionPanel, "NewButton");
            mNewButton.SetText(Strings.CharacterSelection.New);
            mNewButton.Clicked += _newButton_Clicked;

            //Logout Button
            mLogoutButton = new Button(mCharacterSelectionPanel,"LogoutButton");
            mLogoutButton.SetText(Strings.CharacterSelection.logout);
            mLogoutButton.IsHidden = true;
            mLogoutButton.Clicked += mLogoutButton_Clicked;

            mCharacterSelectionPanel.LoadJsonUi(GameContentManager.UI.Menu, GameGraphics.Renderer.GetResolutionString());
        }


        //Methods
        public void Update()
        {
            if (!GameNetwork.Connected)
            {
                Hide();
                mMainMenu.Show();
                Gui.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.lostconnection));
            }
        }

        private void UpdateDisplay()
        {
            var isFace = true;

            //Show and hide Options based on the character count
            if (Characters.Count > 1)
            {
                mNextCharButton.Show();
                mPrevCharButton.Show();
            }

            if (Characters.Count <= 1)
            {
                mNextCharButton.Hide();
                mPrevCharButton.Hide();
            }

            if (mPaperdollPortraits == null)
            {
                mPaperdollPortraits = new ImagePanel[Options.EquipmentSlots.Count];
                mCharacterPortrait = new ImagePanel(mCharacterContainer);
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    mPaperdollPortraits[i] = new ImagePanel(mCharacterContainer);
                }
                mNextCharButton.BringToFront();
                mPrevCharButton.BringToFront();
            }
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                mPaperdollPortraits[i].Hide();
            }

            if (Characters[mSelectedChar] != null)
            {
                mCharnameLabel.SetText(Strings.CharacterSelection.name.ToString(Characters[mSelectedChar].Name));
                mInfoLabel.SetText(Strings.CharacterSelection.info.ToString(Characters[mSelectedChar].Level,  Characters[mSelectedChar].Class));
                mPlayButton.Show();
                mDeleteButton.Show();
                mNewButton.Hide();

                for (int i = 0; i < Options.EquipmentSlots.Count + 1; i++)
                {
                    if (Characters[mSelectedChar].Equipment[i] == "Player")
                    {
                        mCharacterPortrait = mPaperdollPortraits[i];
                    }
                }

                if (mCharacterPortrait == null)
                {
                    mCharacterPortrait = mPaperdollPortraits[0];
                }

                mCharacterPortrait.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, Characters[mSelectedChar].Face);
                if (mCharacterPortrait.Texture == null)
                {
                    mCharacterPortrait.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, Characters[mSelectedChar].Sprite);
                    isFace = false;
                }

                if (mCharacterPortrait.Texture != null)
                {
                    if (isFace)
                    {
                        mCharacterPortrait.SetTextureRect(0, 0, mCharacterPortrait.Texture.GetWidth(), mCharacterPortrait.Texture.GetHeight());
                        mCharacterPortrait.SetSize(mCharacterContainer.Width - mCharacterContainer.Padding.Left - mCharacterContainer.Padding.Right,mCharacterContainer.Height - mCharacterContainer.Padding.Top - mCharacterContainer.Padding.Bottom);
                        mCharacterPortrait.SetPosition(mCharacterContainer.Padding.Left,mCharacterContainer.Padding.Top);
                        mCharacterPortrait.Show();
                    }
                    else
                    {
                        mCharacterPortrait.SetTextureRect(0, 0, mCharacterPortrait.Texture.GetWidth() / 4,
                            mCharacterPortrait.Texture.GetHeight() / 4);
                        mCharacterPortrait.SetSize(mCharacterPortrait.Texture.GetWidth() / 4,
                            mCharacterPortrait.Texture.GetHeight() / 4);
                        mCharacterPortrait.SetPosition(mCharacterContainer.Width / 2 - mCharacterPortrait.Width / 2,
                            mCharacterContainer.Height / 2 - mCharacterPortrait.Height / 2);

                        for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                        {
                            if (mPaperdollPortraits[i] != mCharacterPortrait)
                            {
                                mPaperdollPortraits[i].Texture =
                                    Globals.ContentManager.GetTexture(GameContentManager.TextureType.Paperdoll,
                                        Characters[mSelectedChar].Equipment[i]);
                                if (mPaperdollPortraits[i].Texture != null)
                                {
                                    mPaperdollPortraits[i].Show();
                                    mPaperdollPortraits[i].SetTextureRect(0, 0,
                                        mPaperdollPortraits[i].Texture.GetWidth() / 4,
                                        mPaperdollPortraits[i].Texture.GetHeight() / 4);
                                    mPaperdollPortraits[i].SetSize(mPaperdollPortraits[i].Texture.GetWidth() / 4,
                                        mPaperdollPortraits[i].Texture.GetHeight() / 4);
                                    mPaperdollPortraits[i].SetPosition(
                                        mCharacterContainer.Width / 2 - mPaperdollPortraits[i].Width / 2,
                                        mCharacterContainer.Height / 2 - mPaperdollPortraits[i].Height / 2);
                                }
                            }
                            else
                            {
                                if (mPaperdollPortraits[i].Texture != null)
                                    mPaperdollPortraits[i].Show();
                            }
                        }
                    }
                }
            }
            else
            {
                mPlayButton.Hide();
                mDeleteButton.Hide();
                mNewButton.Show();

                mCharnameLabel.SetText(Strings.CharacterSelection.empty);
                mInfoLabel.SetText("");

                mCharacterPortrait.Texture = null;
            }
        }

        public void Show()
        {
            mSelectedChar = 0;
            UpdateDisplay();
            mCharacterSelectionPanel.Show();
        }

        public void Hide()
        {
            mCharacterSelectionPanel.Hide();
        }

        private void mLogoutButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mMainMenu.Reset();
        }

        private void _prevCharButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mSelectedChar--;
            if (mSelectedChar < 0)
            {
                mSelectedChar = Characters.Count - 1;
            }
            UpdateDisplay();
        }

        private void _nextCharButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mSelectedChar++;
            if (mSelectedChar >= Characters.Count)
            {
                mSelectedChar = 0;
            }
            UpdateDisplay();
        }

        private void _playButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ChatboxMsg.ClearMessages();
            Globals.PlayPressed();
            PacketSender.SendSelectCharacter(Characters[mSelectedChar].Id);
        }

        private void _deleteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            InputBox iBox =
                new InputBox(Strings.CharacterSelection.deletetitle.ToString( Characters[mSelectedChar].Name),
                    Strings.CharacterSelection.deleteprompt.ToString( Characters[mSelectedChar].Name),
                    true, InputBox.InputType.YesNo, DeleteCharacter, null, Characters[mSelectedChar].Id,
                    mCharacterSelectionPanel.Parent, GameContentManager.UI.Menu);
        }

        private void DeleteCharacter(Object sender, EventArgs e)
        {
            PacketSender.SendDeleteCharacter((Guid)(((InputBox) sender).UserData));
            mSelectedChar = 0;
            UpdateDisplay();
        }

        private void _newButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendNewCharacter();
        }
    }

    public class Character
    {
        public string Class = "";
        public string[] Equipment = new string[Options.EquipmentSlots.Count + 1];
        public bool Exists = false;
        public string Face = "";
        public Guid Id;
        public int Level = 1;
        public string Name = "";
        public string Sprite = "";

        public Character(Guid id)
        {
            Id = id;
        }

        public Character(Guid id, string name, string sprite, string face, int level, string charClass, string[] equipment)
        {
            Equipment = equipment;
            Id = id;
            Name = name;
            Sprite = sprite;
            Face = face;
            Level = level;
            Class = charClass;
            Exists = true;
        }

        public Character()
        {
            for (int i = 0; i < Options.EquipmentSlots.Count + 1; i++)
            {
                Equipment[i] = "";
            }
            Equipment[0] = "Player";
        }
    }
}