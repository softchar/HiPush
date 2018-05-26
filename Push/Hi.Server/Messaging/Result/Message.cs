using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiPush.Server.Messaging.Response
{
    public class CreateMessageResponse : ResponseBase<CreateMessageResponse, CreateMessageResponseState>
    {
        private CreateMessageResponseState statusCode = CreateMessageResponseState.Sys_Success;
        public override CreateMessageResponseState StatusCode
        {
            get
            {
                return statusCode;
            }
            set
            {
                statusCode = value;
            }
        }
    }

    public enum CreateMessageResponseState
    {
        Sys_Success,        //成功
        Sys_Fail,           //失败
        Sys_Paramter,       //参数有误
        Sys_Business,       //没有满足固定的业务规则
        User_Had,           //AppName已存在
    }
}
