#include "pch.h"
#include "ShellFile.h"

#include <vcclr.h>

using namespace System;
using namespace ExplorerX;


#pragma region Natives::NativeShellFile

Natives::NativeShellFile::~NativeShellFile()
{
	ClearSmallIcon();
	ClearLargeIcon();
}

static inline SHFILEINFO GetInfo(LPCWSTR path, UINT flag) {
	SHFILEINFO info = { 0 };
	SHGetFileInfo(path, FILE_ATTRIBUTE_NORMAL, &info, sizeof(info), flag);
	return info;
}

HICON Natives::NativeShellFile::GetSmallIcon()
{
	if (hSmallIcon != nullptr) return hSmallIcon;
	return hSmallIcon = GetInfo(path, SHGFI_ICON | SHGFI_SMALLICON).hIcon;
}

HICON Natives::NativeShellFile::GetLargeIcon()
{
	if (hLargeIcon != nullptr) return hLargeIcon;
	return hLargeIcon = GetInfo(path, SHGFI_ICON | SHGFI_LARGEICON).hIcon;
}

void Natives::NativeShellFile::ClearSmallIcon()
{
	if (hSmallIcon != nullptr) {
		DestroyIcon(hSmallIcon);
		hSmallIcon = nullptr;
	}
}

void Natives::NativeShellFile::ClearLargeIcon()
{
	if (hLargeIcon != nullptr) {
		DestroyIcon(hLargeIcon);
		hLargeIcon = nullptr;
	}
}

std::shared_ptr<wchar_t> Natives::NativeShellFile::GetDisplayName() const
{
	return std::shared_ptr<wchar_t>(GetInfo(path, SHGFI_DISPLAYNAME).szDisplayName);
}

std::shared_ptr<wchar_t> Natives::NativeShellFile::GetTypeName() const
{
	return std::shared_ptr<wchar_t>(GetInfo(path, SHGFI_TYPENAME).szTypeName);
}

DWORD_PTR Natives::NativeShellFile::GetExeType() const
{
	return SHGetFileInfo(path, -1, NULL, sizeof(NULL), SHGFI_EXETYPE);
}

#pragma endregion

#pragma region Native::ShellFile

CLI::ShellFile::ShellFile(String^ path)
{
	pin_ptr<const wchar_t> path_ptr = PtrToStringChars(path);
	native = new Natives::NativeShellFile(path_ptr);
}

IntPtr^ CLI::ShellFile::SmallIconHandle::get() {
	using System::Runtime::InteropServices::Marshal;
	return IntPtr(native->GetSmallIcon());
}

IntPtr^ CLI::ShellFile::LargeIconHandle::get() {
	return IntPtr(native->GetLargeIcon());
}

String^ CLI::ShellFile::DisplayName::get() {
	return gcnew String(native->GetDisplayName().get());
}

String^ CLI::ShellFile::TypeName::get() {
	return gcnew String(native->GetTypeName().get());
}

CLI::ExeType CLI::ShellFile::FileType::get() {
	static const int
		PE = 0x4550,
		NE = 0x454E,
		MZ = 0x5A4D;
	auto a = *PtrToStringChars(gcnew String(""));
	DWORD_PTR hr = native->GetExeType();
	if (hr == -1) return CLI::ExeType::Unknown;

	WORD low = LOWORD(hr),
		high = HIWORD(hr);

	if (low == PE && high == 0)
		return CLI::ExeType::ConsoleApp;
	if (low == MZ && high == 0)
		return CLI::ExeType::MicrosoftDos;
	if (low == PE || low == NE)
		return CLI::ExeType::WindowsApp;
	return CLI::ExeType::Unknown;
}

void CLI::ShellFile::ClearSmallIcon() { native->ClearSmallIcon(); }
void CLI::ShellFile::ClearLargeIcon() { native->ClearLargeIcon(); }

#pragma endregion
