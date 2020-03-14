using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeModels;
using Newtonsoft.Json;

namespace AgentMobile.Controllers
{
    public class testController : Controller
    {

        public class Circle { 
            public Circle(double radius) {
                Radius = radius;
            }
            public double Radius;
            public double GetPerimeter() 
            { 
                return 2 * Math.PI * Radius;
            } 
            public double GetArea()
            { 
                return Math.PI * Math.Pow(Radius, 2); } 
        }


        public ActionResult Index()
        {


            Circle c = new Circle(1.0); 
            Console.WriteLine(c.GetPerimeter()); 
            Console.WriteLine(c.GetArea()); 
            Console.ReadLine();    








            return View();
        }

    }
}
