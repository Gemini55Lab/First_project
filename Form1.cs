using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tettris_game
{
    public partial class frmTetris : Form
    {
        private const int gridWidth = 10;
        private const int gridHeight = 20;
        private int[,] gameGrid = new int[gridWidth, gridHeight];
        private int score = 0;
        private int level = 1;
        private int currentX = 3, currentY = 0;
        private int[,] currentPiece;
        private int pieceSize; // 2, 3, or 4 depending on piece
        private int pieceType;

        public frmTetris()
        {
            InitializeComponent();

            gamePanel.Size = new Size(300, 600);

            gameTimer.Interval = 1000; // starting speed (Level 1)
            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Start();

            lblScore.Text = "Score: 0";
            lblLevel.Text = "Level: 1";

            level = 1;
            score = 0;

            UpdateGameSpeed();   // 🔥 make sure speed matches level

            GenerateNewPiece();
        }


        private void gamePanel_Paint(object sender,PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw placed blocks
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gameGrid[x, y] == 1)
                    {
                        g.FillRectangle(Brushes.Blue, x * 30, y * 30, 30, 30);
                    }
                    g.DrawRectangle(Pens.Black, x * 30, y * 30, 30, 30);
                }
            }

            // Draw current falling piece
            if (currentPiece != null)
            {
                for (int y = 0; y < currentPiece.GetLength(0); y++)
                {
                    for (int x = 0; x < currentPiece.GetLength(1); x++)
                    {
                        if (currentPiece[y, x] == 1)
                        {
                            int drawX = (currentX + x) * 30;
                            int drawY = (currentY + y) * 30;

                            g.FillRectangle(Brushes.Red, drawX, drawY, 30, 30);
                            g.DrawRectangle(Pens.Black, drawX, drawY, 30, 30);
                        }
                    }
                }
            }


        }

        private void gameTimer_Tick(object sender,EventArgs e)
        {
            // game logic here

            MovePieceDown();
            gamePanel.Invalidate(); // redraw
        }

        private void GenerateNewPiece()
        {
            Random rand = new Random();
            pieceType = rand.Next(0, 7);
            currentX = 3;
            currentY = 0;

            switch (pieceType)
            {
                case 0: // I
                    currentPiece = new int[,]
                    {
                {1,1,1,1}
                    };
                    pieceSize = 4;
                    break;

                case 1: // O
                    currentPiece = new int[,]
                    {
                {1,1},
                {1,1}
                    };
                    pieceSize = 2;
                    break;

                case 2: // T
                    currentPiece = new int[,]
                    {
                {0,1,0},
                {1,1,1},
                {0,0,0}
                    };
                    pieceSize = 3;
                    break;

                case 3: // L
                    currentPiece = new int[,]
                    {
                {1,0,0},
                {1,0,0},
                {1,1,0}
                    };
                    pieceSize = 3;
                    break;

                case 4: // J
                    currentPiece = new int[,]
                    {
                {0,0,1},
                {0,0,1},
                {0,1,1}
                    };
                    pieceSize = 3;
                    break;

                case 5: // S
                    currentPiece = new int[,]
                    {
                {0,1,1},
                {1,1,0},
                {0,0,0}
                    };
                    pieceSize = 3;
                    break;

                case 6: // Z
                    currentPiece = new int[,]
                    {
                {1,1,0},
                {0,1,1},
                {0,0,0}
                    };
                    pieceSize = 3;
                    break;
            }
        }


        private void MovePieceDown()
        {

            if (CanMove(0, 1))
            {
                currentY++;
            }
            else
            {
                PlacePiece();
                int lines = ClearLines();  // 🔥 check for completed rows
                AddScore(lines);           // 🔥 update score + level + speed
                GenerateNewPiece();
            }
        }

        private void UpdateGameSpeed()
        {
            // Speed increases every 100 points, even in Level 1
            int speedStep = score / 100;

            int newInterval = 1000 - (speedStep * 80); // reduce by 50ms each step

            if (newInterval < 100) // don't go crazy fast
                newInterval = 100;

            gameTimer.Interval = newInterval;
        }


        private bool CanMove(int xOffset, int yOffset)
        {
            for (int y = 0; y < currentPiece.GetLength(0); y++)
            {
                for (int x = 0; x < currentPiece.GetLength(1); x++)
                {
                    if (currentPiece[y, x] == 0) continue;

                    int newX = currentX + x + xOffset;
                    int newY = currentY + y + yOffset;

                    // Check walls
                    if (newX < 0 || newX >= gridWidth)
                        return false;

                    // Check floor
                    if (newY >= gridHeight)
                        return false;

                    // Check collision with placed blocks
                    if (newY >= 0 && gameGrid[newX, newY] == 1)
                        return false;
                }
            }
            return true;
        }

        private void PlacePiece()
        {
            // ifaka incezuu kwi-grid
            for (int y = 0; y < currentPiece.GetLength(0); y++)
            {
                for (int x = 0; x < currentPiece.GetLength(1); x++)
                {
                    if (currentPiece[y, x] == 1)
                    {
                        int gridX = currentX + x;
                        int gridY = currentY + y;

                        if (gridY >= 0)
                            gameGrid[gridX, gridY] = 1;
                    }
                }
            }
        }

        private void gamePanel_Paint_1(object sender, PaintEventArgs e)
        {
            this.gamePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gamePanel_Paint);
        }

        private void AddScore(int linesCleared)
        {
            if (linesCleared == 0) return;

            score += linesCleared * 100;
            lblScore.Text = "Score: " + score;

            // Level still increases normally
            int newLevel = score / 500 + 1;

            if (newLevel > level)
            {
                level = newLevel;
                lblLevel.Text = "Level: " + level;
            }

            UpdateGameSpeed(); // 🔥 speed changes even if level didn't
        }


        private int ClearLines()
        {
            int linesCleared = 0;

            for (int y = gridHeight - 1; y >= 0; y--)
            {
                bool fullLine = true;

                for (int x = 0; x < gridWidth; x++)
                {
                    if (gameGrid[x, y] == 0)
                    {
                        fullLine = false;
                        break;
                    }
                }

                if (fullLine)
                {
                    linesCleared++;

                    for (int row = y; row > 0; row--)
                        for (int col = 0; col < gridWidth; col++)
                            gameGrid[col, row] = gameGrid[col, row - 1];

                    for (int col = 0; col < gridWidth; col++)
                        gameGrid[col, 0] = 0;

                    y++; // recheck same row
                }
            }

            return linesCleared;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    if (CanMove(-1, 0))
                        currentX--;
                    break;

                case Keys.Right:
                    if (CanMove(1, 0))
                        currentX++;
                    break;

                case Keys.Down:
                    MovePieceDown();
                    break;
            }

            gamePanel.Invalidate();
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
