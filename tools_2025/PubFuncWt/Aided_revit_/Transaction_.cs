using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Transaction_
    {
        /// <summary>
        /// 尝试解决——弹窗提示的异常和错误
        /// </summary>
        /// <param name="trans"></param>
        public static void DeleteErrOrWaringTaskDialog(this Transaction trans)
        {
            FailureHandlingOptions fho = trans.GetFailureHandlingOptions();
            fho.SetFailuresPreprocessor(new FiluresPrecessor());
            trans.SetFailureHandlingOptions(fho);
        }
    }

    /// <summary>
    /// 可用于执行预处理步骤的接口，以过滤出预期的事务失败或将某些失败标记为不可持续的。
    /// </summary>
    public class FiluresPrecessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> listFma = failuresAccessor.GetFailureMessages();
            if (listFma.Count <= 0)//没有任何 警告 or 弹窗 弹窗提示
            {
                return FailureProcessingResult.Continue;
            }
            foreach (FailureMessageAccessor fma in listFma)
            {
                if (fma.GetSeverity() == FailureSeverity.Error)//判断弹窗消息，是否，为 错误 提示
                {
                    string errorMsg = fma.GetDescriptionText();
                    var ex = new Exception(errorMsg);
                    string email = "wang.tan@goa.com.cn";
                    UserMessages.SendErrorMessageToWeChat(ex, email, "what???");
                    if (fma.HasResolutions())
                    {
                        failuresAccessor.ResolveFailure(fma);
                    }
                }
                if (fma.GetSeverity() == FailureSeverity.Warning)//判断弹窗消息，是否，为 警告 提示
                {
                    failuresAccessor.DeleteWarning(fma);
                }
            }
            return FailureProcessingResult.ProceedWithCommit;
        }
    }
}
