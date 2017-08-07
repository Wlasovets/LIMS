using System.Collections.Generic;

namespace Web.Models
{
    public static class DepartmentRepository
    {
        public static List<Department> GetDepartments()
        {
            return new List<Department>
                       {
                           new Department {Name = "все"},
                           new Department {Name = "токсикология"},
                           new Department {Name = "радиология"},
                           new Department {Name = "химлаборатория"},
                           new Department {Name = "бактериология"}
                       };
        }
    }
}