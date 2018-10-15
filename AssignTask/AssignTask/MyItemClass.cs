using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignTask
{
    public class MyItem
    {
        public string EmployeeName { get; set; }
        public string Task { get; set; }
        public string Desc { get; set; }
        public int EmployeeID { get; set; }
        public string Dep { get; set; }
        public DateTime EstStartDay { get; set; }
        public DateTime EstEndDay { get; set; }
        public DateTime EstWorkTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProjectName{ get; set; }
}
}
