using System.Web.Mvc;
using System.Web.Security;
using BusinessLogic;
using Web.Models;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private DataManager dataManager;
        public AccountController(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        public ActionResult WindowLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult WindowLogin(LoginViewModel model)
        {
            return Login(model, "Window", "Index", "window", "одно окно", "WindowLogin");
        }

        public ActionResult MetrologyLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MetrologyLogin(LoginViewModel model)
        {
            return Login(model, "Metrology", "Index", "metrology", "метрология", "MetrologyLogin");
        }

        public ActionResult SamplingLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SamplingLogin(LoginViewModel model)
        {
            return Login(model, "Sampling", "Index", "sampling", "отбор проб", "SamplingLogin");
        }

        public ActionResult RadiologyLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RadiologyLogin(LoginViewModel model)
        {
            return Login(model, "Radiology", "Index", "radiology", "радиология", "RadiologyLogin");
        }

        public ActionResult ChemistryLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChemistryLogin(LoginViewModel model)
        {
            return Login(model, "ChemLab", "Index", "chemLab", "химлаборатория", "ChemistryLogin");
        }

        public ActionResult ToxicologyLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ToxicologyLogin(LoginViewModel model)
        {
            return Login(model, "Toxicology", "Index", "toxicology", "токсикология", "ToxicologyLogin");
        }

        public ActionResult BacteriologyLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BacteriologyLogin(LoginViewModel model)
        {
            return Login(model, "Bacteriology", "Index", "bacteriology", "бактериология", "BacteriologyLogin");
        }

        //Проверка вводимых данных и вход в заданный раздел (повтоярющийся код)
        private ActionResult Login(LoginViewModel model, string controller, string action, string authName, string department, string viewName)
        {
            if (ModelState.IsValid)
            {
                if (dataManager.Employees.ValidateEmployee(model.LoginName, model.Password))
                {
                    if (dataManager.Employees.GetEmployeeByLogin(model.LoginName).Department == department)
                    {
                        FormsAuthentication.SetAuthCookie(authName, false);
                        return RedirectToAction(action, controller);
                    }
                    ModelState.AddModelError("", "Пользователь " + model.LoginName + " не зарегистрирован в этом отделе.");
                }
                else
                {
                    ModelState.AddModelError("", "Логин или пароль введен неверно.");
                }
            }
            return View(viewName, model);
        }
    }
}
