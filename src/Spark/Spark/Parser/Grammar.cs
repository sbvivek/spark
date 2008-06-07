using System.Collections.Generic;

namespace MvcContrib.SparkViewEngine.Parser
{
	public abstract class Grammar
	{
		public static ParseAction<Chain<TValue1, TValue2>> And<TValue1, TValue2>(
			ParseAction<TValue1> p1,
			ParseAction<TValue2> p2)
		{
			return delegate(Position input)
					   {
						   var r1 = p1(input);
						   if (r1 == null) return null;
						   var r2 = p2(r1.Rest);
						   if (r2 == null) return null;

						   return new ParseResult<Chain<TValue1, TValue2>>(r2.Rest,
																		   new Chain<TValue1, TValue2>(r1.Value,
																									   r2.Value));
					   };
		}

		public static ParseAction<TValue> Or<TValue>(
			ParseAction<TValue> p1,
			ParseAction<TValue> p2)
		{
			return input => p1(input) ?? p2(input);
		}

		public static ParseAction<TValue> Opt<TValue>(ParseAction<TValue> parse)
		{
			return input => parse(input) ?? new ParseResult<TValue>(input, default(TValue));
		}

		public static ParseAction<TValue> IfNext<TValue, TValue2>(ParseAction<TValue> parse, ParseAction<TValue2> cond)
		{
			return delegate(Position input)
					   {
						   var result = parse(input);
						   if (result == null || cond(result.Rest) == null) return null;
						   return result;
					   };
		}

		public static ParseAction<TValue> NotNext<TValue, TValue2>(ParseAction<TValue> parse, ParseAction<TValue2> cond)
		{
			return delegate(Position input)
					   {
						   var result = parse(input);
						   if (result == null || cond(result.Rest) != null) return null;
						   return result;
					   };
		}


		public static ParseAction<IList<TValue>> Rep<TValue>(ParseAction<TValue> parse)
		{
			return delegate(Position input)
			{
				var list = new List<TValue>();

				var rest = input;
				var result = parse(rest);
				while (result != null)
				{
					list.Add(result.Value);
					rest = result.Rest;
					result = parse(rest);
				}
				return new ParseResult<IList<TValue>>(rest, list);
			};
		}

		public static ParseAction<IList<TValue>> Rep1<TValue>(ParseAction<TValue> parse)
		{
			return delegate(Position input)
			{
				var rest = input;
				var result = parse(rest);
				if (result == null)
					return null;


				var list = new List<TValue>();
				while (result != null)
				{
					rest = result.Rest;
					list.Add(result.Value);
					result = parse(rest);
				}
				return new ParseResult<IList<TValue>>(rest, list);
			};
		}
	}
}