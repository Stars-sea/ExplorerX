﻿using ExplorerX.Events;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ExplorerX.Data {

	public sealed record Registry<T>(
		[CallerMemberName]
		string	Name		= "",
		string? Desc		= null,
		Regex?	KeyRegex	= null
	) : IStorable, IReadOnlyDictionary<string, T> where T : notnull {

		public int Count => entries.Count;

		public IEnumerable<string> Keys => entries.Keys;
		public IEnumerable<T> Values => entries.Values;

		private Entries entries = new();

		/// <summary>
		/// <para>When it is called, we can add default entries.</para>
		/// <para>当它被调用时, 我们可以添加默认条目</para>
		/// </summary>
		public event EventHandler<RegistryEventArgs<T>>? NotFoundFile;

		#region Common Medthods

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
		/// <returns>
		/// <see cref="true"/> if the key matches the regex and doesn't contains in the registry;
		/// otherwise, <see cref="false"/>
		/// </returns>
		public bool Register(string key, T value) {
			if (KeyRegex?.IsMatch(key) ?? true)
				return entries.TryAdd(key, value);
			return false;
		}

		public void RegisterAll(IDictionary<string, T> entries) {
			foreach ((string k, T v) in entries)
				Register(k, v);
		}

		public bool Unregister(string key) => entries.Remove(key);

		/// <summary>
		/// <para>Unregister & get the key/value pair</para>
		/// <para>注销并取得键值对</para>
		/// </summary>
		public bool Unregister(string key, [NotNullWhen(true)] out T? value) {
			if (entries.TryGetValue(key, out value))
				return entries.Remove(key);
			return false;
		}

		public bool TryGetValue(string key, [NotNullWhen(true)] out T? value)
			=> entries.TryGetValue(key, out value);

		public bool ContainsKey(string key) => entries.ContainsKey(key);

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
			=> entries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion

		#region IO Operations
		private string GetPath(string dir) => Path.Combine(dir, $"{Name}.xml");

		public bool Save(string dir) {
			using FileStream stream = File.OpenWrite(GetPath(dir));

			DataContractSerializer serializer = new(typeof(Builder));
			serializer.WriteObject(stream, new Builder(this));
			return true;
		}

		public bool Read(string dir) {
			string path = GetPath(dir);
			if (!File.Exists(path)) {
				Trace.TraceWarning($"Not found file when init registry {Name}.");
				if (NotFoundFile is not null) {
					Trace.TraceInformation($"App will try to generate default entries in registry {Name}.");
					NotFoundFile(this, new(Register, Unregister, ContainsKey, RegisterAll));
				}
				return false;
			}

			Builder? builder = Builder.GetFromFile(path);

			if (builder is not null)
				entries = builder.Entries;
			return true;
		}
		#endregion

		#region Inner Classes
		[CollectionDataContract(
			IsReference	= true,
			ItemName	= "Entry",
			KeyName		= "Key",
			ValueName	= "Value",
			Namespace	= "http://shcemas.explorerx.org/data"
		)]
		public sealed class Entries : Dictionary<string, T> { }

		[DataContract(Name = "Registry", Namespace = "http://shcemas.explorerx.org/data")]
		public sealed class Builder {

			[DataMember(IsRequired = true)]
			public string	Name { get; set; } = "";

			[DataMember(EmitDefaultValue = false)]
			public string?	Desc { get; set; }

			[DataMember(IsRequired = true)]
			public Entries	Entries { get; set; } = new();

			[DataMember(Name = "KeyRegex", EmitDefaultValue = false)]
			string? KeyPattern;
			public Regex? KeyRegex { get; set; }

			[OnSerializing]
			void OnSerializing(StreamingContext context) {
				KeyPattern = KeyRegex?.ToString();
			}

			[OnDeserialized]
			void OnDeserialized(StreamingContext context) {
				if (!string.IsNullOrWhiteSpace(KeyPattern))
					KeyRegex = new(KeyPattern);
			}

			public Registry<T> Build()
				=> new(Name, Desc, KeyRegex) { entries = Entries };

			public Builder(Registry<T> registry) {
				Name		= registry.Name;
				Desc		= registry.Desc;
				KeyRegex    = registry.KeyRegex;
				Entries     = registry.entries;
			}

			public static Builder? GetFromFile(string path) {
				using FileStream stream = File.OpenRead(path);

				DataContractSerializer serializer = new(typeof(Builder));
				return (Builder?)serializer.ReadObject(stream);
			}
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
