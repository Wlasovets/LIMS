using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Implementations;
using BusinessLogic.Interfaces;
using Domain;
using Ninject;

namespace Web
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        //Извлекаем экземпляр контроллера для заданного контекста запроса и типа контроллера
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController) ninjectKernel.Get(controllerType);
        }

        //Определяем все привязки
        private void AddBindings()
        {
            ninjectKernel.Bind<IAdmissibleLevelsRepository>().To<EFAdmissibleLevelsRepository>();
            ninjectKernel.Bind<ICertificatesRepository>().To<EFCertificatesRepository>();
            ninjectKernel.Bind<IConditionsRepository>().To<EFConditionsRepository>();
            ninjectKernel.Bind<IDirectionsRepository>().To<EFDirectionsRepository>();
            ninjectKernel.Bind<IEmployeesRepository>().To<EFEmployeesRepository>();
            ninjectKernel.Bind<IGagesRepository>().To<EFGagesRepository>();
            ninjectKernel.Bind<IIndicatorsRepository>().To<EFIndicatorsRepository>();
            ninjectKernel.Bind<IResultsRepository>().To<EFResultsRepository>();
            ninjectKernel.Bind<ISamplesRepository>().To<EFSamplesRepository>();
            ninjectKernel.Bind<ISelectedSamplesRepository>().To<EFSelectedSamplesRepository>();
            ninjectKernel.Bind<ITechnicalRegulationsRepository>().To<EFTechnicalRegulationsRepository>();
            ninjectKernel.Bind<EFDbContext>().ToSelf().WithConstructorArgument("connectionString",
                                                                               ConfigurationManager.ConnectionStrings[0]
                                                                                   .ConnectionString);
        }
    }
}