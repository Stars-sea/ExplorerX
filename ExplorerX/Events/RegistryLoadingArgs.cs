using System;

namespace ExplorerX.Events {
	/// <summary>
	/// <para>When the program starts to load the registry, 
	/// you can modifier entries by using it.</para>
	/// <para>当程序启动加载注册表时, 可通过它操作条目</para>
	/// </summary>
	/// <param name="Add">
	/// Add a entry with its name & value.
	/// 添加条目的名称和值
	/// </param>
	/// <param name="Exist">
	/// Examine whether the incoming name has been used.
	/// 检测传入的名称是否被占用
	/// </param>
	public record RegistryLoadingArgs<T>(
		Func<string, T, bool> Add,
		Func<string, bool> Exist
	);
}
