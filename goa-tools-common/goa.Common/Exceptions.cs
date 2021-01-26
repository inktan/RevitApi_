using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace goa.Common.Exceptions
{
    public class IgnorableException : Exception
    {
        //just ignore me 
    }

    /// <summary>
    /// base class for exceptions that gives specific instruction to user.
    /// </summary>
    public class CommonUserExceptions : Exception
    {
        public CommonUserExceptions() { }
        public CommonUserExceptions(string _message) : base(_message) { }
    }
    public class UserInputInvalidException : CommonUserExceptions
    {
        public override string Message
        {
            get
            {
                return "输入条件有误。请检查。";
            }
        }
    }
    public class MissingFamilySymbolException : CommonUserExceptions
    {
        public override string Message
        {
            get
            {
                return "未能找到所需要的族类型。";
            }
        }
    }
    public class UnsupportedFamilyException : CommonUserExceptions
    {
        public override string Message
        {
            get
            {
                return "不支持选择的族类型。";
            }
        }
    }
    public class InvalidPickedObjectException : CommonUserExceptions
    {
        public override string Message
        {
            get
            {
                return "所选物体无效。";
            }
        }
    }
    public class WrongActiveViewTypeException : CommonUserExceptions
    {
        public override string Message
        {
            get
            {
                return "当前视图类型错误。";
            }
        }
    }
    public class HostNotFoundException : CommonUserExceptions
    {
        private string id;
        public HostNotFoundException(string _id)
        {
            this.id = _id;
        }
        public override string Message
        {
            get
            {
                return "未找到图元的主体。图元ID:\r\n\r\n" + this.id;
            }
        }
    }
    public class TransactionNotCommitedException : CommonUserExceptions
    {
        private string m;
        public TransactionNotCommitedException(string _message)
        {
            this.m = _message;
        }
        public override string Message
        {
            get
            {
                return "对模型的操作未能完成，以下为相关信息：\r\n\r\n" + this.m;
            }
        }
    }
}
