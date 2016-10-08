#pragma once

class IEditSupport
{
	virtual bool SetText(const CString& str);
	virtual CString GetText() const;
};