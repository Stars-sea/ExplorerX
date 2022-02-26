#pragma once

namespace ExplorerX {
	/// <summary>
	/// 存放 C++\CLI 代码
	/// </summary>
	namespace CLI {
		public ref class NativeManager abstract sealed
		{
		public:
			static void Startup();
			static void Shutdown();
		};
	}

	/// <summary>
	/// 存放原生 C++ 代码
	/// </summary>
	namespace Native {}
}
