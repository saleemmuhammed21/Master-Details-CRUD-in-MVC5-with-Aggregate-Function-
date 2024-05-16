using Project_Job_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_Job_Portal.ViewModels
{
    public class GroupedData
    {
        public string Key { get; set; }
        public IEnumerable<Qualification> Data { get; set; }=new List<Qualification>();
        public IEnumerable<Candidate> candidates { get; set; } = new List<Candidate>();
    }
}