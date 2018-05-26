using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Result
{

    public class MessageResult<TResponse,TCode> 
        where TResponse : MessageResult<TResponse, TCode>, new() 
        where TCode : struct
    {

        private bool state = true;
        /// <summary>
        /// 提交/请求状态是否成功
        /// </summary>
        public bool State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public virtual TCode StatusCode { get; set; }

        /// <summary>
        /// 状态描述文本
        /// </summary>
        public virtual string StatusText { get; set; }

        private ReturnValue returnValue;
        public ReturnValue GetReturnValue()
        {
            return returnValue;
        }

        public TResponse Create(ReturnValue retValue)
        {
            returnValue = retValue;

            var result = new TResponse();
            
            result.State = retValue.Code == 0;
            result.StatusCode = (TCode)Enum.Parse(typeof(TCode), retValue.Code.ToString());
            result.StatusText = retValue.GetBrokenRuleString();

            return result;
        }

        public TResponse Create(BusinessRuleException excep) {

            returnValue = excep.ReturnValue;

            var result = new TResponse();

            result.State = excep.Code == 0;
            result.StatusCode = (TCode)Enum.Parse(typeof(TCode), excep.Code.ToString());
            result.StatusText = excep.ReturnValue.GetBrokenRuleString();

            return result;
        }
    }

}
