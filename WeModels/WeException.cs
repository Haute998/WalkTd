using System;

namespace WeModels
{

    /// <summary>
    /// 返回结果类型
    /// </summary>
    public enum ReturnResult
    {
        View = 1,
        Json = 2,
        Content = 3
    }

    /// <summary>
    /// 系统异常类
    /// <code>
    /// try
    /// {
    ///     ...
    /// }
    /// catch(Exception ex)
    /// {
    ///     throw new WeException(ex.GetType().ToString(),"自定义消息隐藏敏感提示",ex.ToString());
    /// }
    /// </code>
    /// </summary>
    public class WeException : Exception
    {


        /// <summary>
        /// 异常类型
        /// </summary>
        public string Ex_TypeName { get; private set; }



        /// <summary>
        /// 异常简要
        /// </summary>
        public string Ex_Message { get; private set; }



        /// <summary>
        /// 异常说明
        /// </summary>
        public string Ex_Content { get; private set; }





        public WeException(string _TypeName, string _Message)
            : base(_Message)
        {

            Ex_TypeName = _TypeName;
            Ex_Message = _Message;
            Ex_Content = _Message;
        }


        public WeException(string _TypeName, string _Message, string _Content)
            : base(_Message)
        {

            Ex_TypeName = _TypeName;
            Ex_Message = _Message;
            Ex_Content = _Content;
        }



    }


}
