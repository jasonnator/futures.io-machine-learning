// StartDetached.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <iostream>
#include <string>
#include <windows.h>

#include "StartDetached.h"

int __cdecl wmain(int argc, wchar_t** argv)
{
	std::wstring commandLine;

	if (argc < 2)
	{
		wprintf(L"Usage: %s [command/program] [arguments]\n", argv[0]);
		return 1;
	}

	for (int i = 1; argv[i]; ++i)
	{
		std::wstring argument(argv[i]);
		std::wstring escapedArgument;

		::ArgvQuote(argument, escapedArgument, false);

		commandLine.append(escapedArgument);
		commandLine.push_back(L' ');
	}

	STARTUPINFO si = { sizeof(si) };
	PROCESS_INFORMATION pi;

	// if not using C++11 or later, force a zero at the end to make sure path is null-ternminated
	commandLine.push_back(0);

	if (::CreateProcessW(0, &commandLine[0], NULL, NULL, FALSE, DETACHED_PROCESS, NULL, NULL, &si, &pi))
	{
		////WaitForSingleObject(pi.hProcess, INFINITE);
		::CloseHandle(pi.hProcess);
		::CloseHandle(pi.hThread);
		return 0;
	}
	else
	{
		wchar_t* messageBuffer;

		if (::FormatMessageW(
			FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_ALLOCATE_BUFFER,
			NULL,
			::GetLastError(),
			MAKELANGID(LANG_NEUTRAL, SUBLANG_SYS_DEFAULT),
			(wchar_t*)&messageBuffer,
			0,
			NULL))
		{
			std::wcout << messageBuffer << std::endl;
			::LocalFree(messageBuffer);
		}
		else
		{
			wprintf(L"Format message failed with 0x%x\n", ::GetLastError());
		}

		return -1;
	}

	return 0;
}