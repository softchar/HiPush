using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Pipeline {

    /// <summary>
    /// 管道上下文
    /// </summary>
    public class PipelineContext {

        /// <summary>
        /// 管道过程是否已完成，如果isFinished=true，那么管道不再向下进行处理
        /// </summary>
        private bool isFinished = false;

        /// <summary>
        /// 管道过程是否已完成
        /// </summary>
        /// <returns>true：已完成（管道的处理过程不再向下进行处理）</returns>
        public bool IsPipelineFinished() {
            return isFinished;
        }

        /// <summary>
        /// 设置管理过程已经完成，管道的处理过程不再向下进行处理
        /// </summary>
        public void SetPipelineFinished() {
            isFinished = true;
        }

    }
}
