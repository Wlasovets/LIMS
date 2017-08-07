using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using Domain.Entities;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Users = "metrology")]
    public class MetrologyController : Controller
    {
        private DataManager dataManager;

        public MetrologyController(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        public ActionResult Index(GridSortOptions sort, int? page)
        {
            ViewBag.GagesGrid = PreparationGageGrid(sort, page);
            ViewBag.ExpirationOfVerification = ExpirationOfVerification();

            return View();
        }

        [HttpPost]
        public ActionResult Index(GageViewModel model, GridSortOptions sort, int? page)
        {
            if (ModelState.IsValid)
            {
                dataManager.Gages.CreateGage(model.Name, model.SerialNumber, "не поверен");
                return RedirectToAction("Index", "Metrology");
            }

            ViewBag.GagesGrid = PreparationGageGrid(sort, page);
            ViewBag.ExpirationOfVerification = ExpirationOfVerification();

            return View(model);
        }

        private List<string> ExpirationOfVerification()
        {
            IEnumerable<Certificate> certificates =
                dataManager.Certificates.GetCertificatesByExpirationOfVerification(DateTime.Now.AddDays(-7),
                                                                                   DateTime.Now);
            if (certificates != null && certificates.Any())
            {
                return
                    certificates.Select(certificate => dataManager.Gages.GetGageById(certificate.GageId).Name).ToList();
            }
            return null;
        }

        //Подготовка списка средств измерения для вывода на страницу (извлечение из репозитория, сортировка)
        private IEnumerable<GageGridViewModel> PreparationGageGrid(GridSortOptions sort, int? page)
        {
            IEnumerable<Gage> allGages = dataManager.Gages.GetGages();
            var gagesGridViewModel = new List<GageGridViewModel>();

            foreach (Gage gage in allGages)
            {
                var gageForGrid = new GageGridViewModel
                                      {
                                          GageId = gage.Id,
                                          GageName = gage.Name,
                                          SerialNumber = gage.SerialNumber,
                                          State = gage.State
                                      };
                
                IEnumerable<Certificate> certificates = dataManager.Certificates.GetCertificatesByGageId(gage.Id);
                if (certificates != null && certificates.Any())
                {
                    gageForGrid.EndVerification = certificates.Max(x => x.EndVerificationDate).ToShortDateString();
                }
                else
                {
                    gageForGrid.EndVerification = "не поверен";
                }
                
                gagesGridViewModel.Add(gageForGrid);
            }

            IEnumerable<GageGridViewModel> model = gagesGridViewModel.OrderBy(sort.Column, sort.Direction);
                ViewBag.Sort = sort;

            return model;
        }

        public ActionResult ChangeState(int id = 0)
        {
            if(id != 0)
            {
                return View(dataManager.Gages.GetGageById(id));
            }
            return RedirectToAction("Index", "Metrology");
        }

        [HttpPost]
        public ActionResult ChangeState(Gage model, int id = 0)
        {
            Gage gage = dataManager.Gages.GetGageById(id);
            gage.State = model.State;
            dataManager.Gages.SaveGage(gage);

            return RedirectToAction("Index", "Metrology");
        }

        public ActionResult Verification(int id = 0)
        {
            if (id == 0)
                return RedirectToAction("Index", "Metrology");

            var model = new CertificateViewModel
                            {
                                GageId = id,
                                GageName = dataManager.Gages.GetGageById(id).Name,
                                SerialNumber = dataManager.Gages.GetGageById(id).SerialNumber
                            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Verification(CertificateViewModel model)
        {
            if (ModelState.IsValid)
            {
                dataManager.Certificates.CreateCertificate(model.GageId, model.CertificateNumber, model.VerificationDate,
                                                           model.EndVerificationDate);

                if(model.EndVerificationDate > DateTime.Now)
                {
                    dataManager.Gages.ChangeState(model.GageId, "поверен");
                }

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult TechnicalRegulations(GridSortOptions sort)
        {
            var model = new TechnicalRegulationsViewModel
                            {
                                TechnicalRegulations =
                                    dataManager.TechnicalRegulations.GetTechnicalRegulations()
                            };

            model.TechnicalRegulations = model.TechnicalRegulations.OrderBy(sort.Column, sort.Direction);
            ViewBag.Sort = sort;

            return View(model);
        }

        [HttpPost]
        public ActionResult TechnicalRegulations(TechnicalRegulationsViewModel model, GridSortOptions sort)
        {
            if(ModelState.IsValid)
            {
                dataManager.TechnicalRegulations.CreateTechnicalRegulation(model.TrDesignation, model.TrName, model.TrNote);
                return RedirectToAction("TechnicalRegulations");
            }

            model.TechnicalRegulations = dataManager.TechnicalRegulations.GetTechnicalRegulations();
            model.TechnicalRegulations = model.TechnicalRegulations.OrderBy(sort.Column, sort.Direction);
            ViewBag.Sort = sort;

            return View(model);
        }

        public ActionResult Report()
        {
            var gages = new List<Gage> {new Gage {Name = "Все", Id = 0}};
            gages.AddRange(dataManager.Gages.GetGages().ToList());
            var model = new MetrologyReportViewModel {Gages = gages};

            return View("ReportGeneration", model);
        }

        [HttpPost]
        public ActionResult Report(MetrologyReportViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.FirstDate <= model.LastDate)
                {
                    var resultGroups = dataManager.Results.GetResults().Where(x => x.ResultDate >= model.FirstDate && x.ResultDate <= model.LastDate).GroupBy(x => x.GageId);

                    if (model.SelectedGage == 0)
                    {
                        model.GageGroups = (from resultGroup in resultGroups
                                          let gage = dataManager.Gages.GetGageById(resultGroup.Key)
                                          select new GageGroupeForMetrologyReport
                                                     {
                                                         GageName = gage.Name,
                                                         SerialNumber = gage.SerialNumber,
                                                         CountOfResults = resultGroup.Count()
                                                     }).ToList();
                    }
                    else
                    {
                        var gageGroup = new GageGroupeForMetrologyReport
                                            {
                                                GageName = dataManager.Gages.GetGageById(model.SelectedGage).Name,
                                                SerialNumber = dataManager.Gages.GetGageById(model.SelectedGage).SerialNumber
                                            };
                        var resultGroup = resultGroups.FirstOrDefault(x => x.Key == model.SelectedGage);
                        gageGroup.CountOfResults = resultGroup != null ? resultGroup.Count() : 0;
                        model.GageGroups = new List<GageGroupeForMetrologyReport> {gageGroup};
                    }
                    return View(model);
                }
                ModelState.AddModelError("", "Дата начала отчетного периода более поздняя, чем дата конца отчетного периода");
            }
            var gages = new List<Gage> { new Gage { Name = "Все", Id = 0 } };
            gages.AddRange(dataManager.Gages.GetGages().ToList());
            model.Gages = gages;
            return View("ReportGeneration", model);
        }
    }
}
