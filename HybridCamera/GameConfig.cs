﻿using System;
using System.Collections.Generic;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Common.Configuration;

// Jacked from SimpleTweaks

namespace HybridCamera;

public static unsafe class GameConfig {
    public class EntryWrapper {
        public string Name { get; }
        public ConfigEntry* Entry { get; }

        public object? Value {
            get {
                return Entry->Type switch {
                    2 => Entry->Value.UInt,
                    3 => Entry->Value.Float,
                    4 => Entry->Value.String->ToString(),
                    _ => null
                };
            }
            set {
                switch (Entry->Type) {
                    case 2 when value is uint u32: {
                            if (!Entry->SetValueUInt(u32)) {
                                throw new Exception("Failed");
                            }
                            break;
                        }
                    case 3 when value is float f: {
                            if (!Entry->SetValueFloat(f)) {
                                throw new Exception("Failed");
                            }
                            break;
                        }
                    case 4 when value is string s: {
                            if (!Entry->SetValueString(s)) {
                                throw new Exception("Failed");
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException("Invalid Value");
                }
            }
        }


        public EntryWrapper(ConfigEntry* entry, string name) {
            Name = name;
            Entry = entry;
        }
    }

    public class GameConfigSection {

        public readonly ConfigBase* configBase;

        private readonly Dictionary<string, uint> indexMap = new();
        private readonly Dictionary<uint, string> nameMap = new();

        private string[] ignoredNames = Array.Empty<string>();

        public uint ConfigCount => configBase->ConfigCount;

        public GameConfigSection(ConfigBase* configBase, string[] ignoredNames = null) {
            this.configBase = configBase;

            if (ignoredNames != null) {
                this.ignoredNames = ignoredNames;
            }

            var e = configBase->ConfigEntry;
            for (var i = 0U; i < configBase->ConfigCount; i++, e++) {
                if (e->Name == null) continue;
                var eName = MemoryHelper.ReadStringNullTerminated(new IntPtr(e->Name));
                if (!indexMap.ContainsKey(eName)) indexMap.Add(eName, i);
            }
        }

        public EntryWrapper? this[uint i] {
            get {
                if (i >= configBase->ConfigCount) return null;

                var e = configBase->ConfigEntry;
                e += i;
                if (e->Name == null) return null;

                if (!nameMap.TryGetValue(i, out var name)) {
                    name = MemoryHelper.ReadStringNullTerminated(new IntPtr(e->Name));
                    nameMap.TryAdd(i, name);
                    indexMap.TryAdd(name, i);
                }

                return new EntryWrapper(e, name);

            }
        }

        public EntryWrapper? this[string name] {
            get {
                if (!TryGetIndex(name, out var i)) return null;
                var e = configBase->ConfigEntry;
                e += i;
                if (e->Name == null) return null;
                return new EntryWrapper(e, name);
            }
        }

        public bool TryGetEntry(string name, out EntryWrapper result, StringComparison? nameComparison = null) {
            result = null;
            if (!TryGetIndex(name, out var i, nameComparison)) return false;
            var e = configBase->ConfigEntry;
            e += i;
            if (e->Name == null) return false;
            result = new EntryWrapper(e, name);
            return true;
        }

        public bool TryGetName(uint index, out string? name) {
            name = null;
            if (index >= configBase->ConfigCount) return false;
            var hasName = nameMap.TryGetValue(index, out name);
            if (hasName) return name != null;
            var e = configBase->ConfigEntry;
            e += index;
            if (e->Name == null) return false;
            name = MemoryHelper.ReadStringNullTerminated(new IntPtr(e->Name));
            indexMap.TryAdd(name, index);
            nameMap.TryAdd(index, name);
            return true;
        }

        public bool TryGetIndex(string name, out uint index, StringComparison? stringComparison = null) {
            if (indexMap.TryGetValue(name, out index)) return true;
            var e = configBase->ConfigEntry;
            for (var i = 0U; i < configBase->ConfigCount; i++, e++) {
                if (e->Name == null) continue;
                var eName = MemoryHelper.ReadStringNullTerminated(new IntPtr(e->Name));
                if (eName.Equals(name)) {
                    indexMap.TryAdd(name, i);
                    nameMap.TryAdd(i, name);
                    index = i;
                    return true;
                }
            }
            index = 0;
            return false;
        }

        private bool TryGetEntry(uint index, out ConfigEntry* entry) {
            entry = null;
            if (configBase->ConfigEntry == null || index >= configBase->ConfigCount) return false;
            entry = configBase->ConfigEntry;
            entry += index;
            return true;
        }

        public bool TryGetBool(string name, out bool value) {
            value = false;
            if (!TryGetIndex(name, out var index)) return false;
            if (!TryGetEntry(index, out var entry)) return false;
            value = entry->Value.UInt != 0;
            return true;
        }

        public bool GetBool(string name) {
            if (!TryGetBool(name, out var value)) throw new Exception($"Failed to get Bool '{name}'");
            return value;
        }

        public void Set(string name, bool value) {
            if (!TryGetIndex(name, out var index)) return;
            if (!TryGetEntry(index, out var entry)) return;
            entry->SetValue(value ? 1U : 0U);
        }

        public bool TryGetUInt(string name, out uint value) {
            value = 0;
            if (!TryGetIndex(name, out var index)) return false;
            if (!TryGetEntry(index, out var entry)) return false;
            value = entry->Value.UInt;
            return true;
        }

        public uint GetUInt(string name) {
            if (!TryGetUInt(name, out var value)) throw new Exception($"Failed to get UInt '{name}'");
            return value;
        }

        public void Set(string name, uint value) {
            if (!TryGetIndex(name, out var index)) return;
            if (!TryGetEntry(index, out var entry)) return;
            entry->SetValue(value);
        }

        public bool TryGetFloat(string name, out float value) {
            value = 0;
            if (!TryGetIndex(name, out var index)) return false;
            if (!TryGetEntry(index, out var entry)) return false;
            value = entry->Value.Float;
            return true;
        }

        public float GetFloat(string name) {
            if (!TryGetFloat(name, out var value)) throw new Exception($"Failed to get Float '{name}'");
            return value;
        }

        public void Set(string name, float value) {
            if (!TryGetIndex(name, out var index)) return;
            if (!TryGetEntry(index, out var entry)) return;
            entry->SetValue(value);
        }

        public bool TryGetString(string name, out string value) {
            value = string.Empty;
            if (!TryGetIndex(name, out var index)) return false;
            if (!TryGetEntry(index, out var entry)) return false;
            if (entry->Type != 4) return false;
            if (entry->Value.String == null) return false;
            value = entry->Value.String->ToString();
            return true;
        }

        public string GetString(string name) {
            if (!TryGetString(name, out var value)) throw new Exception($"Failed to get String '{name}'");
            return value;
        }

        public void Set(string name, string value) {
            if (!TryGetIndex(name, out var index)) return;
            if (!TryGetEntry(index, out var entry)) return;
            entry->SetValue(value);
        }
    }

    static GameConfig() {
        System = new GameConfigSection(&Framework.Instance()->SystemConfig.CommonSystemConfig.ConfigBase, new[] { "PadMode" });
        UiConfig = new GameConfigSection(&Framework.Instance()->SystemConfig.CommonSystemConfig.UiConfig);
        UiControl = new GameConfigSection(&Framework.Instance()->SystemConfig.CommonSystemConfig.UiControlConfig);
    }


    public static GameConfigSection System;
    public static GameConfigSection UiConfig;
    public static GameConfigSection UiControl;
}

