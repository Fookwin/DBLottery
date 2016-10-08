package com.fookwin.lotterydata.data;

//C# TO JAVA CONVERTER WARNING: Java does not allow user-defined value types. The behavior of this class will differ from the original:
//ORIGINAL LINE: public struct NumState
public final class NumState
{
	public int m_iHit;
	public int m_iOmission;
	public int m_iMaxOmission;
	public int m_iAverageOmission;

	public NumState clone()
	{
		NumState varCopy = new NumState();

		varCopy.m_iHit = this.m_iHit;
		varCopy.m_iOmission = this.m_iOmission;
		varCopy.m_iMaxOmission = this.m_iMaxOmission;
		varCopy.m_iAverageOmission = this.m_iAverageOmission;

		return varCopy;
	}
}