using System;
using SubTrackr.Core.Models;

namespace SubTrackr.Core.Interfaces
{
    public interface IReportGenerator
    {
        MonthlyReport GenerateMonthlyReport(string userId, DateTime month);
        string ExportReportToString(MonthlyReport report);
    }
}