using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ExplorerX.Data {
	/// <summary>
	/// <para>Path wrapping, which can iterate from the top-level directory to the current path.</para>
	/// <para>路径包装, 可从顶级目录迭代至当前路径</para>
	/// </summary>
	public sealed class PathContainer : IEnumerable<string> {

		public int Count => this.Count();
		public string Name => System.IO.Path.GetFileName(originPath);
		public string Parent => new StringBuilder().AppendJoin(null, this[..^2]).ToString();
		public IEnumerable<string> this[Range range] => this.Take(range);
		public string this[int index] => this.ElementAt(index);

		private readonly string originPath;
		private PathContainer(string originPath) => this.originPath = originPath;

		// 此处需要强制转换是为了不混淆
		public static explicit operator PathContainer(string path) => new(SimplifyPath(path));
		public static implicit operator string(PathContainer container) => container.originPath;

		public IEnumerator<string> GetEnumerator() {
			StringBuilder builder = new();
			foreach (char c in originPath) {
				builder.Append(c);

				// 不用 System.IO.Path.PathSeparator 是为了更好的兼容性
				if (c == '/' || c == '\\') {
					yield return builder.ToString();
					builder.Clear();
				}
			}
			if (builder.Length != 0)
				yield return builder.ToString();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// <para>Unpack method</para>
		/// <para>解包方法</para>
		/// </summary>
		/// <param name="parent">文件(夹)父路径</param>
		/// <param name="name">文件(夹)名</param>
		public void Deconstruct(out string parent, out string name) {
			parent = Parent;
			name   = Name;
		}

		public override string ToString() => originPath;

		public static string SimplifyPath(string origin) {
			PathContainer container	= new(origin);
			LinkedList<string> list = new();
			foreach (string segment in container) {
				string trimed = segment.TrimEnd('/', '\\');
				if (trimed.Equals("."))
					continue;
				if (trimed.Equals("..") && list.Any())
					list.RemoveLast();
				else
					list.Append(segment);
			}
			StringBuilder builder = new();
			return builder.AppendJoin(null, list).ToString();
		}
	}
}
