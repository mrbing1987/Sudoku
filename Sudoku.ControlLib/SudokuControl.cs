using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Windows.Input;
using Sudoku.ControlLib.UserDefinedClass;

namespace Sudoku.ControlLib
{
    public partial class SudokuControl : UserControl
    {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.PictureBox _picBox = null;

        /// <summary>
        /// 
        /// </summary>
        public SudokuHelper _sudoku = new SudokuHelper();

        /// <summary>
        /// 
        /// </summary>
        private SoundPlayer _squarePlace = null;

        /// <summary>
        /// 
        /// </summary>
        private SoundPlayer _puzzleSolved = null;

        /// <summary>
        /// 
        /// </summary>
        private SoundPlayer _puzzleFine = null;

        /// <summary>
        /// 
        /// </summary>
        private SoundPlayer _moveNo = null;
        #endregion 字段

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public SudokuHelper Sudoku
        {
            get
            {
                return _sudoku;
            }
        }
        #endregion 属性

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SudokuControl()
        {
            InitializeComponent();

            Application.DoEvents();

            _picBox = new PictureBox()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Location = new System.Drawing.Point(0, 0),
                Name = "pictureBox1",
                Size = new System.Drawing.Size(150, 150),
                TabIndex = 0,
                TabStop = false,
            };

            _picBox.MouseDown += pictureBox1_MouseDown;
            _picBox.Paint += pictureBox1_Paint;
            _picBox.SizeChanged += pictureBox1_SizeChanged;

            this.Controls.Add(_picBox);

            this.KeyDown += SudokuControl_KeyDown;

            //_sudoku.GenerateGame();

            LoadSounds();

            _sudoku.PlaySound += new SudokuHelper.SudokuEvent(S_PlaySound);

            _sudoku.RequestRepaint += new SudokuHelper.SudokuEvent2(RefreshSudoKu);
        }
        #endregion 构造函数

        #region 事件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _sudoku.SetLocation(e.Location);

                RefreshSudoKu();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            _sudoku.Draw(e.Graphics, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            RefreshSudoKu();
        }

        /// <summary>
        /// 按键处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SudokuControl_KeyDown(object sender, KeyEventArgs e)
        {
            _sudoku.KeyPress((char)e.KeyValue);

            if ((int)e.KeyValue == 27)
            {
                _sudoku.Deselect();
            }

            if ((int)e.KeyValue == 46)
            {
                _sudoku.DeleteCurrentSquare();
            }

            RefreshSudoKu();
        }
        #endregion 事件

        #region 私有方法
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="S"></param>
        private void S_PlaySound(SudokuHelper.SudokuSound S)
        {
            /*
            switch (S)
            {
                case Sudoku.SudokuSound.Square:
                   // SquarePlace.Play();
                    break;
                case Sudoku.SudokuSound.Solved:
                    PuzzleSolved.Play();
                    break;
                case Sudoku.SudokuSound.No:
                    MoveNo.Play();
                    break;
                case Sudoku.SudokuSound.Fine:
                    PuzzleFine.Play();
                    break;
                case Sudoku.SudokuSound.Stop :
                    PuzzleSolved.Stop();
                    break;
            }
             */
        }

        /// <summary>
        /// 加载声音
        /// </summary>
        private void LoadSounds()
        {
            _squarePlace = new SoundPlayer("square.wav");

            _puzzleSolved = new SoundPlayer("warm-interlude.wav");

            _puzzleFine = new SoundPlayer("fine.wav");

            _moveNo = new SoundPlayer("no.wav");

            Application.DoEvents();

            //_squarePlace.Load();

            //_puzzleSolved.Load();

            //_puzzleFine.Load();

            //_moveNo.Load();

        }
        #endregion 私有方法

        #region 公有方法
        /// <summary>
        /// 刷新数独状态
        /// </summary>
        public void RefreshSudoKu()
        {
            if (_picBox != null)
            {
                _picBox.Invalidate();
            }
        }

        /// <summary>
        /// 设置被按下的按键
        /// </summary>
        /// <param name="keyCode">键值</param>
        public void SetKeyValue(int keyCode)
        {
            _sudoku.KeyPress((char)keyCode);

            if (keyCode == 27)
            {
                _sudoku.Deselect();
            }

            if ((keyCode == 46) || (keyCode == 8))
            {
                _sudoku.DeleteCurrentSquare();
            }

            RefreshSudoKu();
        }
        #endregion 公有方法
    }
}
