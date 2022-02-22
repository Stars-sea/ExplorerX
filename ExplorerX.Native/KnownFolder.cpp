#include "pch.h"
#include "KnownFolder.h"

using namespace System;


String^ ExplorerX::CLI::KnownFolder::Path::get() {
	PWSTR path = L"";
	SHGetKnownFolderPath(*ID, 0, NULL, &path);
	return gcnew String(path);
}
