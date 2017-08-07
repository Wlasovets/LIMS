using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic;
using Domain.Entities;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;
using MvcContrib.Sorting;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Users = "window")]
    public class WindowController : Controller
    {
        private DataManager dataManager;

        public WindowController(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        public ActionResult Index(GridSortOptions sort, int ? page)
        {
            IEnumerable<Sample> model = dataManager.Samples.GetSamples();
            if (model == null)
                return View("Error");

            if(sort.Column != null)
            {
                model = model.OrderBy(sort.Column, sort.Direction).AsPagination(page ?? 1, 30);
            }
            else
            {
                model = model.OrderBy("DateOfRegistration", SortDirection.Descending).AsPagination(page ?? 1, 30);
            }
            ViewBag.Sort = sort;

            return View(model);
        }

        public ActionResult RegistrationSample()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrationSample(RegistrationSampleViewModel model)
        {
            if (ModelState.IsValid)
            {
                dataManager.Samples.CreateSample(model.Name, model.Applicant, model.PhoneNumber);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Direct(int id = 0)
        {
            if (dataManager.Samples.GetSampleByNumber(id) != null)
            {
                ViewBag.SampleId = id;
                ViewBag.SampleName = dataManager.Samples.GetSampleByNumber(id).Name;
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Direct(DirectViewModel model)
        {
            if(dataManager.Samples.GetSampleByNumber(model.SampleId) == null)
                return RedirectToAction("Index");

            if ((!dataManager.Directions.IsSampleDirect(model.SampleId, "отбор проб")) && model.DirectToSelect)
                dataManager.Directions.CreateDirection(model.SampleId, "отбор проб", "не выполнено");
            if ((!dataManager.Directions.IsSampleDirect(model.SampleId, "токсикология")) && model.DirectToToxicology)
                dataManager.Directions.CreateDirection(model.SampleId, "токсикология", "не выполнено");
            if ((!dataManager.Directions.IsSampleDirect(model.SampleId, "бактериология")) && model.DirectToBacteriology)
                dataManager.Directions.CreateDirection(model.SampleId, "бактериология", "не выполнено");
            if ((!dataManager.Directions.IsSampleDirect(model.SampleId, "химлаборатория")) && model.DirectToChemicalLab)
                dataManager.Directions.CreateDirection(model.SampleId, "химлаборатория", "не выполнено");
            if ((!dataManager.Directions.IsSampleDirect(model.SampleId, "радиология")) && model.DirectToRadiology)
                dataManager.Directions.CreateDirection(model.SampleId, "радиология", "не выполнено");

            return RedirectToAction("Directions");
        }


        public ActionResult Directions(GridSortOptions sort, int? page)
        {
            var directions = dataManager.Directions.GetDirections();

            if (directions == null || !directions.Any())
                return RedirectToAction("Index");

            var directionViewModels = new List<DirectionViewModel>();
            foreach (var direction in directions)
            {
                DirectionViewModel directionViewModel = new DirectionViewModel
                                               {
                                                   SampleId = direction.SampleId,
                                                   Department = direction.Department,
                                                   DirectDate = direction.DirectionDate,
                                                   SampleName =
                                                       dataManager.Samples.GetSampleByNumber(direction.SampleId).Name,
                                                   State = direction.State
                                               };
                directionViewModels.Add(directionViewModel);
            }

            IEnumerable<DirectionViewModel> directionGrid;

            if (sort.Column != null)
            {
                directionGrid = directionViewModels.OrderBy(sort.Column, sort.Direction).AsPagination(page ?? 1, 30);
            }
            else
            {
                directionGrid = directionViewModels.OrderBy("DirectDate", SortDirection.Descending).AsPagination(page ?? 1, 30);
            }
            ViewBag.Sort = sort;

            return View(directionGrid);
        }

        public ActionResult Employees(GridSortOptions sort)
        {
            ViewBag.Employees = SortEmployeesForGrid(sort);
            return View();
        }

        [HttpPost]
        public ActionResult Employees(RegistrationViewModel model, GridSortOptions sort)
        {
            ViewBag.Employees = SortEmployeesForGrid(sort);

            if (ModelState.IsValid)
            {
                if (dataManager.Employees.GetEmployeeByLogin(model.Login) == null)
                {
                    dataManager.Employees.CreateEmployee(model.FirstName, model.LastName, model.MiddleName, model.Post,
                                                         model.Department, model.Login, model.Password);
                    return RedirectToAction("Employees");
                }
                ModelState.AddModelError("", "Пользователь с таким логином уже существует.");
            }
            return View(model);
        }

        private IEnumerable<Employee> SortEmployeesForGrid(GridSortOptions sort)
        {
            IEnumerable<Employee> employees =
                dataManager.Employees.GetEmployees().Where(x => x.Login != null).Select(x => x);

            if (sort.Column != null)
            {
                employees = employees.OrderBy(sort.Column, sort.Direction);
                ViewBag.Sort = sort;
            }
            return employees;
        }

        public ActionResult DeleteLogin(int id = 0)
        {
            dataManager.Employees.DeleteLoginById(id);
            return RedirectToAction("Employees");
        }

        public ActionResult Report()
        {
            ViewBag.Departments = DepartmentRepository.GetDepartments();
            return View("GenerateReport");
        }

        [HttpPost]
        public ActionResult Report(GenerateReportViewModel introModel)
        {
            if(ModelState.IsValid)
            {
                if (introModel.LastDate >= introModel.FirstDate)
                {
                    IEnumerable<IGrouping<int, Result>> results;
                    if (introModel.Department.Name == "все")
                    {
                        results =
                            dataManager.Results.GetResults().Where(
                                x => x.ResultDate >= introModel.FirstDate && x.ResultDate <= introModel.LastDate).
                                GroupBy(x => x.IndicatorId);
                    }
                    else
                    {
                        results =
                            dataManager.Results.GetResultsByDepartment(introModel.Department.Name).Where(
                                x => x.ResultDate >= introModel.FirstDate && x.ResultDate <= introModel.LastDate).
                                GroupBy(x => x.IndicatorId);
                    }
                    var outModel = new WindowReportViewModel {GenerateReport = introModel};
                    var countResults = results.Select(resultGroup => new CountOfResultsViewModel
                                                                         {
                                                                             Indicator =
                                                                                 dataManager.Indicators.GetIndicatorById
                                                                                 (resultGroup.Key).Name,
                                                                             Count = resultGroup.Count()
                                                                         }).ToList();

                    outModel.CountOfResults = countResults;
                    return View(outModel);
                }
                ModelState.AddModelError("", "Дата начала отчетного периода более поздняя, чем дата конца отчетного периода");
            }
            ViewBag.Departments = DepartmentRepository.GetDepartments();
            return View("GenerateReport", introModel);
        }
    }
}
