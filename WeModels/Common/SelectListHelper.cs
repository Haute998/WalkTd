using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WeModels
{
    public class SelectListHelper
    {
        /// <summary>
        /// 将List对象转换成SelectList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list对象</param>
        /// <param name="valField">实际值列名</param>
        /// <param name="txtField">显示值列名</param>
        /// <param name="selVal">选中值</param>
        /// <returns></returns>
        public static SelectList ListToSelectList<T>(List<T> list, string valField, string txtField, string selVal)
        {
            if (txtField != null && valField != null)
            {
                if (selVal != null)
                {
                    return new SelectList(list, valField, txtField, selVal);
                }
                else
                {
                    return new SelectList(list, valField, txtField);
                }
            }
            return null;
        }

        public static SelectList GetSYSIntegralCodeArea_AreaName(string selVal, string emptytext)
        {
            List<SYSIntegralCodeArea> list = SYSIntegralCodeArea.GetEntitysAll().ToList();
            SYSIntegralCodeArea item = new SYSIntegralCodeArea();
            item.ID = 0;
            item.AreaName = emptytext;
            list.Insert(0, item);
            return ListToSelectList(list, "AreaName", "AreaName", selVal);
        }

        public static IEnumerable<SelectListItem> GetEntitysAll1()
        {
            List<B_problem> list = B_problem.GetEntitysAllc();
            B_problem role = new B_problem();
            role.problem = "- -请选择- -";
            list.Insert(0, role);
            return ListToSelectList(list, "problem", "problem", null);
        }
        public static IEnumerable<SelectListItem> GetEntitysAll2()
        {
            List<B_problem> list = B_problem.GetEntitysAlle();
            B_problem role = new B_problem();
            role.problem = "- -请选择- -";
            list.Insert(0, role);
            return ListToSelectList(list, "problem", "problem", null);
        }
        /// <summary>
        /// 上级菜单列表
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetParentMenu(string selVal)
        {
            List<B_Menu> list = B_Menu.GetValidMenus().Where(m => m.ParentID == 0).ToList();
            B_Menu menu = new B_Menu();
            menu.ID = 0;
            menu.MenuName = "无";
            list.Insert(0, menu);
            return ListToSelectList(list, "ID", "MenuName", selVal);
        }

        /// <summary>
        /// 获取奖项类别
        /// </summary>
        /// <returns></returns>
        public static SelectList GetPrizeType()
        {
            List<LotteryPrizes> list = LotteryPrizes.GetEntitysAll();
            LotteryPrizes prizes = new LotteryPrizes();
            prizes.PrizeLevel = "全部";
            list.Add(prizes);
            return ListToSelectList(list, "PrizeLevel", "PrizeLevel", "全部");
        }
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetRoles(string selVal)
        {
            List<B_Role> list = B_Role.GetEntitysByIsValid(1);
            B_Role role = new B_Role();
            role.ID = 0;
            role.RoleName = "无";
            list.Insert(0, role);
            return ListToSelectList(list, "ID", "RoleName", selVal);
        }
        /// <summary>
        /// 获取代理级别
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetC_UserType(string selVal)
        {
            List<C_UserType> list = C_UserType.GetEntityList("");

            C_UserType UserType = C_UserType.GetEntittyC_Type(Convert.ToInt32(selVal));
            return ListToSelectList(list, "Lever", "Name", UserType != null ? UserType.Lever.ToString() : "");
        }

        /// <summary>
        /// 获取代理级别 值为ID
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetC_UserTypeID(string selVal)
        {
            List<C_UserType> list = C_UserType.GetEntitysAll().ToList();
            list = list.FindAll(delegate(C_UserType p) { return p.Lever > int.Parse(selVal); }).OrderBy(m => m.Lever).ToList();
            return ListToSelectList(list, "Lever", "Name", selVal);
        }
        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetC_UserTypeAll(string selVal)
        {
            List<C_UserType> list = C_UserType.GetEntitysAll().ToList();
            list = list.OrderBy(m => m.Lever).ToList();
            C_UserType type = new C_UserType();
            type.Lever = 0;
            type.Name = "全部级别";
            list.Insert(0, type);
            return ListToSelectList(list, "Lever", "Name", selVal);
        }
        /// <summary>
        /// 获取低于某代理的所有级别
        /// </summary>
        /// <param name="selVal"></param>
        /// <param name="pUserID"></param>
        /// <returns></returns>
        public static SelectList GetC_UserTypeIDByPUser(string selVal,int pUserID,int UserID,bool andself=false)
        {
            List<C_UserType> list = C_UserType.GetLowerByUserID(pUserID, UserID, andself).ToList();
            list = list.OrderByDescending(m => m.Lever).ToList();
            return ListToSelectList(list, "Lever", "Name", selVal);
        }

        public static SelectList GetC_UserTypeIDByPUserAtAll(string selVal, int pUserID, int UserID, bool andself = false)
        {
            List<C_UserType> list = C_UserType.GetLowerByUserID(pUserID, UserID, andself).ToList();
            list = list.OrderByDescending(m => m.Lever).ToList();
            C_UserType item = new C_UserType();
            item.ID = 0;
            item.Name = "不限";
            list.Insert(0, item);
            return ListToSelectList(list, "Lever", "Name", selVal);
        }

        /// <summary>
        /// 获取代理级别 值为ID
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetPrizeType(string selVal)
        {
            List<Prize> list = Prize.GetEntitysAll().ToList();
            list = list.OrderBy(m => m.Lever).ToList();
            return ListToSelectList(list, "Lever", "Name", selVal);
        }


        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetSupplier(string selVal)
        {
            List<Supplier> list = Supplier.GetEntitysAll().ToList();
            list = list.OrderBy(m => m.ID).ToList();
            return ListToSelectList(list, "ID", "Name", selVal);
        }

        public static SelectList GetOrderNo(string selVal)
        {
            List<Order> list = Worder.GetNoID().ToList();
            list = list.OrderBy(m => m.ID).ToList();
            return ListToSelectList(list, "ID", "OrderNo", selVal);
        }
        /// <summary>
        /// 获取高于或等于自己的级别
        /// </summary>
        /// <param name="selVal"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static SelectList GetC_UserTypeHeighter(string selVal,int UserID)
        {
            List<C_UserType> list = C_UserType.GetC_UserTypeHeighter(UserID).ToList();
            list = list.OrderByDescending(m => m.Lever).ToList();
            return ListToSelectList(list, "Lever", "Name", selVal);
        }

        /// <summary>
        /// 获取自定义级别
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetC_UserTypeIDs(string selVal, List<C_UserType> list)
        {
            return ListToSelectList(list, "Lever", "Name", selVal);
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetProducts()
        {
            List<Product> list = Product.GetEntitysAll();
            return ListToSelectList(list, "ProductNumber", "ProductName", null);
        }
        public static SelectList GetProduct11()
        {
            List<Product> list = Product.GetEntitysAll();
            return ListToSelectList(list, "ProductName", "ProductName", null);
        }
        public static SelectList GetProductsname()
        {
            List<Product> list = Product.GetEntitysAll();
            return ListToSelectList(list, "ProductName", "ProductName", null);
        }
        public static SelectList GetProducts2()
        {
            List<Product> list = Product.GetEntitysAll();
            return ListToSelectList(list, "ProductNumber", "ProductNumber", null);
        }
        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetAuthorityCustomer()
        {
            List<C_User> list = C_User.GetAuthorityUser();
            return ListToSelectList(list, "UserName", "Name", null);
        }
        /// <summary>
        /// 获取快递列表
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetKDList(string selVal)
        {
            List<BasePostCode> list = BasePostCode.GetHaveAllBySort().ToList();
            return ListToSelectList(list, "ID", "PostName", selVal);
        }

        /// <summary>
        /// 栏目列表
        /// </summary>
        /// <param name="selVal"></param>
        /// <returns></returns>
        public static SelectList GetMainMenus(string selVal)
        {
            List<B_Menu> list = B_Menu.GetMainMenus();
            B_Menu role = new B_Menu();
            role.ID = 0;
            role.MenuName = "全部";
            list.Insert(0, role);
            return ListToSelectList(list, "ID", "MenuName", selVal);
        }

        public static SelectList GetSYSIntegralCodeArea(string selVal, string emptytext)
        {
            List<SYSIntegralCodeArea> list = SYSIntegralCodeArea.GetEntitysAll().ToList();
            SYSIntegralCodeArea item = new SYSIntegralCodeArea();
            item.ID = 0;
            item.AreaName = emptytext;
            list.Insert(0, item);
            return ListToSelectList(list, "ID", "AreaName", selVal);
        }
    }
}
