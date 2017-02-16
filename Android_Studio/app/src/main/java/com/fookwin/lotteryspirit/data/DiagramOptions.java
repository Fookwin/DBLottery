package com.fookwin.lotteryspirit.data;

public class DiagramOptions
{
	public enum ViewOption
	{
		None,
		RedHorizantalConnection,
		RedVerticalConnection,
		RedObliqueConnection,
		RedOddConnection,
		RedEvenConnection,
		RedObmissionBreak,
		RedSumDetail,
		RedContinuityDetail,
		RedEvenOddDetail,
		RedBigSmallDetail,
		RedPrimaryCompositeDetail,
		RedRemain0Detail,
		RedRemain1Detail,
		RedRemain2Detail,
		RedDiv1Detail,
		RedDiv2Detail,
		RedDiv3Detail
	}
	
	public enum SubCategory
	{
		None,
		RedDivisionIn3,
		RedDivisionIn4,
		RedDivisionIn7,
		RedDivisionIn11,
		RedPosition1,
		RedPosition2,
		RedPosition3,
		RedPosition4,
		RedPosition5,
		RedPosition6
	}
	
	public enum Category
	{
		RedGeneral,
		RedDivision,
		RedPosition,
		BlueGeneral,
		BlueSpan;
	}
	
	public Category category = Category.RedGeneral;
	public SubCategory subCategory = SubCategory.None;
	public ViewOption viewOption = ViewOption.RedSumDetail;
	
	public int calculateRowWidth()
	{
		switch (category)
		{
		case BlueGeneral:
		case BlueSpan:
			return 39; //9 + 16 + 14;
		case RedDivision:
			return 42; //9 + 33
		case RedGeneral:
			switch (viewOption)
			{
			case None:
			{
				return 24; //9 + 15
			}
			case RedDiv1Detail:
			case RedDiv2Detail:
			case RedDiv3Detail:
			case RedRemain0Detail:
			case RedRemain1Detail:
			case RedRemain2Detail:
			case RedEvenOddDetail:
			case RedBigSmallDetail:
			case RedPrimaryCompositeDetail:
			{
				return 38; //9 + 15 + 14
			}
			case RedSumDetail:
			{
				return 42; //9 + 15 + 18
			}
			case RedContinuityDetail:
			{
				return 36; //9 + 15 + 12
			}
			default:
				return -1;
			}
		case RedPosition:
			return 39; //9 + 18 + 12;
		default:
			return -1;		
		}
	}
	
	public boolean requireLineGraph()
	{
		switch (category)
		{
		case RedDivision:
			return false;
		case RedPosition:
		case BlueGeneral:
		case BlueSpan:
			return true;
		case RedGeneral:
			return viewOption != ViewOption.None;
		default:
			return false;	
		}
	}
	
	public static int getDivisionCount(SubCategory op)
	{
		switch (op)
		{
		case RedDivisionIn11:
		{
			return 11;
		}
		case RedDivisionIn3:
		{
			return 3;
		}
		case RedDivisionIn4:
		{					
			return 5;
		}
		case RedDivisionIn7:
		{
			return 7;
		}
		default:
			return -1;	
		}	
	}
	
	public static int getDivisionIndex(SubCategory op, int num)
	{
		switch (op)
		{
		case RedDivisionIn11:
		{
			return (num - 1) / 3;
		}
		case RedDivisionIn3:
		{
			return (num - 1) / 11;
		}
		case RedDivisionIn4:
		{
			if (num == 17)
				return 2;			
			
			return num < 17 ? (num - 1) / 8 : ((num - 2) / 8 + 1);
		}
		case RedDivisionIn7:
		{
			return (num - 1) / 5;
		}
		default:
			return -1;	
		}	
	}
}