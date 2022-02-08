using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;


namespace ExplorerX.Data {
	public class RegistryManager<T> : IStorable, IReadOnlyDictionary<string, T> where T : notnull {
		public static readonly JsonSerializerOptions SerializerOptions = new() {
			WriteIndented = true
		};

		public string Name { get; init; }
		public string? Desc { get; init; }
		public int Count => entries.Count;

		[JsonPropertyName("Entries")]
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

		#region Public Medthods
		public IEnumerable<string> Keys => entries.Keys;
		public IEnumerable<T> Values => entries.Values;

		public bool Register(string key, T item) => entries.TryAdd(key, item);
		public T    Get(string key) => entries[key];
		public bool TryGetValue(string id, [NotNullWhen(true)] out T? value)
			=> (value = this[id]) is not null;
		public bool ContainsKey(string id) => entries.ContainsKey(id);

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
			=> entries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Deconstruct(out IEnumerable<string> keys, out IEnumerable<T> values) {
			keys    = Keys;
			values = Values;
		}
		#endregion

		#region IO Operations
		private static string GetPath(string dir, string name)
			=> Path.Combine(dir, $"{name}.json");

		public async ValueTask<bool> SaveAsync(string dir, CancellationToken token) {
			using FileStream stream = File.OpenWrite(GetPath(dir, Name));
			await JsonSerializer.SerializeAsync(stream, this, SerializerOptions, token);
			return true;
		}

		public async ValueTask<bool> ReadAsync(string dir, CancellationToken token) {
			using FileStream stream = File.OpenRead(GetPath(dir, Name));
			var @new = await JsonSerializer.DeserializeAsync<Dictionary<string, T>>(
				stream, SerializerOptions, token
			);

			if (@new is not null)
				foreach ((string key, T value) in @new)
					if (!ContainsKey(key)) Register(key, value);
			return true;
		}
		#endregion
	}

	public static class RegistryManagers {
		// 偷天换日
		private static async ValueTask PreInit<T>(RegistryManager<T> registry) where T : notnull {
			storables.Add(registry);
			await ((IStorable)registry).ReadAsync(ConfigPath.Registry, 3);
		}

		private static readonly List<IStorable> storables = new();

		public static readonly RegistryManager<object> VariablePool	= new();
		public static readonly RegistryManager<string> QuickAccess	= new();

		internal static async void PreInit() {
			await PreInit(VariablePool);
			await PreInit(QuickAccess);
		}

		public static async void SaveAll() {
			foreach (IStorable storable in storables)
				await storable.SaveAsync(ConfigPath.Registry, 3);
		}
	}
}
