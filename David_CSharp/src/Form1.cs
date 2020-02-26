using System;
using System.Drawing;
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

            var bruteForceButton = new Button
            {
                Text = "Brute Force",
                Location = new Point(100, 0),
            };

            bruteForceButton.Click += BruteForceButton_Click;
            this.Controls.Add(bruteForceButton);

            var calculateButton = new Button
            {
                Text = "Calculate",
                Location = new Point(200, 0),
                Width = 200,
            };

            calculateButton.Click += CalculateButton_Click;
            this.Controls.Add(calculateButton);

            var gameLoader = new LoadGameFromFile();
            _game = gameLoader.Load("ComplexGame.txt").GetAwaiter().GetResult();
            _gameDrawer = new GameDrawer(_game);
            this.Paint += Form1_Paint;
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            using var g = this.CreateGraphics();
            _gameDrawer.DisplayProbabilities(g);
        }

        private void BruteForceButton_Click(object sender, EventArgs e)
        {
            _game.BruteForce();

            using var g = this.CreateGraphics();
            g.Clear(this.BackColor);
            _gameDrawer.Draw(g);
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
