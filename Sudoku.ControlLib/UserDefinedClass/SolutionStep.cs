using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.ControlLib.UserDefinedClass
{
    /// <summary>
    /// 
    /// </summary>
    public class SolutionStep
    {
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Num { get; set; }
        #endregion 属性

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SolutionStep()
        {
            X = 0; Y = 0; Num = 0;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="num"></param>
        public SolutionStep(int x, int y, int num)
        {
            this.X = x;
            this.Y = y;
            this.Num = num;
        }
        #endregion 构造函数
    }
}
