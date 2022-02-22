using ExplorerX.Events;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ExplorerX.Data {

	[JsonObject]
	public sealed record Registry<T>(
		[CallerMemberName]
		string	Name		= "",
		string? Desc		= null,
		Regex?	KeyRegex	= null
	) : IStorable, IReadOnlyDictionary<string, T> where T : notnull {

		[JsonIgnore] public int Count => entries.Count;

		[JsonIgnore] public IEnumerable<string> Keys => entries.Keys;
		[JsonIgnore] public IEnumerable<T> Values => entries.Values;

		[JsonProperty(PropertyName = "Entries")]
		private readonly KeyRestrictionDict<T> entries = new(KeyRegex);

		public event EventHandler<RegistryLoadedArgs<T>>?  RegLoaded;
		public event EventHandler<RegistryChangedArgs<T>>? RegChanged;

		#region Common Medthods
		[JsonConstructor]
		internal Registry(string name, string? desc, Regex? keyRegex, Dictionary<string, T> entries)
			: this(name, desc, keyRegex)
		{
			// Necessary, entries may have been manipulated with LINQ
			// 不知道为什么没了这句就读不了键值对 _(:з)∠)_, 也许用了 LINQ ?
			foreach (var entry in entries) { }
			this.entries = new(keyRegex, entries);
		}

		public T this[string key] {
			get => entries[key];
			set {
				if (!Register(key, value))
					throw new InvalidOperationException($"Failed to register {Name}.{key}");
			}
		}

		/// <summary>
		/// <para>Register a key/value pair</para>
		/// <para>注册一个键值对</para>
		/// </summary>
		public bool Register(string key, T value) {
			if (entries.TryAdd(key, value)) {
				RegChanged?.Invoke(this, new(
					new Dictionary<string, T> { [key] = value },
					RegistryChangedArgs<T>.OperationMode.Register
				));
				return true;
			}
			return false;
		}

		public void RegisterAll(IDictionary<string, T> entries) {
			var failed	  = this.entries.AddPairs(entries);
			var succeeded = failed.Any() ? entries.Except(failed) : entries;

			if (succeeded.Any())
				RegChanged?.Invoke(this, new(
					new Dictionary<string, T>(succeeded),
					RegistryChangedArgs<T>.OperationMode.Register
				));
		}

		public bool Unregister(string key) => Unregister(key, out _);

		/// <summary>
		/// <para>Unregister & get the key/value pair</para>
		/// <para>注销并取得键值对</para>
		/// </summary>
		public bool Unregister(string key, [NotNullWhen(true)] out T? value) {
			if (entries.Remove(key, out value)) {
				RegChanged?.Invoke(this, new(
					new Dictionary<string, T> { [key] = value },
					RegistryChangedArgs<T>.OperationMode.Unregister
				));
				return true;
			}
			return false;
		}

		public bool ModifyValue(string key, T @new, [NotNullWhen(true)] out T? old) {
			if (!TryGetValue(key, out old)) return false;

			entries[key] = @new;
			RegChanged?.Invoke(this, new(
				new Dictionary<string, T> { [key] = old },
				RegistryChangedArgs<T>.OperationMode.Modify
			));
			return true;
		}

		public bool TryGetValue(string key, [NotNullWhen(true)] out T? value)
			=> entries.TryGetValue(key, out value);

		public bool ContainsKey(string key) => entries.ContainsKey(key);

		IEnumerator IEnumerable.GetEnumerator() => entries.GetEnumerator();
		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
			=> entries.GetEnumerator();

		public override string ToString() => $"Registry[{Name}]";
		#endregion

		#region IO Operations
		private string GetPath(string dir) => Path.Combine(dir, $"{Name}.json");

		public bool Save(string dir) {
			using StreamWriter sw = new(File.OpenWrite(GetPath(dir)));
			sw.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
			sw.Flush();

			return true;
		}

		private bool Read_(string dir) {
			string path = GetPath(dir);
			if (!File.Exists(path)) {
				Trace.TraceWarning($"File not found while initializing {this}.");
				Trace.TraceInformation($"App will try to generate default entries in {this}.");
				return false;
			}

			Registry<T>? registry = ReadFromFile(path);
			if (registry is not null) {
				// Don't call RegChanged.
				// RegisterAll(registry.entries);
				entries.AddPairs(registry.entries);
				return true;
			}
			return false;
		}

		public bool Read(string dir) {
			bool result = Read_(dir);
			RegLoaded?.Invoke(this, new(result, this));
			return result;
		}

		public static Registry<T>? ReadFromFile(string path) {
			using StreamReader sr = new(File.OpenRead(path));
			return JsonConvert.DeserializeObject<Registry<T>>(sr.ReadToEnd());
		}
		#endregion
	}

	public static class RegistryManagers {

		private static Registry<T> Create<T>(
			Regex? regex = null,
			string? desc = null,
			[CallerMemberName] string name = ""
		) where T : notnull {
			Registry<T> registry = new(name, desc, regex);
			storables.Add(registry);
			return registry;
		}

		private static readonly List<IStorable> storables = new();

		public static readonly Registry<object> VariablePool = Create<object>(new(@"^(?i)\$[a-z]\w*$"));
		public static readonly Registry<string> QuickAccess	 = Create<string>();

		public static async ValueTask Reload() {
			await Task.WhenAll(storables.Select(
				async s => await s.ReadAsync(ConfigPath.Registry, 3)
			));
		}

		public static async ValueTask SaveAll() {
			await Task.WhenAll(storables.Select(
				async s => await s.SaveAsync(ConfigPath.Registry, 3)
			));
		}
	}
}
