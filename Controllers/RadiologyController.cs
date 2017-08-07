using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic;
using Domain.Entities;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Users = "radiology")]
    public class RadiologyController : Controller
    {
        private DataManager _dataManager;

        public RadiologyController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public ActionResult Index(GridSortOptions sort, int? page)
        {
            var directions = _dataManager.Directions.GetNotCompletedDirectionsByDepartment("радиология");
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
                                                                "радиология");
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
                                                                                              "радиология");
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
            var results = _dataManager.Results.GetResultsByDepartment("радиология");
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

            if(selectedSample != null)
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

        public ActionResult Robotron()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Robotron(RobotronViewModel model)
        {
            model.Activity = null;
            model.AbsoluteError = null;

            if (ModelState.IsValid)
            {
                const double detectorSensitivity = 0.053; //Чувствительность детектора
                const double quantumExit = 0.851; //Квантовый выход
                double activity = ((model.NumberOfDecaysSample - model.NumberOfDecaysBackground)/model.SampleMass*
                                   detectorSensitivity*quantumExit);
                double absoluteError = activity*0.2;

                model.Activity = activity.ToString("#.00");
                model.AbsoluteError = absoluteError.ToString("#.00");
            }

            return View(model);
        }

        public ActionResult Ern()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ern(ErnViewModel model)
        {
            if(ModelState.IsValid)
            {
                double ernActivity = model.RadiumActivity + 1.31*model.ThoriumActivity + 0.085*model.PotassiumActivity;
                double ernError =
                    Math.Sqrt(model.RadiumError*model.RadiumError + 1.7*model.ThoriumError*model.ThoriumError +
                              0.007*model.PotassiumError*model.PotassiumError);
                model.Result = (ernActivity + ernError).ToString("#.00");
            }

            return View(model);
        }

        public ActionResult Umf()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Umf(UmfViewModel model)
        {
            if (ModelState.IsValid)
            {
                const double EffAlpha = 0.019;
                const double EffBeta = 0.234;
                const double Ktr = 0.759;

                //Расчет активности
                double amountAlphaDecaysOfSample =
                    (double)
                    (model.AmountAlphaDecaysOfSample1 + model.AmountAlphaDecaysOfSample2 +
                     model.AmountAlphaDecaysOfSample3)/3;
                double amountBetaDecaysOfSample =
                    (double)
                    (model.AmountBetaDecaysOfSample1 + model.AmountBetaDecaysOfSample2 +
                     model.AmountBetaDecaysOfSample3)/3;
                double amountOfBackgroundAlphaDecays =
                    (double)
                    (model.AmountOfBackgroundAlphaDecays1 + model.AmountOfBackgroundAlphaDecays2 +
                     model.AmountOfBackgroundAlphaDecays3)/3;
                double amountOfBackgroundBetaDecays =
                    (double)
                    (model.AmountOfBackgroundBetaDecays1 + model.AmountOfBackgroundBetaDecays2 +
                     model.AmountOfBackgroundBetaDecays3)/3;
                double amountAlphaDecays = amountAlphaDecaysOfSample - amountOfBackgroundAlphaDecays;
                double amountBetaDecays = amountBetaDecaysOfSample - amountOfBackgroundBetaDecays;
                double alphaActivity = amountAlphaDecays/(model.Time*EffAlpha);
                double betaActivity = (amountBetaDecays - amountAlphaDecays*Ktr)/(model.Time*EffBeta);
                model.AlphaActivity = Math.Round(alphaActivity, 4).ToString("0.0000");
                model.BetaActivity = Math.Round(betaActivity, 4).ToString("0.0000");

                //Расчет неопределенности
                double deviationAlphaDecaysOfSample =
                    (Math.Pow(model.AmountAlphaDecaysOfSample1 - amountAlphaDecaysOfSample, 2) +
                     Math.Pow(model.AmountAlphaDecaysOfSample2 - amountAlphaDecaysOfSample, 2) +
                     Math.Pow(model.AmountAlphaDecaysOfSample3 - amountAlphaDecaysOfSample, 2))/6;
                double deviationBetaDecaysOfSample =
                    (Math.Pow(model.AmountBetaDecaysOfSample1 - amountBetaDecaysOfSample, 2) +
                     Math.Pow(model.AmountBetaDecaysOfSample2 - amountBetaDecaysOfSample, 2) +
                     Math.Pow(model.AmountBetaDecaysOfSample3 - amountBetaDecaysOfSample, 2))/6;
                double deviationAlphaDecaysOfBackground =
                    (Math.Pow(model.AmountOfBackgroundAlphaDecays1 - amountOfBackgroundAlphaDecays, 2) +
                     Math.Pow(model.AmountOfBackgroundAlphaDecays2 - amountOfBackgroundAlphaDecays, 2) +
                     Math.Pow(model.AmountOfBackgroundAlphaDecays3 - amountOfBackgroundAlphaDecays, 2))/6;
                double deviationBetaDecaysOfBackground =
                    (Math.Pow(model.AmountOfBackgroundBetaDecays1 - amountOfBackgroundBetaDecays, 2) +
                     Math.Pow(model.AmountOfBackgroundBetaDecays2 - amountOfBackgroundBetaDecays, 2) +
                     Math.Pow(model.AmountOfBackgroundBetaDecays3 - amountOfBackgroundBetaDecays, 2))/6;

                //Стандартная неопределенность
                double standardUncertaintyForAlpha =
                    Math.Sqrt(deviationAlphaDecaysOfSample + deviationAlphaDecaysOfBackground);
                double standardUncertaintyForBeta =
                    Math.Sqrt(deviationBetaDecaysOfSample + deviationBetaDecaysOfBackground +
                              deviationAlphaDecaysOfSample*Ktr);

                //Расширенная неопределенность
                double absoluteUncertaintyForAlpha = (2*standardUncertaintyForAlpha)/(model.Time*EffAlpha);
                double absoluteUncertaintyForBeta = (2*standardUncertaintyForBeta)/(model.Time*EffBeta);
                model.AbsoluteUncertaintyForAlpha = Math.Round(absoluteUncertaintyForAlpha, 4).ToString("0.0000");
                model.AbsoluteUncertaintyForBeta = Math.Round(absoluteUncertaintyForBeta, 4).ToString("0.0000");
            }

            return View(model);
        }
    }
}
