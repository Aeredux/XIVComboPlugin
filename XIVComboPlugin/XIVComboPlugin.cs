﻿using Dalamud.Game.Command;
using Dalamud.Plugin;
using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Dalamud.Game;
using Dalamud.Utility;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Utility;
using Dalamud.Game.Network.Structures.InfoProxy;
using Dalamud.Game.ClientState.Objects;

namespace XIVComboPlugin
{
    class XIVComboPlugin : IDalamudPlugin
    {
        public string Name => "XIV Combo Plugin";

        [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService] internal static IClientState ClientState { get; private set; } = null!;
        [PluginService] internal static IChatGui ChatGui{ get; private set; } = null!;
        [PluginService] internal static IJobGauges JobGauges { get; private set; } = null!;
        [PluginService] internal static IGameInteropProvider HookProvider{ get; private set; } = null!;
        [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;
        [PluginService] internal static IDataManager DataManager { get; private set; } = null!;

        [PluginService] internal static ITargetManager TargetManager { get; private set; } = null!;

        public XIVComboConfiguration Configuration;

        private IconReplacer iconReplacer;
        private CustomComboPreset[] orderedByClassJob;

        public XIVComboPlugin()
        {
            CommandManager.AddHandler("/pcombo", new CommandInfo(OnCommandDebugCombo)
            {
                HelpMessage = "Open a window to edit custom combo settings.",
                ShowInHelp = true
            });

            this.Configuration = PluginInterface.GetPluginConfig() as XIVComboConfiguration ?? new XIVComboConfiguration();
            if (Configuration.Version < 4)
            {
                Configuration.Version = 4;
            }

            this.iconReplacer = new IconReplacer(SigScanner, ClientState, DataManager, this.Configuration, HookProvider, JobGauges, PluginLog, TargetManager);

            this.iconReplacer.Enable();

            PluginInterface.UiBuilder.OpenConfigUi += () => isImguiComboSetupOpen = true;
            PluginInterface.UiBuilder.OpenMainUi += () => isImguiComboSetupOpen = true;
            PluginInterface.UiBuilder.Draw += UiBuilder_OnBuildUi;

            orderedByClassJob = Enum.GetValues(typeof(CustomComboPreset)).Cast<CustomComboPreset>().ToArray();
            Array.Sort(orderedByClassJob, (x, y) => x.GetAttribute<CustomComboInfoAttribute>().ClassJob.CompareTo(y.GetAttribute<CustomComboInfoAttribute>().ClassJob));
            UpdateConfig();
        }

        private bool isImguiComboSetupOpen = false;

        private string ClassJobToName(byte key)
        {
            switch (key)
            {
                default: return "Unknown";
                case 1: return "Gladiator";
                case 2: return "Pugilist";
                case 3: return "Marauder";
                case 4: return "Lancer";
                case 5: return "Archer";
                case 6: return "Conjurer";
                case 7: return "Thaumaturge";
                case 8: return "Carpenter";
                case 9: return "Blacksmith";
                case 10: return "Armorer";
                case 11: return "Goldsmith";
                case 12: return "Leatherworker";
                case 13: return "Weaver";
                case 14: return "Alchemist";
                case 15: return "Culinarian";
                case 16: return "Miner";
                case 17: return "Botanist";
                case 18: return "Fisher";
                case 19: return "Paladin";
                case 20: return "Monk";
                case 21: return "Warrior";
                case 22: return "Dragoon";
                case 23: return "Bard";
                case 24: return "White Mage";
                case 25: return "Black Mage";
                case 26: return "Arcanist";
                case 27: return "Summoner";
                case 28: return "Scholar";
                case 29: return "Rogue";
                case 30: return "Ninja";
                case 31: return "Machinist";
                case 32: return "Dark Knight";
                case 33: return "Astrologian";
                case 34: return "Samurai";
                case 35: return "Red Mage";
                case 36: return "Blue Mage";
                case 37: return "Gunbreaker";
                case 38: return "Dancer";
                case 39: return "Reaper";
                case 40: return "Sage";
                case 41: return "Viper";
                case 42: return "Pictomancer";
            }
        }

        private void UpdateConfig()
        {

        }

        private void UiBuilder_OnBuildUi()
        {

            if (!isImguiComboSetupOpen)
                return;
            if (orderedByClassJob.Length != Configuration.ComboPresets.Length)
            {
                bool[] newConfig = new bool[orderedByClassJob.Length];
                for (var i = 0; i < orderedByClassJob.Length && i < Configuration.ComboPresets.Length; i++)
                {
                    newConfig[i] = Configuration.ComboPresets[i];
                }
                Configuration.ComboPresets = newConfig;
            }
            var flagsSelected = new bool[orderedByClassJob.Length];
            for (var i = 0; i < orderedByClassJob.Length; i++)
            {
                flagsSelected[i] = Configuration.ComboPresets[i];
            }

            ImGui.SetWindowSize(new Vector2(750, (30 + ImGui.GetStyle().ItemSpacing.Y) * 17));

            ImGui.Begin("XIVCombo", ref isImguiComboSetupOpen, ImGuiWindowFlags.NoScrollbar);

            ImGui.Text("This window allows you to enable and disable custom combos to your liking.");
            ImGui.Separator();

            ImGui.BeginChild("scrolling", new Vector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y)) * ImGuiHelpers.GlobalScale, true, ImGuiWindowFlags.HorizontalScrollbar);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            var lastClassJob = 0;

            for (var i = 0; i < orderedByClassJob.Length; i++)
            {
                var flag = orderedByClassJob[i];
                var flagInfo = flag.GetAttribute<CustomComboInfoAttribute>();
                if (lastClassJob != flagInfo.ClassJob)
                {
                    lastClassJob = flagInfo.ClassJob;
                    if (ImGui.CollapsingHeader(ClassJobToName((byte)lastClassJob)))
                    {
                        for (int j = i; j < orderedByClassJob.Length; j++)
                        {
                            flag = orderedByClassJob[j];
                            flagInfo = flag.GetAttribute<CustomComboInfoAttribute>();
                            if (lastClassJob != flagInfo.ClassJob)
                            {
                                break;
                            }
                            ImGui.PushItemWidth(200);
                            ImGui.Checkbox(flagInfo.FancyName, ref flagsSelected[(int)flag]);
                            ImGui.PopItemWidth();
                            ImGui.TextColored(new Vector4(0.68f, 0.68f, 0.68f, 1.0f), $"#{j+1}: " + flagInfo.Description);
                            ImGui.Spacing();
                        }
                        
                    }
                    
                }
            }

            for (var i = 0; i < orderedByClassJob.Length; i++)
            {
                Configuration.ComboPresets[i] = flagsSelected[i];
            }

            ImGui.PopStyleVar();

            ImGui.EndChild();

            ImGui.Separator();
            if (ImGui.Button("Save"))
            {
                PluginInterface.SavePluginConfig(Configuration);
                UpdateConfig();
            }
            ImGui.SameLine();
            if (ImGui.Button("Save and Close"))
            {
                PluginInterface.SavePluginConfig(Configuration);
                this.isImguiComboSetupOpen = false;
                UpdateConfig();
            }

            ImGui.End();
        }

        public void Dispose()
        {
            this.iconReplacer.Dispose();

            CommandManager.RemoveHandler("/pcombo");

            //PluginInterface.Dispose();
        }

        private void OnCommandDebugCombo(string command, string arguments)
        {
            var argumentsParts = arguments.Split();

            switch (argumentsParts[0])
            {
                case "setall":
                    {
                        foreach (int value in Enum.GetValues(typeof(CustomComboPreset)))
                        {
                            this.Configuration.ComboPresets[value] = true;
                        }

                        ChatGui.Print("all SET");
                    }
                    break;
                case "unsetall":
                    {
                        foreach (int value in Enum.GetValues(typeof(CustomComboPreset)))
                        {
                            this.Configuration.ComboPresets[value] = false;
                        }

                        ChatGui.Print("all UNSET");
                    }
                    break;
                case "set":
                    {
                        foreach (int value in Enum.GetValues(typeof(CustomComboPreset)))
                        {
                            if (value.ToString().ToLower() != argumentsParts[1].ToLower())
                                continue;

                            this.Configuration.ComboPresets[value] = true;
                        }
                    }
                    break;
                case "toggle":
                    {
                        foreach (int value in Enum.GetValues(typeof(CustomComboPreset)))
                        {
                            this.Configuration.ComboPresets[value] = !this.Configuration.ComboPresets[value];
                        }
                    }
                    break;

                case "unset":
                    {
                        foreach (int value in Enum.GetValues(typeof(CustomComboPreset)))
                        {
                            if (value.ToString().ToLower() != argumentsParts[1].ToLower())
                                continue;

                            this.Configuration.ComboPresets[value] = false;
                        }
                    }
                    break;

                //case "list":
                //    {
                //        foreach (var value in Enum.GetValues(typeof(CustomComboPreset)).Cast<CustomComboPreset>().Where(x => x != CustomComboPreset.None))
                //        {
                //            if (argumentsParts[1].ToLower() == "set")
                //            {
                //                if (this.Configuration.ComboPresets.HasFlag(value))
                //                    ChatGui.Print(value.ToString());
                //            }
                //            else if (argumentsParts[1].ToLower() == "all")
                //                ChatGui.Print(value.ToString());
                //        }
                //    }
                //    break;

                default:
                    this.isImguiComboSetupOpen = true;
                    break;
            }

            PluginInterface.SavePluginConfig(this.Configuration);
        }
    }
}
