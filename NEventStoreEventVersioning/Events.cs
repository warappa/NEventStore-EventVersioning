using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventStoreEventVersioning
{
	public class Event
	{
		public Event() { }
		public Event(string aggregateId)
		{
			this.AggregateId = aggregateId;
		}
		public string AggregateId { get; set; }
	}

	[VersionedEvent("TeamMemberChangedUsername", 0)]
	public class TeamMemberChangedUsername : Event
	{
		public readonly string Username;
		public readonly DateTime Date = DateTime.Now;

		public TeamMemberChangedUsername(string aggregateId, string username, DateTime date)
			: base(aggregateId)
		{
			if (date == default(DateTime) ||
				date == DateTime.MinValue)
				date = DateTime.Now;

			this.Username = username;
			this.Date = date;
		}
	}
	
	/*
	 * Uncomment in step 5
	 */
	/*
	[VersionedEvent("TeamMemberChangedUsername", 1)]
	public class TeamMemberChangedUsername : Event
	{
		public readonly string Username;
		public readonly DateTime Date = DateTime.Now;
		public readonly string Comment = "";

		public TeamMemberChangedUsername(string aggregateId, string username, DateTime date, string comment)
			: base(aggregateId)
		{
			if (date == default(DateTime) ||
				date == DateTime.MinValue)
				date = DateTime.Now;

			this.Username = username;
			this.Date = date;
			this.Comment = comment;
		}
	}
	*/

	/*
	 * Uncomment in step 9
	 */
	/*
	public class Upconversion : NEventStore.Conversion.IUpconvertEvents<TeamMemberChangedUsernameV1, TeamMemberChangedUsername>
	{
		public TeamMemberChangedUsername Convert(TeamMemberChangedUsernameV1 sourceEvent)
		{
			return new TeamMemberChangedUsername(sourceEvent.AggregateId, sourceEvent.Username, sourceEvent.Date, "comment");
		}
	}
	*/
}
