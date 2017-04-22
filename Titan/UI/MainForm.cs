﻿using System;
using System.IO;
using Eto.Drawing;
using Eto.Forms;
using Serilog.Core;
using Titan.Bot;
using Titan.Bot.Mode;
using Titan.Logging;
using Titan.UI.Commands.Links;

namespace Titan.UI
{
    public sealed class MainForm : Form
    {

        private readonly string _icon = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Resources" +
                                        Path.DirectorySeparatorChar + "Logo.ico";

        private Logger _log = LogCreator.Create();

        private readonly DropDown _dropDown;
        private readonly TextBox _targetBox;
        private readonly TextBox _matchIDBox;
        private readonly Label _matchIDLabel;

        public MainForm()
        {
            Title = "Titan";
            ClientSize = new Size(600, 160);
            Resizable = false;
            Icon = new Icon(File.Open(_icon, FileMode.Open));

            // Selected arguments for the UI
            _targetBox = new TextBox { PlaceholderText = "STEAM_0:0:131983088" };
            _matchIDBox = new TextBox { PlaceholderText = "3203363151840018511" };
            _matchIDLabel = new Label { Text = "Match ID" };

            _dropDown = new DropDown { Items = { "Report", "Commend" }, SelectedIndex = 0 };
            _dropDown.SelectedIndexChanged += OnDropDownIndexChange;

            var bombBtn = new Button { Text = "Bomb!" };
            bombBtn.Click += OnBombButtonClick;

            Content = new TableLayout
            {
                Spacing = new Size(5, 5),
                Padding = new Padding(10, 10, 10, 10),
                Rows =
                {
                    new TableRow(
                        new TableCell(new Label { Text = "Mode" }, true),
                        new TableCell(_dropDown, true)
                    ),
                    new TableRow(
                        new Label { Text = "Target" },
                        _targetBox
                    ),
                    new TableRow(
                        _matchIDLabel,
                        _matchIDBox
                    ),
                    new TableRow(new TableCell(), new TableCell()),
                    new TableRow(new TableCell(), new TableCell()),
                    new TableRow(new TableCell(), new TableCell()),
                    new TableRow(
                        new TableCell(),
                        bombBtn
                    ),
                    new TableRow { ScaleHeight = true }
                }
            };

            Menu = new MenuBar
            {
                Items =
                {
                    new ButtonMenuItem { Text = "&Links", Items = {
                        new SteamIO(),
                        new JsonValidator(),
                        new SharecodeFinder()
                    }}
                },
                AboutItem = new Commands.About(),
                QuitItem = new Commands.Quit()
            };

        }

        public void OnBombButtonClick(object sender, EventArgs args)
        {
            var mode = ModeParser.Parse(_dropDown.SelectedIndex);

            if(!string.IsNullOrWhiteSpace(_targetBox.Text) || (!string.IsNullOrEmpty(_matchIDBox.Text) && mode != BotMode.Commend))
            {
                _log.Information("Bomb! Button has been pressed. Starting bombing to {0} in match {1}.", _targetBox.Text, _matchIDBox.Text);

                Hub.StartBotting(_targetBox.Text, _matchIDBox.Text, mode);
            }
            else
            {
                MessageBox.Show(
                    mode == BotMode.Commend
                        ? "Please provide the Target."
                        : "Please provide the Target and the Match ID.", "Error - Titan",
                    MessageBoxType.Error);
            }
        }

        public void OnDropDownIndexChange(object sender, EventArgs args)
        {
            if(_dropDown.SelectedIndex == 0)
            {
                _matchIDLabel.Visible = true;
                _matchIDBox.Visible = true;
            }
            else if(_dropDown.SelectedIndex == 1)
            {
                _matchIDLabel.Visible = false;
                _matchIDBox.Visible = false;
            }
        }

    }
}