﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;


namespace ExplorerX.Data {
	public class RegistryManager<T> : IEnumerable<KeyValuePair<string, T>> where T : notnull {
		public string Name { get; init; }
		public string? Desc { get; init; }
		public int Count => entries.Count;

		private readonly Dictionary<string, T> entries = new();

		// TODO: Add registry id check (using Regex)
		public RegistryManager([CallerMemberName] string name = "", string? desc = null) {
			Name = name;
			Desc = desc;
		}

		public T this[string id] {
			get => Get(id);
			set {
				if (!Register(id, value))
					throw new InvalidOperationException($"Failed to register {Name}.{id}");
			}
		}

		public bool Register(string id, T item) => entries.TryAdd(id, item);
		public T    Get(string id) => entries[id];
		public bool TryGet(string id, [NotNullWhen(true)] out T? value)
			=> (value = this[id]) is not null;
		public bool Exist(string id) => entries.ContainsKey(id);

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
			=> entries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerable<string> GetIDs() => entries.Keys;
		public IEnumerable<T> GetValues() => entries.Values;

		public void Deconstruct(out IEnumerable<string> ids, out IEnumerable<T> values) {
			ids    = GetIDs();
			values = GetValues();
		}
	}

	public static class RegistryManagers {
		public static readonly RegistryManager<object>  VariablePool    = new();
		public static readonly RegistryManager<string>	QuickAccess		= new();
	}
}