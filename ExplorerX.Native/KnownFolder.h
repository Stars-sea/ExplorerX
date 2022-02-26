#pragma once

#include <ShlObj.h>
#include <KnownFolders.h>


namespace ExplorerX::CLI {
	using namespace System;

	public ref class KnownFolder
	{
	private:
		const GUID* const ID;
		KnownFolder(const GUID& id) : ID(&id) { }

		KnownFolder(const KnownFolder% folder) : ID(folder.ID) { }
		
	public:
		static const KnownFolder^ Desktop	= gcnew KnownFolder(FOLDERID_Desktop);
		static const KnownFolder^ Downloads	= gcnew KnownFolder(FOLDERID_Downloads);
		static const KnownFolder^ Documents	= gcnew KnownFolder(FOLDERID_Documents);
		static const KnownFolder^ Videos	= gcnew KnownFolder(FOLDERID_Videos);
		static const KnownFolder^ Music		= gcnew KnownFolder(FOLDERID_Music);
		static const KnownFolder^ Pictures	= gcnew KnownFolder(FOLDERID_Pictures);

		property String^ Path {
			String^ get();
		}
	};
}
