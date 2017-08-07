using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic;
using Domain.Entities;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Users = "sampling")]
    public class SamplingController : Controller
    {
        private DataManager _dataManager;

        public SamplingController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public ActionResult Index(GridSortOptions sort, int? page)
        {
            var directionsToSampling = _dataManager.Directions.GetDirectionsByDepartment("отбор проб");
            var samplingDirectionsViewModel = new List<SamplingDirectionViewModel>();

            foreach (Direction direction in directionsToSampling)
            {
                if (_dataManager.SelectedSamples.GetSelectedSampleByRegNumb(direction.SampleId) == null)
                {
                    Sample sample = _dataManager.Samples.GetSampleByNumber(direction.SampleId);
                    var samplingDirectionForModel = new SamplingDirectionViewModel
                                                        {
                                                            SampleId = direction.SampleId,
                                                            SampleName = sample.Name,
                                                            DirectionDate = direction.DirectionDate,
                                                            Applicant = sample.Applicant,
                                                            ContactPhone = sample.ContactPhone
                                                        };
                    samplingDirectionsViewModel.Add(samplingDirectionForModel);
                }
            }

            IEnumerable<SamplingDirectionViewModel> gridModel;

            if (sort.Column != null)
            {
                gridModel = samplingDirectionsViewModel.OrderBy(sort.Column, sort.Direction).AsPagination(page ?? 1, 30);
            }
            else
            {
                gridModel = samplingDirectionsViewModel.OrderBy("DirectionDate", SortDirection.Descending).AsPagination(page ?? 1, 30);
            }
            ViewBag.Sort = sort;

            return View(gridModel);
        }

        public ActionResult SelectSample(int id = 0)
        {
            var model = new SelectedSampleViewModel {SampleId = id};
            return View(model);
        }

        [HttpPost]
        public ActionResult SelectSample(SelectedSampleViewModel model)
        {
            if(ModelState.IsValid)
            {
                int employeeId = _dataManager.Employees.GetIdByProperties(model.EmployeeFirstName,
                                                                          model.EmployeeLastName,
                                                                          model.EmployeeMiddleName,
                                                                          model.EmployeePost,
                                                                          "отбор проб");
                if (employeeId == 0)
                {
                    _dataManager.Employees.CreateEmployee(model.EmployeeFirstName, model.EmployeeLastName,
                                                          model.EmployeeMiddleName, model.EmployeePost,
                                                          "отбор проб", null, null);

                    employeeId = _dataManager.Employees.GetIdByProperties(model.EmployeeFirstName,
                                                                          model.EmployeeLastName,
                                                                          model.EmployeeMiddleName,
                                                                          model.EmployeePost,
                                                                          "отбор проб");
                }

                int technicalRegulationId = _dataManager.TechnicalRegulations.GetIdByProperties(model.TrDesignation,
                                                                                                model.TrName);
                if(technicalRegulationId == 0)
                {
                    _dataManager.TechnicalRegulations.CreateTechnicalRegulation(model.TrDesignation, model.TrName, null);
                    technicalRegulationId = _dataManager.TechnicalRegulations.GetIdByProperties(model.TrDesignation,
                                                                                                model.TrName);
                }

                _dataManager.SelectedSamples.CreateSelectedSample(model.SampleId, model.Manufacturer,
                                                                  model.SamplingPlace, model.NumberOfSelectedSample,
                                                                  model.Units, DateTime.Now, technicalRegulationId,
                                                                  employeeId);
                return RedirectToAction("SelectedSamples");
            }
            return View(model);
        }

        public ActionResult SelectedSamples(GridSortOptions sort, int? page)
        {
            var selectedSamples = _dataManager.SelectedSamples.GetSelectedSamples();
            var samplesForGrid = new List<SelectedSamplesOutViewModel>();

            foreach (var selectedSample in selectedSamples)
            {
                Sample sample = _dataManager.Samples.GetSampleByNumber(selectedSample.RegistrationNumber);
                var sampleForGrid = new SelectedSamplesOutViewModel
                                        {
                                            RegistrationNumber = selectedSample.RegistrationNumber,
                                            SampleName = sample.Name,
                                            Applicant = sample.Applicant,
                                            SamplingDate = selectedSample.SamplingDate
                                        };
                samplesForGrid.Add(sampleForGrid);
            }

            IEnumerable<SelectedSamplesOutViewModel> gridModel;

            if (sort.Column != null)
            {
                gridModel = samplesForGrid.OrderBy(sort.Column, sort.Direction).AsPagination(page ?? 1, 30);
            }
            else
            {
                gridModel = samplesForGrid.OrderBy("SamplingDate", SortDirection.Descending).AsPagination(page ?? 1, 30);
            }
            ViewBag.Sort = sort;

            return View(gridModel);
        }

        public ActionResult SamplingAct(int id = 0)
        {
            if(id == 0)
            {
                return View("Error");
            }

            var sample = _dataManager.Samples.GetSampleByNumber(id);
            var selectedSample = _dataManager.SelectedSamples.GetSelectedSampleByRegNumb(id);
            var employee = _dataManager.Employees.GetEmployeeById(selectedSample.EmployeeId);
            var samplingTechnique =
                _dataManager.TechnicalRegulations.GetTechnicalRegulationById(selectedSample.SamplingTechnique);

            var model = new ActOfSamplingViewModel
                            {
                                SampleId = id,
                                SampleName = sample.Name,
                                SamplingDate = selectedSample.SamplingDate,
                                Applicant = sample.Applicant,
                                Manufacturer = selectedSample.Manufacturer,
                                EmployeeFirstName = employee.FirstName,
                                EmployeeLastName = employee.LastName,
                                EmployeeMiddleName = employee.MiddleName,
                                EmployeePost = employee.Post,
                                Number = selectedSample.Number,
                                Units = selectedSample.Units,
                                SamplingTechniqueDesignation = samplingTechnique.Designation,
                                SamplingTechniqueName = samplingTechnique.Name
                            };
            return View(model);
        }
    }
}