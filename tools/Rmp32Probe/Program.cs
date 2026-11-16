// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using WwDevicesDotNet;
using WwDevicesDotNet.Winctrl.Rmp32;

namespace Rmp32Probe
{
    /// <summary>
    /// Guided probe for the Winctrl 32 RMP: asks the user to press / turn / look at
    /// things, records what the panel reports and writes everything to
    /// rmp32-probe-results.json (plus a raw input log) so it can be analysed afterwards.
    /// </summary>
    static class Program
    {
        // Zero-based bit positions in the 24-bit button mask (button N = bit N-1).
        const int ToggleOffBit = 19;
        const int ToggleOnBit = 20;
        static readonly int[] EncoderBits = { 14, 15, 16, 17 };

        static IFrontpanel _Panel;
        static Rmp32Device _Device;
        static readonly ConcurrentQueue<(double Time, byte[] Data)> _Reports = new ConcurrentQueue<(double, byte[])>();
        static readonly Stopwatch _Clock = Stopwatch.StartNew();
        static StreamWriter _RawLog;
        static uint _Mask;
        static byte[] _LastReport;
        static readonly SortedDictionary<string, object> _Results = new SortedDictionary<string, object>();
        static string _ResultsPath;

        static readonly (string Name, int Button)[] ExpectedButtons = {
            ("Swap (flèche <-> entre les afficheurs)", 1),
            ("VHF1", 2), ("VHF2", 3), ("VHF3", 4), ("LOAD", 5),
            ("HF1", 6), ("HF2", 7), ("AM", 8),
            ("NAV", 9), ("VOR", 10), ("ILS", 11), ("GLS", 12), ("MLS", 13), ("ADF", 14),
            ("SEL (si c'est un bouton)", 0),
            ("Push du gros bouton rotatif", 19),
        };

        static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Rmp32Probe : sonde guidée pour le Winctrl 32 RMP ===\n");

            if (!Connect())
            {
                return 1;
            }

            var cmdIndex = Array.IndexOf(args, "--cmd");
            if (cmdIndex >= 0)
            {
                RunCommands(args.Skip(cmdIndex + 1).ToArray());
                _Panel.Dispose();
                return 0;
            }

            if (args.Contains("--watch"))
            {
                Watch();
                return 0;
            }

            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            _ResultsPath = $"rmp32-probe-results-{stamp}.json";
            _RawLog = new StreamWriter($"rmp32-probe-raw-{stamp}.log") { AutoFlush = true };
            Console.WriteLine($"Résultats : {Path.GetFullPath(_ResultsPath)}");
            Console.WriteLine($"Log brut  : {Path.GetFullPath($"rmp32-probe-raw-{stamp}.log")}\n");

            var running = true;
            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("1 Boutons    2 Encodeurs + interrupteur    3 Afficheurs");
                Console.WriteLine("4 LEDs + rétroéclairage    5 Octets inconnus de l'affichage");
                Console.WriteLine("6 Moniteur live    A Tout enchaîner (1-4)    Q Quitter");
                Console.Write("> ");
                var choice = (Console.ReadLine() ?? "q").Trim().ToLowerInvariant();
                try
                {
                    switch (choice)
                    {
                        case "1": TestButtons(); break;
                        case "2": TestEncoders(); break;
                        case "3": TestDisplays(); break;
                        case "4": TestLeds(); break;
                        case "5": TestUnknownBytes(); break;
                        case "6": Monitor(); break;
                        case "a": TestButtons(); TestEncoders(); TestDisplays(); TestLeds(); break;
                        case "q": running = false; break;
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("(étape interrompue)");
                }
                Save();
            }

            try { _Device.Reset(); } catch { }
            _Panel.Dispose();
            _RawLog.Dispose();
            Console.WriteLine($"\nTerminé. Envoie-moi : {_ResultsPath}");
            return 0;
        }

        // ------------------------------------------------------------ connection

        static bool Connect()
        {
            var found = FrontpanelFactory.FindLocalDevices().Where(d => d.Device == Device.WinctrlRmp32).ToList();
            if (found.Count == 0)
            {
                Console.WriteLine("Aucun RMP 32 trouvé (VID 0x4098, PID 0xBB83/84/85). Ferme SimAppPro et réessaie.");
                return false;
            }

            var id = found[0];
            if (found.Count > 1)
            {
                for (var i = 0; i < found.Count; ++i)
                {
                    Console.WriteLine($"{i + 1}: {found[i]}");
                }
                Console.Write("Lequel ? ");
                int.TryParse(Console.ReadLine(), out var n);
                id = found[Math.Max(0, Math.Min(found.Count - 1, n - 1))];
            }

            Console.WriteLine($"Connexion à {id} ...");
            try
            {
                _Panel = FrontpanelFactory.ConnectLocal(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Échec : {ex.Message} (SimAppPro est-il fermé ?)");
                return false;
            }
            _Device = _Panel as Rmp32Device;
            if (_Device == null)
            {
                Console.WriteLine("Connexion impossible.");
                return false;
            }

            _Panel.InputReportReceived += (s, e) =>
            {
                var data = (byte[])e.RawData.Clone();
                _Reports.Enqueue((_Clock.Elapsed.TotalSeconds, data));
            };

            _Results["device"] = new { id.Description, ProductId = $"0x{id.UsbProductId:X4}" };
            return true;
        }

        // ------------------------------------------------------------ input helpers

        static uint MaskOf(byte[] d) => d.Length >= 4 ? (uint)(d[1] | (d[2] << 8) | (d[3] << 16)) : 0;

        static void Flush()
        {
            while (_Reports.TryDequeue(out var r))
            {
                Track(r);
            }
        }

        static void Track((double Time, byte[] Data) r)
        {
            if (_LastReport == null || !r.Data.SequenceEqual(_LastReport))
            {
                _RawLog?.WriteLine($"{r.Time,9:F3} len={r.Data.Length} {BitConverter.ToString(r.Data).Replace("-", "")}");
            }
            _LastReport = r.Data;
            _Mask = MaskOf(r.Data);
        }

        static void CheckAbort()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
                {
                    throw new OperationCanceledException();
                }
                if (key == ConsoleKey.S)
                {
                    throw new TimeoutException("skip");
                }
            }
        }

        static int Counter(byte[] d, int offset) => d.Length > offset + 1 ? (short)(d[offset] | (d[offset + 1] << 8)) : 0;

        /// <summary>
        /// Waits for a button bit that goes from released to pressed, ignoring the
        /// given zero-based bits. Returns the 1-based button number, or null if skipped.
        /// </summary>
        static int? WaitForPress(string prompt, params int[] ignoredBits)
        {
            Console.WriteLine($"\n>> {prompt}   [s = passer, q = arrêter]");
            Flush();
            while (true)
            {
                try { CheckAbort(); }
                catch (TimeoutException) { Console.WriteLine("   (passé)"); return null; }

                if (_Reports.TryDequeue(out var r))
                {
                    var before = _Mask;
                    Track(r);
                    var rising = _Mask & ~before;
                    for (var bit = 0; bit < 24; ++bit)
                    {
                        if ((rising >> bit & 1) != 0 && !ignoredBits.Contains(bit))
                        {
                            Console.WriteLine($"   -> bouton {bit + 1} détecté");
                            var sw = Stopwatch.StartNew();
                            while ((_Mask >> bit & 1) != 0 && sw.Elapsed.TotalSeconds < 3)
                            {
                                if (_Reports.TryDequeue(out var rr)) Track(rr); else Thread.Sleep(2);
                            }
                            return bit + 1;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(2);
                }
            }
        }

        static string Ask(string question)
        {
            Console.Write($"   {question} ");
            return (Console.ReadLine() ?? "").Trim();
        }

        // ------------------------------------------------------------ tests

        static void TestButtons()
        {
            Console.WriteLine("\n--- BOUTONS --- Appuie sur chaque touche demandée (un appui bref).");
            var found = new SortedDictionary<string, object>();
            var ignore = EncoderBits.Concat(new[] { ToggleOffBit, ToggleOnBit }).ToArray();
            foreach (var (name, expected) in ExpectedButtons)
            {
                var button = WaitForPress($"Appuie sur : {name}", ignore);
                if (button.HasValue)
                {
                    var verdict = expected == 0 ? "nouveau" : button == expected ? "conforme" : $"DIFFERENT (attendu {expected})";
                    Console.WriteLine($"   {verdict}");
                    found[name] = new { button, expected, verdict };
                }
                else
                {
                    found[name] = new { button = (int?)null, expected, verdict = "passé" };
                }
            }
            _Results["buttons"] = found;
        }

        static void TestEncoders()
        {
            Console.WriteLine("\n--- ENCODEURS ET INTERRUPTEUR ---");
            var found = new SortedDictionary<string, object>();
            var rings = new[] {
                "l'anneau EXTÉRIEUR du double bouton, 3 crans vers la DROITE (sens horaire)",
                "l'anneau EXTÉRIEUR, 3 crans vers la GAUCHE",
                "l'anneau INTÉRIEUR (le gros bouton), 3 crans vers la DROITE (sens horaire)",
                "l'anneau INTÉRIEUR (le gros bouton), 3 crans vers la GAUCHE",
            };
            foreach (var ring in rings)
            {
                Console.WriteLine($"\n>> Tourne {ring}   [s = passer, q = arrêter]");
                Flush();
                var startA = Counter(_LastReport ?? new byte[24], 10);
                var startB = Counter(_LastReport ?? new byte[24], 12);
                var pulses = new SortedDictionary<int, int>();
                var lastActivity = (double?)null;
                var skipped = false;
                while (true)
                {
                    try { CheckAbort(); }
                    catch (TimeoutException) { skipped = true; break; }

                    if (_Reports.TryDequeue(out var r))
                    {
                        var before = _Mask;
                        Track(r);
                        var rising = _Mask & ~before;
                        foreach (var bit in EncoderBits)
                        {
                            if ((rising >> bit & 1) != 0)
                            {
                                pulses[bit + 1] = pulses.TryGetValue(bit + 1, out var c) ? c + 1 : 1;
                                lastActivity = _Clock.Elapsed.TotalSeconds;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(2);
                        if (lastActivity.HasValue && _Clock.Elapsed.TotalSeconds - lastActivity > 1.2)
                        {
                            break;
                        }
                    }
                }

                if (skipped) { Console.WriteLine("   (passé)"); continue; }
                var dA = Counter(_LastReport, 10) - startA;
                var dB = Counter(_LastReport, 12) - startB;
                Console.WriteLine($"   -> impulsions par bouton : {string.Join(", ", pulses.Select(p => $"#{p.Key} x{p.Value}"))} ; compteur A {dA:+0;-0;0}, B {dB:+0;-0;0}");
                found[ring] = new { pulses = pulses.ToDictionary(p => $"button{p.Key}", p => p.Value), counterA = dA, counterB = dB };
            }
            _Results["encoders"] = found;

            var toggle = new SortedDictionary<string, object>();
            foreach (var position in new[] { "OFF", "ON" })
            {
                var button = WaitForPress($"Mets l'interrupteur ON/OFF sur {position}", EncoderBits.Concat(Enumerable.Range(0, 19)).ToArray());
                toggle[position] = new { button };
            }
            _Results["toggle"] = toggle;
        }

        static void Show(string active, string standby)
        {
            _Device.UpdateDisplay(new Rmp32State { ActiveDisplay = active, StandbyDisplay = standby });
        }

        static void TestDisplays()
        {
            Console.WriteLine("\n--- AFFICHEURS --- Regarde les deux afficheurs. Entrée = comme prévu, sinon décris.");
            _Device.SetBrightness(255, 255, 255);
            var answers = new SortedDictionary<string, string>();

            _Device.UpdateDisplayRaw(Enumerable.Range(0, 40).Select(i => i >= 4 && i < 16 ? (byte)0xFF : (byte)0).ToArray());
            answers["all segments"] = Ask("Tous les segments + points sont allumés (8.8.8.8.8.8.) sur les DEUX afficheurs ?");

            Show("123456", "654321");
            answers["digits 123456 / 654321"] = Ask("ACTIVE = 123456 et STBY = 654321 (gauche -> droite) ?");

            Show("124.850", "118.000");
            answers["124.850 / 118.000"] = Ask("ACTIVE = 124.850 et STBY = 118.000 (point après le 3e chiffre) ?");

            for (var i = 0; i < 6; ++i)
            {
                var text = new string(' ', i) + "1";
                Show(text, text);
                Thread.Sleep(700);
            }
            answers["walking digit"] = Ask("Le '1' s'est déplacé de gauche à droite sur les deux afficheurs ?");

            Show("ABCDEF", "HLNOPR");
            answers["letters ABCDEF / HLNOPR"] = Ask("ACTIVE = ABCDEF et STBY = HLNOPR (b, d, E minuscule/majuscule : note ce qui est bizarre) ?");
            Show("TU-_", "dAtA");
            answers["letters TU-_ / dAtA"] = Ask("ACTIVE = TU-_ (aligné à droite) et STBY = dAtA ?");

            _Device.Reset();
            answers["blank after reset"] = Ask("Après Reset les deux afficheurs sont éteints ?");
            _Results["displays"] = answers;
        }

        static void TestLeds()
        {
            Console.WriteLine("\n--- LEDS ET RÉTROÉCLAIRAGE --- Décris ce qui s'allume (ex: 'triangle au-dessus de VHF1'). Entrée = rien.");
            _Device.Reset();
            var answers = new SortedDictionary<string, string>();

            for (byte type = 0x03; type <= 0x10; ++type)
            {
                _Device.SendIllumination(type, 1);
                answers[$"IND{type - 2} (type 0x{type:X2})"] = Ask($"IND{type - 2} (type 0x{type:X2}) allumée : qu'est-ce qui s'allume ?");
                _Device.SendIllumination(type, 0);
            }

            foreach (var (label, type) in new[] { ("type 0x00", (byte)0), ("type 0x01", (byte)1), ("type 0x02", (byte)2) })
            {
                _Device.SetBrightness(255, 255, 255);
                _Device.SendIllumination(type, 0);
                var off = Ask($"{label} = 0 : qu'est-ce qui s'éteint / change ?");
                _Device.SendIllumination(type, 0x30);
                var mid = Ask($"{label} = 0x30 (faible) : qu'est-ce qui change ?");
                _Device.SendIllumination(type, 255);
                var full = Ask($"{label} = 255 (max) : qu'est-ce qui change ?");
                answers[label] = $"0: {off} | 0x30: {mid} | 255: {full}";
            }

            _Device.Reset();
            _Results["leds"] = answers;
        }

        static void TestUnknownBytes()
        {
            Console.WriteLine("\n--- OCTETS INCONNUS DE L'AFFICHAGE --- Chaque octet est mis à 0xFF seul. Entrée = rien, q = arrêter.");
            var answers = new SortedDictionary<string, string>();
            var offsets = Enumerable.Range(0, 4).Concat(Enumerable.Range(16, 24));
            foreach (var offset in offsets)
            {
                var framebuffer = new byte[40];
                framebuffer[offset] = 0xFF;
                _Device.UpdateDisplayRaw(framebuffer);
                var answer = Ask($"octet {offset} = 0xFF : qu'est-ce qui s'allume ?");
                if (answer.Equals("q", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                if (answer.Length > 0)
                {
                    answers[$"byte {offset}"] = answer;
                }
            }
            _Device.Reset();
            _Results["unknownFramebufferBytes"] = answers;
        }

        static void Monitor()
        {
            Console.WriteLine("\n--- MONITEUR --- Touche une touche du clavier pour revenir au menu.");
            Flush();
            while (!Console.KeyAvailable)
            {
                if (_Reports.TryDequeue(out var r))
                {
                    var before = _Mask;
                    var lastBefore = _LastReport;
                    Track(r);
                    if (lastBefore == null || !r.Data.SequenceEqual(lastBefore))
                    {
                        var down = Enumerable.Range(0, 24).Where(b => ((_Mask & ~before) >> b & 1) != 0).Select(b => b + 1);
                        var up = Enumerable.Range(0, 24).Where(b => ((before & ~_Mask) >> b & 1) != 0).Select(b => b + 1);
                        Console.WriteLine($"{r.Time,8:F3}  +[{string.Join(",", down)}] -[{string.Join(",", up)}]  A={Counter(r.Data, 10)} B={Counter(r.Data, 12)}  {BitConverter.ToString(r.Data).Replace("-", "")}");
                    }
                }
                else
                {
                    Thread.Sleep(2);
                }
            }
            Console.ReadKey(true);
        }

        /// <summary>
        /// One-shot commands, separated by ';' : show A S | led N 0/1 | ill TYPE VALUE |
        /// raw HEX80 | bright P L K | sleep MS | reset.
        /// </summary>
        static void RunCommands(string[] words)
        {
            var commands = string.Join(" ", words).Split(';');
            foreach (var command in commands)
            {
                var w = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (w.Length == 0) continue;
                switch (w[0])
                {
                    case "show": Show(w.Length > 1 ? w[1] : "", w.Length > 2 ? w[2] : ""); break;
                    case "led": _Device.SendIllumination((byte)(int.Parse(w[1]) + 2), byte.Parse(w[2])); break;
                    case "ill": _Device.SendIllumination(Convert.ToByte(w[1], 16), byte.Parse(w[2])); break;
                    case "raw": _Device.UpdateDisplayRaw(Enumerable.Range(0, 40).Select(i => Convert.ToByte(w[1].Substring(i * 2, 2), 16)).ToArray()); break;
                    case "bright": _Device.SetBrightness(byte.Parse(w[1]), byte.Parse(w[2]), byte.Parse(w[3])); break;
                    case "sleep": Thread.Sleep(int.Parse(w[1])); break;
                    case "reset": _Device.Reset(); break;
                }
                Console.WriteLine($"ok: {command.Trim()}");
                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// Non-interactive: prints one line per press/release/encoder change until killed.
        /// Meant to be run in the background while someone else drives the panel.
        /// </summary>
        static void Watch()
        {
            Console.WriteLine("watching...");
            var prevCounters = (A: 0, B: 0);
            while (true)
            {
                if (!_Reports.TryDequeue(out var r))
                {
                    Thread.Sleep(2);
                    continue;
                }
                var before = _Mask;
                Track(r);
                var down = Enumerable.Range(0, 24).Where(b => ((_Mask & ~before) >> b & 1) != 0).Select(b => b + 1);
                var up = Enumerable.Range(0, 24).Where(b => ((before & ~_Mask) >> b & 1) != 0).Select(b => b + 1);
                var counters = (A: Counter(r.Data, 10), B: Counter(r.Data, 12));
                var parts = new List<string>();
                if (down.Any()) parts.Add("DOWN " + string.Join(",", down));
                if (up.Any()) parts.Add("UP " + string.Join(",", up));
                if (counters != prevCounters) parts.Add($"encA={counters.A} encB={counters.B}");
                prevCounters = counters;
                if (parts.Count > 0)
                {
                    Console.WriteLine($"{r.Time,8:F2}  {string.Join("  ", parts)}");
                    Console.Out.Flush();
                }
            }
        }

        static void Save()
        {
            File.WriteAllText(_ResultsPath, JsonSerializer.Serialize(_Results, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
