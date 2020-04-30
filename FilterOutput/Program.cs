using System;
using System.Diagnostics;
using System.IO;

namespace FilterOutput
{
	class Program
	{
		private static StreamWriter htmlStream;
		private static StreamWriter errStream;
		private static StreamWriter outStream;

		static void Main(string[] args)
		{
			//TODO usage if no args

			//Pass args
			Process process = new Process();
			process.StartInfo.FileName = args[0]; //TODO létezik-e a fájl
			for(int i = 1; i < args.Length; i++)
				process.StartInfo.ArgumentList.Add(args[i]);

			//TODO stdin

			//Redirect out and error streams
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardError = true;
			process.ErrorDataReceived += Process_ErrorDataReceived;
			process.StartInfo.RedirectStandardOutput = true;
			process.OutputDataReceived += Process_OutputDataReceived;

			//Open output files
			errStream = new StreamWriter("error.log");
			outStream = new StreamWriter("output.log");
			htmlStream = new StreamWriter("log.html");
			htmlStream.WriteLine("<html><body>");

			//TODO error handling

			//Start the process
			process.Start();
			process.BeginErrorReadLine();
			process.BeginOutputReadLine();

			//Wait for process to finish
			process.WaitForExit();
			Environment.ExitCode = process.ExitCode;
			process.Close();

			//Close streams
			errStream.Close();
			outStream.Close();
			htmlStream.WriteLine("</body></html>");
			htmlStream.Close();
			Console.ResetColor();
		}

		private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			outStream.WriteLine(e.Data);
			Log(e.Data);
			Console.Out.WriteLine(e.Data);
		}

		private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			errStream.WriteLine(e.Data);
			Log(e.Data);
			Console.Error.WriteLine(e.Data);
		}

		private static void Log(string line)
		{
			ConsoleColor? color = null;

			if(line.Contains("error", StringComparison.InvariantCultureIgnoreCase))
				color = ConsoleColor.Red;
			else if(line.Contains("warning", StringComparison.InvariantCultureIgnoreCase))
				color = ConsoleColor.Yellow;
			else if(line.Contains("info", StringComparison.InvariantCultureIgnoreCase) ||
				line.Contains("message", StringComparison.InvariantCultureIgnoreCase))
				color = ConsoleColor.Blue;

			if(color.HasValue) {
				Console.ForegroundColor = color.Value;
				htmlStream.WriteLine($"<span style=\"color:{color.Value}\">{line}</span><br>");
			} else {
				Console.ResetColor();
				htmlStream.WriteLine($"{line}<br>");
			}
		}
	}
}
