#pragma once

#include <memory>
#include <Windows.h>


namespace ExplorerX::Natives {
	public class NativeShellFile {
	private:
		HICON hSmallIcon;
		HICON hLargeIcon;
	public:
		LPCWSTR const path;

		NativeShellFile(LPCWSTR path) : path(path) { }

		~NativeShellFile();

		HICON GetSmallIcon();
		HICON GetLargeIcon();

		void ClearSmallIcon();
		void ClearLargeIcon();

		std::shared_ptr<wchar_t> GetDisplayName() const;
		std::shared_ptr<wchar_t> GetTypeName() const;

		DWORD_PTR GetExeType() const;
	};
}


namespace ExplorerX::CLI {
	using namespace System;

	public enum class ExeType
	{
		Unknown,
		WindowsApp,
		MicrosoftDos,
		ConsoleApp
	};

	public ref class ShellFile
	{
	private:
		Natives::NativeShellFile* native;
	public:
		ShellFile(String^ path);

		property IntPtr^ SmallIconHandle {
			IntPtr^ get();
		}

		property IntPtr^ LargeIconHandle {
			IntPtr^ get();
		}

		property String^ DisplayName {
			String^ get();
		}

		property String^ TypeName {
			String^ get();
		}

		property ExeType FileType {
			ExeType get();
		}

		void ClearSmallIcon();
		void ClearLargeIcon();
	};
}
