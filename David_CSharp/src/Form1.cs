using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private readonly Game _game;
        private readonly GameDrawer _gameDrawer;

        public Form1()
        {
            InitializeComponent();

            this.Text = "Minesweeper";
            this.Size = new Size(1000, 1000);
            this.BackColor = Color.White;

            var solveButton = new Button
            {
                Text = "Solve",
            };

            solveButton.Click += SolveButton_Click;
            this.Controls.Add(solveButton);

            var gameLoader = new LoadGameFromFile();
            _game = gameLoader.Load("ComplexGame.txt").GetAwaiter().GetResult();
            _gameDrawer = new GameDrawer(_game);
            this.Paint += Form1_Paint;
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            using var g = this.CreateGraphics();
            g.Clear(this.BackColor);
            _gameDrawer.Draw(g);
            var result = _game.Solve();

            if (result is null)
            {
                MessageBox.Show("No more solutions found");
            }
            else
            {
                _gameDrawer.DrawMove(g, result);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            _gameDrawer.Draw(e.Graphics);
        }
    }
}
