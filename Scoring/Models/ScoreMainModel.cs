using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scoring.Models;

namespace Scoring.Models
{
    public class ScoreMain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? DepartmentId { get; set; }
        public string Department { get; set; }
    }

    public class ScoreMainModel
    {
        public List<ScoreMain> Employees { get; set; }
    }
}