using ExplorerX.Data;

using System;
using System.Collections.Generic;

namespace ExplorerX.Events {
	/// <summary>
	/// <para>When the program starts to load the registry, 
	/// you can modifier entries by using it.</para>
	/// <para>当程序启动加载注册表时, 可通过它操作条目</para>
	/// </summary>
	public record RegistryLoadedArgs<T>(
		bool IsSuccess,
		Func<string, T, bool> Add,
		Func<string, bool> Remove,
		Func<string, bool> Exist,
		Action<IDictionary<string, T>> AddAll
	) where T : notnull {
		public RegistryLoadedArgs(bool isSuccess, Registry<T> r) : 
			this(isSuccess,
				r.Register, r.Unregister, 
				r.ContainsKey, r.RegisterAll
			) { }
	}
}
