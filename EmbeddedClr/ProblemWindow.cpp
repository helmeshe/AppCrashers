#include "pch.h"
#include <Windows.h>
#include <xutility>
#include <stdexcept>
#include "ProblemWindow.h"

namespace EmbeddedClr
{
	ProblemWindow::ProblemWindow()
	{
		m_checkBox = gcnew CheckBox();
		m_checkBox->Checked += gcnew RoutedEventHandler(this, &ProblemWindow::OnChecked);
		m_checkBox->HorizontalAlignment = Windows::HorizontalAlignment::Center;
		m_checkBox->VerticalAlignment = Windows::VerticalAlignment::Center;
		m_checkBox->RenderTransformOrigin = Point(0.5, 0.5);
		m_checkBox->RenderTransform = gcnew Windows::Media::ScaleTransform(5, 5);

		Content = m_checkBox;
	}

	void ProblemWindow::OnSourceInitialized(EventArgs^ e)
	{
		Window::OnSourceInitialized(e);

		Width = 320;
		Height = 150;
		Title = "Embedded Crasher";
		Topmost = true;

		auto source = (Interop::HwndSource^)PresentationSource::FromVisual(this);
		source->AddHook(gcnew Interop::HwndSourceHook(this, &ProblemWindow::ProblemHook));
	}

	void ProblemWindow::OnChecked(Object^ sender, RoutedEventArgs^ e)
	{
		m_enableCrash = true;
		m_checkBox->IsEnabled = false;
	}

	IntPtr ProblemWindow::ProblemHook(IntPtr, int msg, IntPtr, IntPtr, bool% handled)
	{
		if (msg == WM_GETTEXT)
		{
			if (m_enableCrash)
			{
				SetLastError(ERROR_INVALID_PARAMETER);
				handled = true;
			}
		}
		else if (msg == WM_GETTEXTLENGTH)
		{
			if (m_enableCrash)
			{
				throw std::invalid_argument("Invalid arguments!");
				handled = true;
			}
		}
		return IntPtr::Zero;
	}
}
