using CodeBits;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bebbs.LightWack.Services
{
    public interface ILightPackProfileService
    {
        ILightPackProfile Import(string profilePath);

        void Export(ILightPackProfile profile, string profilePath);
    }

    internal class LightPackProfileService : ILightPackProfileService
    {
        private static readonly string ImportSectionLedGroup = "Led";
        private static readonly Regex ImportSectionLedRegex = new Regex(@"LED_(?<Led>\d+)", RegexOptions.Compiled);
        private static readonly Regex LedPositionRegex = new Regex(@"^@Point\x28(?<Left>\d+)\s+(?<Top>\d+)\x29$", RegexOptions.Compiled);
        private static readonly Regex LedSizeRegex = new Regex(@"^@Size\x28(?<Width>\d+)\s+(?<Height>\d+)\x29$", RegexOptions.Compiled);

        private IEnumerable<Tuple<int, IniFile.Section>> LedSection(IniFile.Section section)
        {
            Match match = ImportSectionLedRegex.Match(section.Name);

            int led;

            if (match.Success && match.Groups[ImportSectionLedGroup].Success && Int32.TryParse(match.Groups[ImportSectionLedGroup].Value, out led))
            {
                return new[] { Tuple.Create(led - 1, section) };
            }
            else
            {
                return Enumerable.Empty<Tuple<int, IniFile.Section>>();
            }
        }

        private IEnumerable<ILightPackLed> ReadLed(int index, IniFile.Section section)
        {
            string enabledText = section["IsEnabled"];
            string positionText = section["Position"];
            string sizeText = section["Size"];

            if (!string.IsNullOrWhiteSpace(enabledText) && !string.IsNullOrWhiteSpace(positionText) && !string.IsNullOrWhiteSpace(sizeText))
            {
                Match positionMatch = LedPositionRegex.Match(positionText);
                Match sizeMatch = LedSizeRegex.Match(sizeText);

                bool enabled;
                int left, top;
                int width, height;

                if (bool.TryParse(enabledText, out enabled) &&
                    positionMatch.Success && positionMatch.Groups["Left"].Success && Int32.TryParse(positionMatch.Groups["Left"].Value, out left) &&  positionMatch.Groups["Top"].Success && Int32.TryParse(positionMatch.Groups["Top"].Value, out top) &&
                    sizeMatch.Success && sizeMatch.Groups["Width"].Success && Int32.TryParse(sizeMatch.Groups["Width"].Value, out width) && sizeMatch.Groups["Height"].Success && Int32.TryParse(sizeMatch.Groups["Height"].Value, out height))
                {
                    return new [] 
                    {
                        new LightPackLed(index, enabled, new Point(left, top), new Size(width, height))
                    };
                }
                else
                {
                    return Enumerable.Empty<ILightPackLed>();
                }
            }
            else
            {
                return Enumerable.Empty<ILightPackLed>();
            }
        }

        private void WriteGeneral(IniFile file, ILightPackProfile profile)
        {
            IniFile.Section section = new IniFile.Section("General");
            section["LightpackMode"] = "Ambilight";
            section["IsBacklightEnabled"] = "true";
            file.Add(section);
        }

        private void WriteGrab(IniFile file, ILightPackProfile profile)
        {
            IniFile.Section section = new IniFile.Section("Grab");
            section["Grabber"] = "WinAPI";
            section["IsAvgColorsEnabled"] = "false";
            section["IsSendDataOnlyIfColorsChanges"] = "true";
            section["Slowdown"] = "20";
            section["LuminosityThreshold"] = "0";
            section["IsMinimumLuminosityEnabled"] = "true";
            file.Add(section);
        }

        private void WriteMoodLamp(IniFile file, ILightPackProfile profile)
        {
            IniFile.Section section = new IniFile.Section("MoodLamp");
            section["LiquidMode"] = "true";
            section["Color"] = "#00ff00";
            section["Speed"] = "23";
            file.Add(section);
        }

        private void WriteDevice(IniFile file, ILightPackProfile profile)
        {
            IniFile.Section section = new IniFile.Section("Device");
            section["RefreshDelay"] = "100";
            section["Brightness"] = "100";
            section["Smooth"] = "0";
            section["Gamma"] = "2.00399994850159";
            section["ColorDepth"] = "128";
            file.Add(section);
        }

        private void WriteLeds(IniFile file, ILightPackProfile profile)
        {
            profile.Leds.ForEach(
                led =>
                {
                    IniFile.Section section = new IniFile.Section(string.Format("LED_{0}", led.Index));
                    section["IsEnabled"] = led.IsEnabled.ToString().ToLower();
                    section["Position"] = string.Format("@Point({0} {1})", led.Position.X, led.Position.Y);
                    section["Size"] = "@Size(150 150)";
                    section["CoefRed"] = "1";
                    section["CoefGreen"] = "1";
                    section["CoefBlue"] = "1";
                    file.Add(section);
                }
            );
        }

        public ILightPackProfile Import(string profilePath)
        {
            using (FileStream stream = new FileStream(profilePath, FileMode.Open, FileAccess.Read))
            {
                IniFile profile = new IniFile(stream);

                return new LightPackProfile(
                    profile.SelectMany(LedSection).SelectMany(tuple => ReadLed(tuple.Item1, tuple.Item2))
                );
            }
        }

        public void Export(ILightPackProfile profile, string profilePath)
        {
            IniFile file = new IniFile();

            WriteGeneral(file, profile);
            WriteGrab(file, profile);
            WriteMoodLamp(file, profile);
            WriteDevice(file, profile);
            WriteLeds(file, profile);

            file.SaveTo(profilePath);
        }
    }
}
