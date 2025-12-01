using System;
using System.Collections.Generic;

namespace SubTrackr.Core.Models
{
	public class MonthlyReport
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public DateTime ReportMonth { get; set; }
		public List<SubscriptionBase> ActiveSubscriptions { get; set; }
		public List<Payment> PaymentHistory { get; set; }
		public decimal TotalAmountBilled { get; set; }
		public int FailedPayments { get; set; }

		public MonthlyReport()
		{
			ActiveSubscriptions = new List<SubscriptionBase>();
			PaymentHistory = new List<Payment>();
		}
	}
}