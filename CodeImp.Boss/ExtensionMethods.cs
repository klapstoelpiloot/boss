using System.Collections;

namespace CodeImp.Boss
{
	public static class TypeExtensions
	{
		public static bool IsEnumerable(this Type type)
		{
			return type.GetInterfaces().Contains(typeof(IEnumerable));
		}

		public static bool IsGenericEnumerable(this Type type)
		{
			return type.GetInterfaces().Any(t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
		}
	}

	public static class IEnumerableExtensions
	{
		public static int Count(this IEnumerable e)
		{
			int count = 0;
			foreach(var item in e)
				count++;
			return count;
		}
	}
}
