using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace ExplorerX.Data {
	/// <summary>
	/// <para>Dictionary with regular expression restriction key</para>
	/// <para>用正则限制键的字典</para>
	/// </summary>
	public class KeyRestrictionDict<TValue> : Dictionary<string, TValue> {
		public Regex? KeyRegex { get; init; }

		public bool IsReadOnly => false;

		public KeyRestrictionDict(Regex? keyRegex = null) : this(keyRegex, 0) { }
		public KeyRestrictionDict(Regex? keyRegex, int capacity) : base(capacity) {
			KeyRegex = keyRegex;
		}
		public KeyRestrictionDict(Regex? keyRegex, IDictionary<string, TValue> dict) : base(dict) {
			KeyRegex = keyRegex;
		}

		private void KeyCheck(string key) {
			if (KeyRegex is not null && !KeyRegex.IsMatch(key))
				throw new ArgumentException($"Key \"{key}\" dones't match /{KeyRegex}/");
		}

		public new TValue this[string key] {
			get => base[key];
			set {
				KeyCheck(key);
				base[key] = value;
			}
		}

		public new void Add(string key, TValue value) {
			KeyCheck(key);
			base.Add(key, value);
		}

		public new bool TryAdd(string key, TValue value) {
			if (KeyRegex is null || KeyRegex.IsMatch(key))
				return base.TryAdd(key, value);
			return false;
		}

		/// <summary>
		/// <para>Add multiple key/value pairs at one time</para>
		/// <para>一次性添加多个键值对</para>
		/// </summary>
		/// <returns>
		/// <para>Key/value pairs that failed to be added</para>
		/// <para>添加失败的键值对</para>
		/// </returns>
		public IDictionary<string, TValue> AddPairs(IDictionary<string, TValue> pairs)
			=> AddPairs((IEnumerable<KeyValuePair<string, TValue>>)pairs);

		public IDictionary<string, TValue> AddPairs(params KeyValuePair<string, TValue>[] pairs)
			=> AddPairs((IEnumerable<KeyValuePair<string, TValue>>)pairs);

		public IDictionary<string, TValue> AddPairs(IEnumerable<KeyValuePair<string, TValue>> pairs)
			=> pairs.Where(p => !TryAdd(p.Key, p.Value)).ToDictionary(p => p.Key, p => p.Value);
	}
}
