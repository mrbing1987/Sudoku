using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Linq;

namespace Sudoku.ControlLib.UserDefinedClass
{
    /// <summary>
    /// 
    /// </summary>
    public class SudokuHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="S"></param>
        public delegate void SudokuEvent(SudokuSound S);

        /// <summary>
        /// 
        /// </summary>
        public delegate void SudokuEvent2();

        /// <summary>
        /// 
        /// </summary>
        public event SudokuEvent PlaySound;

        /// <summary>
        /// 
        /// </summary>
        public event SudokuEvent2 RequestRepaint;

        #region 枚举
        /// <summary>
        /// 数独声音
        /// </summary>
        public enum SudokuSound
        {
            Stop = 0,
            Square = 1,
            No = 2,
            Fine = 3,
            Delete = 4,
            Solved = 5,
            NewPuzzle = 6,
        };

        /// <summary>
        /// 解决方法
        /// </summary>
        public enum SolveMethods
        {
            /// <summary>
            /// 
            /// </summary>
            All = 65535,
            /// <summary>
            /// 
            /// </summary>
            NakedSingles = 1,
            /// <summary>
            /// 
            /// </summary>
            HiddenSingles = 2,
            /// <summary>
            /// 
            /// </summary>
            BlockHiddenSingles = 4,
        }
        #endregion 枚举

        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private int[,] s = new int[9, 9]; // sudoku numbers

        /// <summary>
        /// 
        /// </summary>
        private int[,] a = new int[9, 9]; // aids

        /// <summary>
        /// 
        /// </summary>
        private int[,] f = new int[9, 9]; // facts

        /// <summary>
        /// 
        /// </summary>
        private int[,] e = new int[9, 9]; // errors

        /// <summary>
        /// 
        /// </summary>
        private Random R = new Random();

        /// <summary>
        /// 
        /// </summary>
        private float realx = 0;

        /// <summary>
        /// 
        /// </summary>
        private float realy = 0;

        /// <summary>
        /// 
        /// </summary>
        private float realw = 0;

        /// <summary>
        /// 
        /// </summary>
        private int hitx = -1;

        /// <summary>
        /// 
        /// </summary>
        private int hity = 0;

        /// <summary>
        /// 
        /// </summary>
        public int sw = 0;

        /// <summary>
        /// 
        /// </summary>
        private DateTime starttime = default(DateTime);

        /// <summary>
        /// 
        /// </summary>
        private string duration = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string msg_text1 = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string msg_text2 = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private bool DisplayMessage = false;

        /// <summary>
        /// 
        /// </summary>
        private bool PriorityMessage = false;
        #endregion 字段

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public bool ShowErrors { get; set; }
        #endregion 属性

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SudokuHelper()
        {
            EnumeratePossibilities();
        }
        #endregion 构造函数

        #region 私有方法
        /// <summary>
        /// 使用数独模板
        /// </summary>
        private void UseTemplate(int level = 0)
        {
            string gameTemplate = string.Empty;

            if (level == 0)
            {
                gameTemplate = "X6X1X4X5XXX83X56XX2XXXXXXX18XX4X7XX6XX6XXX3XX7XX9X1XX45XXXXXXX2XX72X69XXX4X5X8X7X";
            }
            else if (level == 1)
            {
                gameTemplate = "XXXX7XX6X6XXXXXX42XXX89XX15XXXXX54XX769XXX521XX19XXXXX91XX42XXX28XXXXXX3X7XX8XXXX";
            }
            else if (level == 2)
            {
                gameTemplate = "XXXXXXXX547XXXXXXX85XX42XXX64X58XXXXXX79X41XXXXXX73X96XXX85XX34XXXXXXX673XXXXXXXX";
            }
            else
            {
                gameTemplate = "829173456713645298645298173381726945976514832452839617297381564168457329534962781";
            }

            SetGameString(gameTemplate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsFull()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s[i, j] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        private bool CheckOneToNine(List<int> lst)
        {
            if ((lst == null) || (lst.Count != 9))
            {
                MessageBox.Show("错误!");
            }

            int[] vals = new int[10];

            for (int i = 0; i < 9; i++)
            {
                int item = lst[i];

                if (item > 0)
                {
                    vals[item]++;

                    if (vals[lst[i]] > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 列举解数独步骤的可能性
        /// </summary>
        private void EnumeratePossibilities()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    a[j, i] = 511;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s[j, i] > 0)
                    {
                        a[j, i] = (1 << (s[j, i]) - 1);

                        int mask = 511 - (1 << (s[j, i]) - 1);

                        int blockx = i / 3;

                        int blocky = j / 3;

                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if ((blocky * 3 + k != j) || (blockx * 3 + l != i))
                                {
                                    a[blocky * 3 + k, blockx * 3 + l] &= mask;
                                }
                            }
                        }

                        for (int n = 0; n < 9; n++)
                        {
                            if (n != i)
                            {
                                a[j, n] &= mask;
                            }

                            if (n != j)
                            {
                                a[n, i] &= mask;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 交换行
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        private void SwapRows(int r1, int r2)
        {
            int temp = 0;

            for (int n = 0; n < 9; n++)
            {
                temp = s[r1, n];

                s[r1, n] = s[r2, n];

                s[r2, n] = temp;

                temp = f[r1, n];

                f[r1, n] = f[r2, n];

                f[r2, n] = temp;

                temp = a[r1, n];

                a[r1, n] = a[r2, n];

                a[r2, n] = temp;
            }
        }

        /// <summary>
        /// 交换列
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        private void SwapCols(int c1, int c2)
        {
            int temp = 0;

            for (int n = 0; n < 9; n++)
            {
                temp = s[n, c1];

                s[n, c1] = s[n, c2];

                s[n, c2] = temp;

                temp = f[n, c1];

                f[n, c1] = f[n, c2];

                f[n, c2] = temp;

                temp = a[n, c1];

                a[n, c1] = a[n, c2];

                a[n, c2] = temp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChangeSymbols()
        {
            int[] symbols = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            int scrambles = 10;

            for (int n = 0; n < scrambles; n++)
            {
                int index1 = R.Next(9) + 1;

                int index2 = R.Next(9) + 1;

                int temp = symbols[index1];

                symbols[index1] = symbols[index2];

                symbols[index2] = temp;
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    s[i, j] = symbols[s[i, j]];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private SudokuHelper CreateCopy()
        {
            SudokuHelper copy = new SudokuHelper();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    copy.s[i, j] = s[i, j];
                }
            }

            return copy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayPuzzleSolved(object sender, EventArgs e)
        {
            Timer T = (Timer)sender;
            T.Stop();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    f[i, j] = 1;
                }
            }

            if (PlaySound != null)
            {
                PlaySound(SudokuSound.Solved);
            }

            msg_text1 = "数独已解决";

            msg_text2 = duration;

            PriorityMessage = true;

            DisplayMessage = true;

            if (RequestRepaint != null)
            {
                RequestRepaint();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ss"></param>
        private void ApplySolutionStep(SolutionStep ss)
        {
            s[ss.Y, ss.X] = ss.Num;

            f[ss.Y, ss.X] = 0;
        }
        #endregion 私有方法

        #region 公有方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Location"></param>
        public void SetLocation(Point Location)
        {
            if (DisplayMessage & !PriorityMessage)
            {
                DisplayMessage = false;

                return;
            }

            int new_hitx = (int)Math.Floor((Location.X - realx) / realw * 9.0);

            int new_hity = (int)Math.Floor((Location.Y - realy) / realw * 9.0);

            if ((new_hitx < 0) || (new_hitx > 8) || (new_hity < 0) || (new_hity > 8))
            {
                hitx = -1;

                return;
            }

            if ((new_hitx == hitx) && (new_hity == hity))
            {
                hitx = -1;
            }
            else
            {
                if (f[new_hity, new_hitx] == 0)
                {
                    hitx = new_hitx;

                    hity = new_hity;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Deselect()
        {
            hitx = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteCurrentSquare()
        {
            if (hitx != -1)
            {
                s[hity, hitx] = 0;
                e[hity, hitx] = 0;
            }

            hitx = -1;

            EnumeratePossibilities();
        }

        /// <summary>
        /// 处理按键
        /// </summary>
        /// <param name="KeyCode">键值</param>
        public void KeyPress(char KeyCode)
        {
            if (DisplayMessage & !PriorityMessage)
            {
                DisplayMessage = false;

                return;
            }

            if ((KeyCode < '1') || (KeyCode > '9'))
            {
                return;
            }

            if (hitx == -1)
            {
                return;
            }

            s[hity, hitx] = KeyCode - '0';

            e[hity, hitx] = 0;

            hitx = -1;

            EnumeratePossibilities();

            if (PlaySound != null)
            {
                PlaySound(SudokuSound.Square);
            }

            if (IsSolved())
            {
                while (hitx != -1)
                {
                    Application.DoEvents();
                }

                Timer T = new Timer();
                T.Tick += new EventHandler(DisplayPuzzleSolved);
                T.Interval = 1500;
                T.Start();

                TimeSpan d = (DateTime.Now - starttime);

                duration = string.Empty;

                if (d.Minutes > 0)
                {
                    duration = d.Minutes.ToString() + " minute";
                }

                if (d.Minutes > 1)
                {
                    duration += "s";
                }

                if ((d.Minutes > 0) && (d.Seconds > 0))
                {
                    duration += " and ";
                }

                if (d.Seconds > 0)
                {
                    duration += d.Seconds.ToString() + " second";
                }

                if (d.Seconds > 1)
                {
                    duration += "s";
                }
            }
        }

        /// <summary>
        /// 创建一个数独游戏
        /// </summary>
        /// <param name="level">游戏等级(0-简单,1-一般,2-难)</param>
        public void GenerateGame(int level = 0)
        {
            if (PlaySound != null)
            {
                PlaySound(SudokuSound.Stop);
            }

            UseTemplate(level);

            int trials_counter = 0;

            int trials_max = 100;

            SolveMethods M = SolveMethods.All;

            List<Point> lstPoint = new List<Point>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s[j, i] > 0)
                    {
                        lstPoint.Add(new Point(i, j));
                    }
                }
            }

            for (int n = 0; n < 55;)
            {
                int index = R.Next(lstPoint.Count);

                int x = lstPoint[index].X;

                int y = lstPoint[index].Y;

                int temp = s[x, y];

                if (temp != 0)
                {
                    s[x, y] = 0;

                    SudokuHelper S2 = CreateCopy();

                    int solutionStepsCount = S2.ComputePossibleSteps(M).Count();

                    bool range0Ok = ((n > 35) && (solutionStepsCount > 25));
                    bool range1Ok = ((n > 15) && (solutionStepsCount > 25));
                    bool range2Ok = n < 16;

                    bool solutionStepsCountOk = range0Ok || range1Ok || range2Ok;

                    if (solutionStepsCountOk)
                    {
                        bool isSolvable = S2.SolvePuzzle(M);

                        if (isSolvable)
                        {
                            lstPoint.RemoveAt(index);

                            n++;

                            trials_counter = 0;
                        }
                        else
                        {
                            s[x, y] = temp;
                        }
                    }
                    else
                    {
                        s[x, y] = temp;
                    }
                    trials_counter++;
                }

                if (trials_counter > trials_max)
                {
                    break;
                }
            }

            ScrambleGame();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s[i, j] == 0)
                    {
                        f[i, j] = 0;
                    }
                    else
                    {
                        f[i, j] = 1;
                    }
                }
            }

            EnumeratePossibilities();

            starttime = DateTime.Now;

            DisplayMessage = false;

            if (PlaySound != null)
            {
                PlaySound(SudokuSound.NewPuzzle);
            }

            if (RequestRepaint != null)
            {
                RequestRepaint();
            }
        }

        /// <summary>
        /// 在数独游戏界面上显示信息
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="priority"></param>
        public void RenderMessage(string line1, string line2, bool priority)
        {
            if (!priority && DisplayMessage)
            {
                return;
            }

            msg_text1 = line1;
            msg_text2 = line2;

            PriorityMessage = priority;

            DisplayMessage = true;

            if (RequestRepaint != null)
            {
                RequestRepaint();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ScrambleGame()
        {
            //0 1, 1 2, 3 4, 4 5, 6 7 7 8

            int[] scramble_array = { 0, 1, 3, 4, 6, 7 };

            int scrambles = 10;

            for (int n = 0; n < scrambles; n++)
            {
                int index = R.Next(6);

                int val = scramble_array[index];

                SwapRows(val, val + 1);
            }

            for (int n = 0; n < scrambles; n++)
            {
                int index = R.Next(6);

                int val = scramble_array[index];

                SwapCols(val, val + 1);
            }

            ChangeSymbols();

            EnumeratePossibilities();
        }

        /// <summary>
        /// 获取数独游戏中的数据
        /// </summary>
        /// <returns></returns>
        public string GetGameString()
        {
            string result = string.Empty;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result += s[i, j].ToString();
                }
            }

            result = result.Replace("0", "X");

            return result;
        }

        /// <summary>
        /// 为数独游戏赋值
        /// </summary>
        /// <param name="game">数独游戏字符串</param>
        /// <returns></returns>
        public bool SetGameString(string game)
        {
            if ((string.IsNullOrEmpty(game)) || (game.Length != 81))
            {
                return false;
            }

            for (int loop = 0; loop < 81; loop++)
            {
                char c = game[loop];

                if (((c < '1') || (c > '9')) && (c != 'X'))
                {
                    return false;
                }
            }

            for (int loop = 0; loop < 81; loop++)
            {
                string c = game.Substring(loop, 1);

                int i = loop / 9;

                int j = loop % 9;

                if (c == "X")
                {
                    s[i, j] = 0;
                    f[i, j] = 0;
                }
                else
                {
                    s[i, j] = Convert.ToInt32(c);
                    f[i, j] = 1;
                }
            }

            EnumeratePossibilities();

            DisplayMessage = false;

            return true;
        }

        /// <summary>
        /// 绘制数独
        /// </summary>
        /// <param name="G"></param>
        /// <param name="angle"></param>
        public void Draw(Graphics G, float angle)
        {
            Color Background = Color.DarkKhaki;

            Brush BG1 = new Pen(Color.Khaki).Brush;

            Brush BG2 = new Pen(Color.LightGoldenrodYellow).Brush;

            Brush BG;

            Brush Selected = new Pen(Color.BurlyWood).Brush;

            Selected = new SolidBrush(Color.FromArgb(64, Color.RoyalBlue));

            Brush FontColor1 = Brushes.Black;

            Brush FontColor2 = Brushes.RoyalBlue;

            Brush FontColor3 = Brushes.Crimson;

            Brush FontColor = FontColor1;

            Pen Error = new Pen(Color.Red, 3);

            SolidBrush SmallFontColor = new SolidBrush(Color.FromArgb(200, Color.Black));

            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            G.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            G.Clear(Background);

            Pen Border1 = new Pen(Color.Black, 3);

            Pen Border2 = new Pen(Color.Black, 1);

            float w = G.VisibleClipBounds.Width;

            float h = G.VisibleClipBounds.Height;

            float min = Math.Min(w, h);

            float centre_x = w / 2;

            float centre_y = h / 2;

            float startx = centre_x - min / 2;

            float starty = centre_y - min / 2;

            float m = 50;

            realx = startx + m;

            realy = starty + m;

            realw = min - 2 * m;

            if (realw <= 0)
            {
                return;
            }

            float error_circle_m = realw / 9 * 0.95f;

            Font F = new Font("Arial", realw / 20);

            Font Fsmall = new Font("Arial", realw / 72);

            G.TranslateTransform(centre_x, centre_y);

            G.RotateTransform(angle);

            G.TranslateTransform(-centre_x, -centre_y);

            G.FillRectangle(Brushes.White, realx, realy, realw, realw);

            G.DrawRectangle(Border1, realx, realy, realw, realw);

            if (sw == 5)
            {
                FontColor2 = FontColor1;
                FontColor3 = FontColor1;
            }

            string num = string.Empty;

            for (float i = 0; i < 3; i++)
            {
                for (float j = 0; j < 3; j++)
                {
                    G.DrawRectangle(Border1, realx + realw * i / 3, realy + realw * j / 3, realw / 3, realw / 3);

                    if ((i + j) % 2 == 0)
                    {
                        BG = BG1;
                    }
                    else
                    {
                        BG = BG2;
                    }

                    G.FillRectangle(BG, realx + realw * i / 3, realy + realw * j / 3, realw / 3, realw / 3);

                    for (float i2 = 0; i2 < 3; i2++)
                    {
                        for (float j2 = 0; j2 < 3; j2++)
                        {
                            if (i * 3 + i2 == hitx && j * 3 + j2 == hity)
                            {
                                G.FillRectangle(Selected, realx + realw * (i / 3 + i2 / 9), realy + realw * (j / 3 + j2 / 9), realw / 9, realw / 9);
                            }

                            G.DrawRectangle(Border2, realx + realw * (i / 3 + i2 / 9), realy + realw * (j / 3 + j2 / 9), realw / 9, realw / 9);

                            int index_i = Convert.ToInt32(i * 3 + i2);

                            int index_j = Convert.ToInt32(j * 3 + j2);

                            num = s[index_j, index_i].ToString();

                            if (num == "0")
                            {
                                num = string.Empty;
                            }

                            if (f[index_j, index_i] == 1)
                            {
                                FontColor = FontColor1;
                            }

                            if (f[index_j, index_i] == 0)
                            {
                                FontColor = FontColor2;
                            }

                            SizeF size_num = G.MeasureString(num, F);

                            float num_x = realx + realw * (i / 3 + i2 / 9) + (realw / 9 - size_num.Width) / 2;

                            float num_y = realy + realw * (j / 3 + j2 / 9) + (realw / 9 - size_num.Height) / 2;

                            RectangleF x = new RectangleF(num_x, num_y, size_num.Width, size_num.Height);

                            RectangleF y = new RectangleF(realx + realw * (i / 3 + i2 / 9), realy + realw * (j / 3 + j2 / 9), realw / 9, realw / 9);

                            RectangleF z = new RectangleF(realx + realw * (i / 3 + i2 / 9) + error_circle_m, realy + realw * (j / 3 + j2 / 9) + error_circle_m, realw / 9 - 2 * error_circle_m, realw / 9 - 2 * error_circle_m);

                            float num_centre_x = num_x + size_num.Width / 2;

                            float num_centre_y = num_y + size_num.Height / 2;

                            G.TranslateTransform(num_centre_x, num_centre_y);

                            G.RotateTransform(-angle);

                            G.TranslateTransform(-num_centre_x, -num_centre_y);

                            G.DrawString(num, F, FontColor, x);

                            G.TranslateTransform(num_centre_x, num_centre_y);

                            G.RotateTransform(+angle);

                            G.TranslateTransform(-num_centre_x, -num_centre_y);

                            if (e[index_j, index_i] == 1 && ShowErrors)
                            {
                                G.DrawEllipse(Error, z);
                            }

                            string hints = string.Empty;

                            for (int b = 0; b < 9; b++)
                            {
                                if (((a[index_j, index_i] >> b) & 1) == 1)
                                {
                                    hints = hints + (b + 1).ToString();
                                }
                            }
                        }
                    }
                }
            }

            if (DisplayMessage)
            {
                RenderMessageBox(G, msg_text1, msg_text2, realw / 22, realw / 36, w, h, realx, realy, realw);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="G"></param>
        /// <param name="txt1"></param>
        /// <param name="txt2"></param>
        /// <param name="emSize1"></param>
        /// <param name="emSize2"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="realx"></param>
        /// <param name="realy"></param>
        /// <param name="realw"></param>
        private void RenderMessageBox(Graphics G, string txt1, string txt2, float emSize1, float emSize2, float w, float h, float realx, float realy, float realw)
        {
            Brush Overlay = new SolidBrush(Color.FromArgb(150, Color.White));

            Brush MsgBoxBG = new SolidBrush(Color.LightSlateGray);

            RectangleF msgbox = new RectangleF(realx + realw / 12, realy + realw / 4, realw * 10 / 12, realw / 3);

            Font F1 = new Font("Arial", emSize1);

            Font F2 = new Font("Arial", emSize2);

            SizeF txt1size = G.MeasureString(txt1, F1);

            SizeF txt2size = G.MeasureString(txt2, F2);

            G.FillRectangle(Overlay, new RectangleF(0, 0, w, h));

            G.FillRectangle(MsgBoxBG, msgbox);

            G.DrawRectangle(Pens.Black, realx + realw / 12, realy + realw / 4, realw * 10 / 12, realw / 3);

            PointF txt1pos = new PointF(msgbox.Left + (msgbox.Width - txt1size.Width) / 2, msgbox.Top + (msgbox.Height - txt1size.Height) / 2 - msgbox.Height / 5);

            PointF txt2pos = new PointF(msgbox.Left + (msgbox.Width - txt2size.Width) / 2, msgbox.Top + (msgbox.Height - txt2size.Height) / 2 + msgbox.Height / 5);

            G.DrawString(txt1, F1, Brushes.White, txt1pos);

            G.DrawString(txt2, F2, Brushes.White, txt2pos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckGame()
        {
            List<int> lst = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                // 检查行:
                lst.Clear();

                for (int j = 0; j < 9; j++)
                {
                    lst.Add(s[i, j]);
                }

                if (CheckOneToNine(lst) == false)
                {
                    return false;
                }

                // 检查列
                lst.Clear();

                for (int j = 0; j < 9; j++)
                {
                    lst.Add(s[j, i]);
                }

                if (CheckOneToNine(lst) == false)
                {
                    return false;
                }

                // 检查9x9块
                for (int x = 0; x < 9; x += 3)
                {
                    for (int y = 0; y < 9; y += 3)
                    {
                        lst.Clear();

                        for (int j = 0; j < 3; j++)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                lst.Add(s[x + j, y + k]);
                            }
                        }

                        if (CheckOneToNine(lst) == false)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public bool checkSolvable(SolveMethods M)
        {
            SudokuHelper copy = CreateCopy();

            return copy.SolvePuzzle(M);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public SolutionStepList ComputePossibleSteps(SolveMethods M)
        {
            EnumeratePossibilities();

            SolutionStepList L = new SolutionStepList();

            // Looking for naked singles
            if ((M & SolveMethods.NakedSingles) > 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        double index = Math.Log(a[j, i], 2) + 1;

                        if (index == Math.Floor(index) && a[j, i] > 0 && s[j, i] == 0)
                        {
                            L.Add(new SolutionStep(i, j, Convert.ToInt32(index)));
                        }
                    }
                }
            }

            // Looking for hidden singles
            if ((M & SolveMethods.HiddenSingles) > 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        int mask = 0;

                        for (int m = 0; m < 9; m++)
                        {
                            if (m != j) mask |= a[m, i];
                        }

                        int match = a[j, i] & (511 - mask);

                        if (s[j, i] == 0 && match > 0)
                        {
                            double index = Math.Log(match, 2) + 1;

                            L.Add(new SolutionStep(i, j, Convert.ToInt32(index)));
                        }
                    }
                }
            }

            // Looking for block-hidden singles
            if ((M & SolveMethods.BlockHiddenSingles) > 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        int blockx = i / 3;

                        int blocky = j / 3;

                        int mask = 0;

                        for (int m = 0; m < 3; m++)
                        {
                            for (int n = 0; n < 3; n++)
                            {
                                int xindex = blockx * 3 + m;

                                int yindex = blocky * 3 + n;

                                if (yindex != j || xindex != i)
                                {
                                    mask |= a[yindex, xindex];
                                }
                            }
                        }

                        int match = a[j, i] & (511 - mask);

                        if (s[j, i] == 0 && match > 0)
                        {
                            double index = Math.Log(match, 2) + 1;

                            L.Add(new SolutionStep(i, j, Convert.ToInt32(index)));
                        }
                    }
                }
            }

            return L;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public bool SolveStep(SolveMethods M)
        {
            SolutionStepList L = ComputePossibleSteps(M);

            if (L.Count() == 0)
            {
                return false;
            }

            ApplySolutionStep(L.getItem(0));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public bool SolvePuzzle(SolveMethods M)
        {
            while (SolveStep(M)) ;

            return (IsFull());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool LoadFile(string file)
        {
            string content = string.Empty;

            using (StreamReader R = new StreamReader(file))
            {
                content = R.ReadLine();
            }

            if (content.Length != 81)
            {
                return false;
            }

            return SetGameString(content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool SaveFile(string file)
        {
            using (StreamWriter W = new StreamWriter(file))
            {
                W.WriteLine(GetGameString());

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ComputeErrors()
        {
            SudokuHelper S2 = this.CreateCopy();

            int errors_count = 0;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (f[j, i] == 0) S2.s[j, i] = 0;
                }
            }

            S2.SolvePuzzle(SolveMethods.All);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (S2.s[j, i] == s[j, i] || s[j, i] == 0)
                    {
                        e[j, i] = 0;
                    }
                    else
                    {
                        e[j, i] = 1;
                        errors_count++;
                    }
                }
            }
            return errors_count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsSolved()
        {
            return (IsFull() && checkSolvable(SolveMethods.All));
        }
        #endregion 公有方法
    }
}
