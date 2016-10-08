#pragma once

class ITextSupport
{
public:
	virtual bool SetText(const CString& str) = 0;
	virtual CString GetText() const = 0;
};