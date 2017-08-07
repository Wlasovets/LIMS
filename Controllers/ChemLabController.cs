using System;
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
    [Authorize(Users = "chemLab")]
    public class ChemLabController : Controller
    {
        private DataManager _dataManager;

        public ChemLabController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public ActionResult Index(GridSortOptions sort, int? page)
        {
            var directions = _dataManager.Directions.GetNotCompletedDirectionsByDepartment("химлаборатория");
            var directionsForGrid = new List<DirectionToResearchViewModel>();

            foreach (var direction in directions)
            {
                Sample sample = _dataManager.Samples.GetSampleByNumber(direction.SampleId);
                var directionForGrid = new DirectionToResearchViewModel
                                           {
                                               SampleId = direction.SampleId,
                                               SampleName = sample.Name,
                                               DirectionDate = direction.DirectionDate,
                                               Applicant = sample.Applicant
                                           };
                directionsForGrid.Add(directionForGrid);
            }
            IEnumerable<DirectionToResearchViewModel> gridModel;

            if (sort.Column != null)
            {
                gridModel = directionsForGrid.OrderBy(sort.Column, sort.Direction).AsPagination(page ?? 1, 30);
            }
            else
            {
                gridModel = directionsForGrid.OrderBy("DirectionDate", SortDirection.Descending).AsPagination(page ?? 1, 30);
            }
            ViewBag.Sort = sort;

            return View(gridModel);
        }

        public ActionResult ResearchSample(int id = 0)
        {
            var model = new ResearchSampleViewModel
                            {
                                SampleId = id,
                                SampleName = _dataManager.Samples.GetSampleByNumber(id).Name
                            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ResearchSample(ResearchSampleViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //Получение внешних ключей для записи в таблицу результатов

            int employeeId = _dataManager.Employees.AddEmployee(model.EmployeeFirstName, model.EmployeeLastName,
                                                                model.EmployeeMiddleName, model.EmployeePost,
                                                                "химлаборатория");
            int gageId = _dataManager.Gages.AddGage(model.GageName, model.GageSerialNumber);
            int techniqueOfTestId =
                _dataManager.TechnicalRegulations.AddTechnicalRegulation(model.TechniqueOfTestDesignation,
                                                                         model.TechniqueOfTestName);
            int indicatorId = _dataManager.Indicators.AddIndicator(model.Indicator, model.Units);

            //Получение внешнего ключа таблицы "Допустимые уровни"

            int normativeDocumentId =
                _dataManager.TechnicalRegulations.AddTechnicalRegulation(model.NormativeDocumentDesignation,
                                                                         model.NormativeDocumentName);
            int levelId = _dataManager.AdmissibleLevels.AddAdmissibleLevel(model.AdmissibleLever, indicatorId,
                                                                           normativeDocumentId);
            //Добавление данных в таблицу результатов

            int resultId = _dataManager.Results.AddResult(model.Result, DateTime.Now, indicatorId, levelId,
                                                          techniqueOfTestId, gageId, model.SampleId, employeeId);
            Direction direction = _dataManager.Directions.GetDirectionBySampleIdAndDepartment(model.SampleId,
                                                                                              "химлаборатория");
            if (direction != null)
            {
                direction.State = "выполнено";
                _dataManager.Directions.SaveDirection(direction);
            }


            //Добавление условий исследований

            indicatorId = _dataManager.Indicators.AddIndicator("температура", "градус Цельсия");
            _dataManager.Conditions.CreateCondition(DateTime.Now, model.Temperature, indicatorId, resultId);
            indicatorId = _dataManager.Indicators.AddIndicator("влажность", "процент");
            _dataManager.Conditions.CreateCondition(DateTime.Now, model.Humidity, indicatorId, resultId);
            indicatorId = _dataManager.Indicators.AddIndicator("атмосферное давление", "килопаскаль");
            _dataManager.Conditions.CreateCondition(DateTime.Now, model.Pressure, indicatorId, resultId);
            indicatorId = _dataManager.Indicators.AddIndicator("мощность экспозиционной дозы", "микрозиверт в час");
            _dataManager.Conditions.CreateCondition(DateTime.Now, model.Radiation, indicatorId, resultId);

            return RedirectToAction("Results");
        }

        public ActionResult Results(GridSortOptions sort, int? page)
        {
            var results = _dataManager.Results.GetResultsByDepartment("химлаборатория");
            var resultsForGrid = new List<ResearchedSampelViewModel>();

            foreach (var result in results)
            {
                var resultForGrid = new ResearchedSampelViewModel
                                        {
                                            ResultId = result.Id,
                                            SampleId = result.SampleId,
                                            SampleName = _dataManager.Samples.GetSampleByNumber(result.SampleId).Name,
                                            ResultDate = result.ResultDate,
                                            ResultValue = result.ResultValue,
                                            LevelValue =
                                                _dataManager.AdmissibleLevels.GetAdmissibleLevelById(
                                                    result.AdmissibleLevelId).LevelValue,
                                            Units = _dataManager.Indicators.GetIndicatorById(result.IndicatorId).Units
                                        };

                resultsForGrid.Add(resultForGrid);
            }

            IEnumerable<ResearchedSampelViewModel> gridModel;

            if (sort.Column != null)
            {
                gridModel = resultsForGrid.OrderBy(sort.Column, sort.Direction).AsPagination(page ?? 1, 30);
            }
            else
            {
                gridModel = resultsForGrid.OrderBy("ResultDate", SortDirection.Descending).AsPagination(page ?? 1, 30);
            }
            ViewBag.Sort = sort;

            return View(gridModel);
        }

        public ActionResult Protocol(int id = 0)
        {
            Result result = _dataManager.Results.GetResultById(id);

            if (result == null)
                return View("Error");

            Sample sample = _dataManager.Samples.GetSampleByNumber(result.SampleId);
            TechnicalRegulation techniqueOfTests =
                _dataManager.TechnicalRegulations.GetTechnicalRegulationById(result.TechniqueOfTestsId);
            Indicator indicator = _dataManager.Indicators.GetIndicatorById(result.IndicatorId);
            AdmissibleLevel admissibleLevel =
                _dataManager.AdmissibleLevels.GetAdmissibleLevelById(result.AdmissibleLevelId);
            TechnicalRegulation technicalRegulation =
                _dataManager.TechnicalRegulations.GetTechnicalRegulationById(admissibleLevel.TechnicalRegulationId);
            Gage gage = _dataManager.Gages.GetGageById(result.GageId);
            Indicator temperatureIndicator =
                _dataManager.Indicators.GetIndicators().FirstOrDefault(x => x.Name == "температура");
            Indicator humidityIndicator =
                _dataManager.Indicators.GetIndicators().FirstOrDefault(x => x.Name == "влажность");
            Indicator pressureIndicator =
                _dataManager.Indicators.GetIndicators().FirstOrDefault(x => x.Name == "атмосферное давление");
            Indicator radiationIndicator =
                _dataManager.Indicators.GetIndicators().FirstOrDefault(x => x.Name == "мощность экспозиционной дозы");


            var model = new ProtocolViewModel
            {
                SampleId = sample.RegistrationNumber,
                SampleName = sample.Name,
                ResultDate = result.ResultDate.ToShortDateString(),
                Applicant = sample.Applicant,
                Temperature =
                    _dataManager.Conditions.GetConditions().FirstOrDefault(
                        x => x.ResultId == result.Id && x.IndicatorId == temperatureIndicator.Id).
                    ConditionValue,
                Humidity =
                    _dataManager.Conditions.GetConditions().FirstOrDefault(
                        x => x.ResultId == result.Id && x.IndicatorId == humidityIndicator.Id).
                    ConditionValue,
                Pressure =
                    _dataManager.Conditions.GetConditions().FirstOrDefault(
                        x => x.ResultId == result.Id && x.IndicatorId == pressureIndicator.Id).
                    ConditionValue,
                Radiation =
                    _dataManager.Conditions.GetConditions().FirstOrDefault(
                        x => x.ResultId == result.Id && x.IndicatorId == radiationIndicator.Id).
                    ConditionValue,
                TechniqueOfTestsDesignation = techniqueOfTests.Designation,
                TechniqueOfTestsName = techniqueOfTests.Name,
                ResultValue = result.ResultValue,
                LevelValue = admissibleLevel.LevelValue,
                Units = indicator.Units,
                TechnicalRegulationDesignation = technicalRegulation.Designation,
                TechnicalRegulationName = technicalRegulation.Name,
                GageName = gage.Name,
                SerialNumber = gage.SerialNumber,
                IndicatorName = indicator.Name
            };

            SelectedSample selectedSample =
                _dataManager.SelectedSamples.GetSelectedSampleByRegNumb(sample.RegistrationNumber);

            if (selectedSample != null)
            {
                TechnicalRegulation samplingTechnique =
                    _dataManager.TechnicalRegulations.GetTechnicalRegulationById(selectedSample.SamplingTechnique);
                model.Manufacturer = selectedSample.Manufacturer;
                model.SamplingDate = selectedSample.SamplingDate.ToShortDateString();
                model.SamplingTechniqueDesignation = samplingTechnique.Designation;
                model.SamplingTechniqueName = samplingTechnique.Name;
            }
            else
            {
                model.Manufacturer = "-";
                model.SamplingDate = "-";
                model.SamplingTechniqueDesignation = "-";
                model.SamplingTechniqueName = "";
            }

            return View(model);
        }
    }
}
