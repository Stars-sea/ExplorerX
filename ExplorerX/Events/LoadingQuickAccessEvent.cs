using System;

namespace ExplorerX.Events {
	/// <summary>
	/// <para>When the program starts to load the quick access entry, 
	/// you can modifier entries by using it.</para>
	/// <para>当程序启动加载快速访问的条目时, 可通过它操作条目</para>
	/// </summary>
	/// <param name="Add">
	/// Add a entry with its name & path.
	/// 添加条目的名称和路径
	/// </param>
	/// <param name="Exist">
	/// Examine whether the incoming name has been used.
	/// 检测传入的名称是否被占用
	/// </param>
	public record LoadingQuickAccessItemsArgs(
		Func<string, string, bool> Add,
		Func<string, bool> Exist
	);
}
