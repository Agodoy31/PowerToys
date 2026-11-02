#pragma warning(push)
#pragma warning(disable : 26466)
#include "CppUnitTest.h"
#pragma warning(pop)

#include "LaunchUIProcess.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace FileLocksmithExtUnitTests
{
    TEST_CLASS(LaunchUIProcessTests)
    {
    public:
        TEST_METHOD(LaunchUIProcess_ReturnsFileNotFound_WhenExecutableDoesNotExist)
        {
            wchar_t tempDirectory[MAX_PATH + 1] = {};
            const DWORD length = GetTempPathW(
                ARRAYSIZE(tempDirectory),
                tempDirectory);

            Assert::IsTrue(
                length > 0 && length < ARRAYSIZE(tempDirectory),
                L"The temporary directory must fit in the buffer.");

            wchar_t executablePath[MAX_PATH] = {};
            Assert::IsTrue(
                GetTempFileNameW(tempDirectory, L"FLT", 0, executablePath) != 0,
                L"Reserve a unique path in an existing directory.");

            Assert::IsTrue(
                DeleteFileW(executablePath) != FALSE,
                L"Remove the reserved file before the launch.");

            SetLastError(ERROR_ACCESS_DENIED);

            const HRESULT result = FileLocksmith::LaunchUIProcess(
                executablePath,
                SW_SHOWNORMAL);

            Assert::IsTrue(
                FAILED(result),
                L"Launching a missing executable must fail.");

            Assert::AreEqual(
                HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND),
                result,
                L"Return the error from process creation.");
        }
    };
}
