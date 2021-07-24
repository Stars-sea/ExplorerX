using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ExplorerX.FileManager {

	public class WeakDict<TKey, TValue> : IDictionary<TKey, TValue>
		where TKey: notnull where TValue: class
	{
		protected Dictionary<TKey, WeakReference<TValue>> InnerDict = new();

		public TValue this[TKey key] {
			get {
				if (TryGetValue(key, out TValue? value))
					return value;
				throw new NullReferenceException();
			}
			set => InnerDict[key] = new WeakReference<TValue>(value);
		}

		public ICollection<TKey> Keys => InnerDict.Keys;

		public ICollection<TValue> Values =>
			new List<TValue>(
				from   reference in InnerDict.Values
				let    value = Unpack(reference)
				where  value is not null
				select value
			);
			

		public int Count => InnerDict.Count;

		public bool IsReadOnly => false;


		private static TValue? Unpack(WeakReference<TValue> reference) {
			if (reference.TryGetTarget(out TValue? value))
				return value;
			return null;
		}


		public void Add(TKey key, TValue value) => InnerDict.Add(key, new WeakReference<TValue>(value));
		public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

		public void Clear() => InnerDict.Clear();

		/// <summary>
		/// 清理 InnerDict 中值为 null 的项
		/// </summary>
		public void Clean() {
			IEnumerable<TKey> bad =
				from   pair in InnerDict
				where  !pair.Value.TryGetTarget(out TValue? value) || value is null
				select pair.Key;

			foreach (TKey key in bad)
				InnerDict.Remove(key);
		}

		public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
		public bool ContainsKey(TKey key) => InnerDict.ContainsKey(key);

		// TODO
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			foreach (KeyValuePair<TKey, WeakReference<TValue>> pair in InnerDict) {
				if (pair.Value.TryGetTarget(out TValue? value) && value != null)
					yield return new KeyValuePair<TKey, TValue>(pair.Key, value);
			}
		}

		public bool Remove(TKey key) => InnerDict.Remove(key);
		public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

		public bool TryGetValue(TKey key, [NotNullWhen(true)][MaybeNullWhen(false)] out TValue value) {
			if (InnerDict.TryGetValue(key, out WeakReference<TValue>? reference) &&
				reference is not null && reference.TryGetTarget(out value)) {
				return value is not null;
			}

			Remove(key);
			value = null;
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
