#pragma once

using namespace System;
using namespace System::Windows;
using namespace System::Windows::Controls;

namespace EmbeddedClr
{
	ref class ProblemWindow : Window
	{
	public:
		ProblemWindow();

	protected:
		void OnSourceInitialized(EventArgs^ e) override;

	private:
		void OnChecked(Object^ sender, RoutedEventArgs^ e);
		IntPtr ProblemHook(IntPtr, int, IntPtr, IntPtr, bool%);

		CheckBox^ m_checkBox;
		bool m_enableCrash = false;
	};
}
