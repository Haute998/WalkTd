using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBusiness.Models
{
    /// <summary>
    /// 菜单权限标签
    /// </summary>
    public class B_MenuRightsTagAttribute : Attribute
    {


        private string _Name = string.Empty;
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
        }



        private bool _IsMainMenu = true;
        /// <summary>
        /// 是否主菜单
        /// </summary>
        public bool IsMainMenu
        {
            get
            {
                return _IsMainMenu;
            }
        }

        private string _MainMethod = string.Empty;
        /// <summary>
        /// 主权限方法名
        /// </summary>
        public string MainMethod
        {
            get
            {
                return _MainMethod;
            }
        }

        /// <summary>
        /// 初始构造(权限名称)
        /// </summary>
        /// <param name="boName">权限名称</param>
        public B_MenuRightsTagAttribute(string boName)
        {
            _Name = boName;
        }



        /// <summary>
        /// 初始构造(权限名称,主方法)
        /// </summary>
        /// <param name="boName">权限名称</param>
        /// <param name="boMainMethod"></param>
        public B_MenuRightsTagAttribute(string boName, string boMainMethod)
        {
            _Name = boName;

            _MainMethod = boMainMethod;
            _IsMainMenu = string.IsNullOrWhiteSpace(_MainMethod);
        }




    }
}