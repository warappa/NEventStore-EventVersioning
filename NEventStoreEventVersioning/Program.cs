using NEventStore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventStoreEventVersioning
{
	class Program
	{
		static void Main(string[] args)
		{
			/*
			 * 1. Setup a database called "testEventStore" with login "test"/"1234" (granted db_owner membership)
			 * 2. Run this program
			 * 3. View the output - full type-name is "TeamMemberChangedUsername"
			 * 4. In Events.cs, rename "TeamMemberChangedUsername" to "TeamMemberChangedUsernameV1"
			 * 5. Uncomment the second, new version, of the event
			 * 6. In line 39, use the new event "TeamMemberChangedUsername" (not V1) and adding a comment string required by the constructor
			 * 7. Run this program
			 * 8. View the output - 1st event is of type "TeamMemberChangedUsernameV1", 2nd of type "TeamMemberChangedUsername"
			 * 9. In Events.cs, uncomment "Upconversion" class
			 * 10. Run this program
			 * 11. View the output - all event are of type "TeamMemberChangedUsername"
			 * 12. Extra: Rename the "NEventStoreEventVersioning"-namespace and run the program again ;)
			 */

			var eventStore = Wireup.Init()
				.UsingEventUpconversion()
				.WithConvertersFromAssemblyContaining(typeof(Program))
				.UsingSqlPersistence("test")
				.WithDialect(new NEventStore.Persistence.Sql.SqlDialects.MsSqlDialect())
				.InitializeStorageEngine()
				.UsingNewtonsoftJsonSerialization(new VersionedEventSerializationBinder())
				.Build();

			var event1 = new TeamMemberChangedUsername("a", "username", DateTime.Now);

			var stream = eventStore.OpenStream("a");
			stream.Add(new EventMessage
			{
				Body = event1
			});
			stream.CommitChanges(Guid.NewGuid());
			
			var getStream = eventStore.OpenStream("a");
			foreach(var eventItem in getStream.CommittedEvents.Select(x => x.Body).ToArray())
			{
				Console.WriteLine("------------------------------------");
				Console.WriteLine("Full type-name: " + eventItem.GetType().FullName);
				Console.WriteLine("Data:");
				Console.WriteLine(JsonConvert.SerializeObject(eventItem));
			}

			Console.WriteLine("\nDone.");
			Console.ReadKey();
		}
	}
}
