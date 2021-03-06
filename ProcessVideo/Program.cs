﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ProcessVideo
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please paste the url then press Enter");
                var url = Console.ReadLine();
                if (url == "Q")
                {
                    break;
                }

                DownloadMp3(url);
            }

            Console.ReadLine();
        }

        public static void DownloadMp3(string url)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var workingDir = Directory.GetCurrentDirectory();

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = "you-get",
                WorkingDirectory = workingDir,
                Arguments = $"  {url}"
            };
            var exeProcess = Process.Start(startInfo);
            var fileNameLine = "";
            while (!exeProcess.StandardOutput.EndOfStream)
            {
                string line = exeProcess.StandardOutput.ReadLine();
                // do something with line
                Console.WriteLine(line);
                if (line.StartsWith("Downloading"))
                {
                    fileNameLine = line;
                }
            }

            if (!string.IsNullOrWhiteSpace(fileNameLine))
            {
                var filenameWithExtension = fileNameLine.Substring(11).TrimEnd('.').Trim();
                var index = filenameWithExtension.LastIndexOf('.');
                var filename = filenameWithExtension.Substring(0, index);
                var startInfo2 = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "ffmpeg",
                    WorkingDirectory = workingDir,
                    Arguments = $" -i \"{filenameWithExtension}\" -vn -f mp3 -ab 192k \"{filename}.mp3\""
                };
                using (var ffmpegProcess = Process.Start(startInfo2))
                {
                    ffmpegProcess.WaitForExit();
                }
            }
            Console.WriteLine("############## File processed #################");
        }
    }
}
