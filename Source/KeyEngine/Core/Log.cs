using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Core;

public sealed class Log
{
	public static void Write(string message)
	{
		try
		{
			Console.Write(message);
		}
		catch (Exception)
		{ }
	}

	public static void Write(string format, object arg0)
	{
		try
		{
			Console.Write(format, arg0);
		}
		catch (Exception)
		{ }
	}

	public static void Write(string format, object arg0, object arg1)
	{
		try
		{
			Console.Write(format, arg0, arg1);
		}
		catch (Exception)
		{ }
	}

	public static void Write(string format, object arg0, object arg1, object arg2)
	{
		try
		{
			Console.Write(format, arg0, arg1, arg2);
		}
		catch (Exception)
		{ }
	}

	public static void Write(string format, params object[] args)
	{
		try
		{
			Console.Write(format, args);
		}
		catch (Exception)
		{ }
	}
}
