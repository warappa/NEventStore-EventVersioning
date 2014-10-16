using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventStoreEventVersioning
{
	[AttributeUsage(AttributeTargets.Class)]
	public class VersionedEventAttribute : Attribute
	{
		public int Version { get; set; }
		public string Identifier { get; set; }

		public VersionedEventAttribute(string identifier, int version = 0)
		{
			this.Version = version;
			this.Identifier = identifier;
		}
	}

	public class VersionedEventSerializationBinder : DefaultSerializationBinder
	{
		private VersionedEventAttribute GetVersionInformation(Type type)
		{
			var attr = type.GetCustomAttributes(typeof(VersionedEventAttribute), false).Cast<VersionedEventAttribute>().FirstOrDefault();

			return attr;
		}

		public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			var versionInfo = GetVersionInformation(serializedType);
			if (versionInfo != null)
			{
				var impl = GetImplementation(versionInfo);

				assemblyName = null;
				typeName = versionInfo.Identifier + "|" + versionInfo.Version;
			}
			else
			{
				base.BindToName(serializedType, out assemblyName, out typeName);
			}
		}

		private VersionedEventAttribute GetVersionInformation(string serializedInfo)
		{
			var strs = serializedInfo.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

			return new VersionedEventAttribute(strs[0], int.Parse(strs[1]));
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			if (typeName.Contains('|'))
			{
				var type = GetImplementation(GetVersionInformation(typeName));
				return type;
			}

			return base.BindToType(assemblyName, typeName);
		}

		private Type GetImplementation(VersionedEventAttribute attribute)
		{
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.IsDynamic == false)
				.SelectMany(x => x.GetExportedTypes()
					.Where(y => y.IsAbstract == false &&
						y.IsInterface == false));

			var versionedEvents = types
				.Where(x => x.GetCustomAttributes(typeof(VersionedEventAttribute), false).Any());

			return versionedEvents.Where(x =>
			{
				var attributes = x.GetCustomAttributes(typeof(VersionedEventAttribute), false).Cast<VersionedEventAttribute>();

				if (attributes.Where(y => y.Version == attribute.Version &&
					y.Identifier == attribute.Identifier)
					.Any())
					return true;
				return false;
			})
				.FirstOrDefault();
		}
	}
}
