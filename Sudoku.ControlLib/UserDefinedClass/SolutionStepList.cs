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
    public class SolutionStepList
    {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private List<SolutionStep> _lstSolutionStep;
        #endregion 字段

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SolutionStepList()
        {
            _lstSolutionStep = new List<SolutionStep>();
        }
        #endregion 构造函数

        #region 公有方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ss"></param>
        public void Add(SolutionStep ss)
        {
            _lstSolutionStep.Add(ss);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _lstSolutionStep.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SolutionStep getItem(int index)
        {
            return _lstSolutionStep[index];
        }
        #endregion 公有方法
    }
}
