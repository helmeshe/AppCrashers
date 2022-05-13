#include "pch.h"

#include <tchar.h>
#include <xutility>

#pragma comment(lib, "EmbeddedClr")

__declspec(dllimport) void ShowWpfWindow();

static bool g_windowShown = false;

LRESULT CALLBACK ForegroundIdleProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode == HC_ACTION)
	{
		if (!g_windowShown)
		{
			g_windowShown = true;
			ShowWpfWindow();
		}
	}
	return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

extern "C" __declspec(dllexport) HOOKPROC GetProcPtr()
{
	return ForegroundIdleProc;
}