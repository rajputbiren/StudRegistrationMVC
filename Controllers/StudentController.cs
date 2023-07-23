using StudRegistrationMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Data.Entity.SqlServer;
using System.Globalization;

namespace StudRegistrationMVC.Controllers
{
    public class StudentController : Controller
    {
        int PageSize = 2;


        // GET: Student
        [Route("StudentIndex")]
        public ActionResult Index()
        {
            return View();
        }
        [Route("StudentList")]
        public ActionResult StudentIndex()
        {
            ViewBag.Message = "Your contact page.";
            List<Student> lstStudent = new List<Student>();
            
            using (PracticeApplicationDBEntities ctx= new PracticeApplicationDBEntities())
            {
                lstStudent = ctx.Students.ToList();
            }
            return View(lstStudent);
        }

        public ActionResult Save(int StdId)
        {
            Student objStd = new Student();
            PracticeApplicationDBEntities ctx1 = new PracticeApplicationDBEntities();
            ViewBag.Country = ctx1.Countries.ToList();
            ViewBag.State = ctx1.States.ToList();
            ViewBag.City = ctx1.Cities.ToList();

            StudentViewModel objSVM = new StudentViewModel();
            if (StdId > 0)
            {
                using (PracticeApplicationDBEntities ctx = new PracticeApplicationDBEntities())
                {
                    objSVM = (from objS in ctx.Students
                              where objS.StudentId == StdId
                              select new StudentViewModel()
                              {
                                  StudentId = objS.StudentId,
                                  StudentName = objS.StudentName,
                                  Email = objS.Email,
                                  Phone = objS.Phone,
                                  CountryId= objS.CountryId,
                                  StateId=objS.StateId,
                                  CityId= objS.CityId,
                                  //City = objS.City,
                                  Address = objS.Address,
                                  DOB = objS.DOB.HasValue ? objS.DOB.Value : default(DateTime),
                                  Gender = objS.Gender,
                                  IsActive = objS.IsActive
                              }).FirstOrDefault();
                   }
            }
            return View(objSVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(StudentViewModel objStd)
        {
            if (ModelState.IsValid)
            {
                Student objTempStd = new Student();
                //objTempStd.StudentId = objStd.StudentId;
                objTempStd.StudentName = objStd.StudentName;
                objTempStd.Email = objStd.Email;
                objTempStd.Phone = objStd.Phone;
                //objTempStd.City = objStd.City;
                objTempStd.CountryId  = objStd.CountryId;
                objTempStd.StateId = objStd.StateId;
                objTempStd.CityId = objStd.CityId;
                objTempStd.Address = objStd.Address;
                objTempStd.DOB = Convert.ToDateTime(objStd.DOB);
                objTempStd.Gender = objStd.Gender;
                objTempStd.IsActive = objStd.IsActive;

                using (PracticeApplicationDBEntities ctx = new PracticeApplicationDBEntities())
                {
                    if (objStd != null && objStd.StudentId > 0)
                    {
                        objTempStd.StudentId = objStd.StudentId;
                        ctx.Entry(objTempStd).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //ctx.Entry(objTempStd).State = System.Data.Entity.EntityState.Added;
                        ctx.Students.Add(objTempStd);
                        ctx.SaveChanges();
                    }
                }
                return RedirectToAction("StudentIndex");
            }
            else
            {
                return View(objStd);
            }
            
        }
        
        public ActionResult IndexWithPagging(string Sorting_Order, string Search_String, string CurrentFilter_Value, int? Page_No)
        {
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingId = String.IsNullOrEmpty(Sorting_Order) ? "Id_Descending" : "Id_Ascending";
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            //ViewBag.SortingDate = Sorting_Order == "Date_Enroll" ? "Date_Description" : "Date";
            

            if (Search_String != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_String = CurrentFilter_Value;
            }
            ViewBag.CurrentFilterValue = Search_String;

            List<Student> lstStudent = new List<Student>();

            using (PracticeApplicationDBEntities ctx = new PracticeApplicationDBEntities())
            {
                //lstStudent = ctx.Students.ToList();
                if (Search_String != null && Search_String.Length > 0)
                {
                    lstStudent = ctx.Students.Where(stu => stu.StudentName.ToUpper().Contains(Search_String.ToUpper())).ToList();
                }
                else
                {
                    lstStudent = ctx.Students.ToList();
                }
            }

            switch (Sorting_Order)
            {
                case "Name_Description":
                    lstStudent = lstStudent.OrderByDescending(stu => stu.StudentName).ToList();
                    break;
                case "Id_Ascending":
                    lstStudent = lstStudent.OrderBy(stu => stu.StudentId).ToList();
                    break;
                case "Id_Descending":
                    lstStudent = lstStudent.OrderByDescending(stu => stu.StudentId).ToList();
                    break;
                //case "Date_Enroll":
                //    students = students.OrderBy(stu => stu.EnrollmentDate);
                //    break;
                //case "Date_Description":
                //    students = students.OrderByDescending(stu => stu.EnrollmentDate);
                //    break;
                default:
                    lstStudent = lstStudent.OrderBy(stu => stu.StudentName).ToList();
                    break;
            }

            int CurrentPageNo = Page_No ?? 1;
            return View(lstStudent.ToPagedList(CurrentPageNo, PageSize) );
        }

        public ActionResult IndexWithJQGrid()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetStudentDetails()
        {
            List<Student> lstStudent = new List<Student>();

            using (PracticeApplicationDBEntities ctx = new PracticeApplicationDBEntities())
            {
                lstStudent = ctx.Students.ToList();
            }
            var jsonData = new
            {
                total = 1,
                page = 1,
                records = lstStudent.Count,
                //rows = JsonConvert.SerializeObject(lstStudent)
                rows = (
                  from std in lstStudent
                  select new
                  {
                      id = std.StudentId,
                      cell = new string[] 
                      {
                          std.StudentId.ToString(),
                          std.StudentName.ToString(),
                          std.Email.ToString(),
                          std.Phone.ToString(),
                          //std.City.ToString()
                      }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SimpleIndexWithJQGrid()
        {
            return View();
        }
        
        public JsonResult GetStudents()
        {
            DataTable dt = new DataTable();

            PracticeApplicationDBEntities ctx = new PracticeApplicationDBEntities();
            //var data = (from i in ctx.Students
            //           select new
            //           {
            //               StudentId = i.StudentId,
            //               StudentName = i.StudentName,
            //               Email= i.Email,
            //               Phone = i.Phone,
            //               City = i.City
            //           }).ToArray();

            //return Json(data, JsonRequestBehavior.AllowGet);
            

            using (ctx)
            {
                var studentList = ctx.Students;
                //var studentList = ctx.Students.Select(
                //    t => new
                //    {
                //        t.StudentId,
                //        t.StudentName,
                //        t.Email,
                //        t.Phone,
                //        t.City
                //    });
                //return Json(new { rows = employeeList }, JsonRequestBehavior.AllowGet);

                //var json2 = new
                //{
                //    total = 1,
                //    page,
                //    records = studentList.Count(),
                //    rows = studentList.ToList()
                //};
                //return Json(json2, JsonRequestBehavior.AllowGet);

                return Json(new { rows = studentList.ToList() }, JsonRequestBehavior.AllowGet);
            }

        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        [HttpGet]
        public JsonResult GetCountry()
        {
            //List<Country> StateList = new List<Country>();
            using (PracticeApplicationDBEntities ctx = new PracticeApplicationDBEntities())
            {
               var StateList = ctx.Countries.Select(c=> new { CountryId= c.CountryId,CountryName= c.CountryName }).ToList();
               return Json(StateList, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetStates(int CountryId)
        {
            //List<State> StateList = new List<State>();
            using (PracticeApplicationDBEntities ctx = new PracticeApplicationDBEntities())
            {
              var  StateList = ctx.States.Where(c => c.CountryId == CountryId).Select(c=>new { StateId= c.StateId,StateName= c.StateName}).ToList();
              return Json(StateList, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCities(int StateId)
        {
            //List<Country> StateList = new List<Country>();
            using (PracticeApplicationDBEntities ctx = new PracticeApplicationDBEntities())
            {
                var StateList = ctx.Cities.Where(c=>c.StateId==StateId).Select(c => new { CityId = c.CityId, CityName = c.CityName }).ToList();
                return Json(StateList, JsonRequestBehavior.AllowGet);
            }
        }
        

    }
}