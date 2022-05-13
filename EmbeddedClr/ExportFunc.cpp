#include "pch.h"

#include "ProblemWindow.h"

__declspec(dllexport) void ShowWpfWindow()
{
	EmbeddedClr::ProblemWindow^ window = gcnew EmbeddedClr::ProblemWindow();
	window->Show();
}
