using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scoring.Models
{
    public class ScoreResult
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public int DepartmentId { get; set; }
        public decimal ScoreAvg { get; set; }
        public int VoteCount { get; set; }
    }
}