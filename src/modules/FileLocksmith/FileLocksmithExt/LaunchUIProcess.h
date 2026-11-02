#pragma once

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#include <windows.h>

#include <string>

namespace FileLocksmith
{
    inline HRESULT LaunchUIProcess(const std::wstring& executablePath, WORD showWindow)
    {
        STARTUPINFOW startupInfo = {};
        startupInfo.cb = sizeof(startupInfo);
        startupInfo.dwFlags = STARTF_USESHOWWINDOW;
        startupInfo.wShowWindow = showWindow;

        PROCESS_INFORMATION processInformation = {};
        std::wstring commandLine = L"\"" + executablePath + L"\"";

        if (!CreateProcessW(
                nullptr,
                commandLine.data(),
                nullptr,
                nullptr,
                TRUE,
                0,
                nullptr,
                nullptr,
                &startupInfo,
                &processInformation))
        {
            const DWORD error = GetLastError();
            return HRESULT_FROM_WIN32(error);
        }

        CloseHandle(processInformation.hProcess);
        CloseHandle(processInformation.hThread);

        return S_OK;
    }
}
