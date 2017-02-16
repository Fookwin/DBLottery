package com.fookwin.lotteryspirit.view;


import android.content.res.Resources;
import android.support.v7.widget.GridLayout;
import android.support.v7.widget.GridLayout.Spec;
import android.util.TypedValue;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import android.widget.TextView;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.util.StringFormater;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.DiagramOptions;

public class DiagramHeaderView
{	
	private GridLayout header_general;
	private GridLayout header_specific;
	private int unit_length = -1;
	private DiagramOptions options;
	
	public void setOptions(DiagramOptions op)
	{
		options = op;
	}
	
	public void addToContainer(LinearLayout container)
	{
		// build the general header grid.
		if (header_general == null)
		{
			header_general = (GridLayout) GridLayout.inflate(LotterySpirit.getInstance(), R.layout.grid_header_general,null);	
		}
		
		LayoutParams params = new LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);	    
		container.addView(header_general, params);
		
		// calculate the width of unit.
		Resources res = container.getResources();
		unit_length = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 25, res.getDisplayMetrics());
	
		// add the detailed header grid.	
		header_specific = (GridLayout) GridLayout.inflate(LotterySpirit.getInstance(), R.layout.grid_header_template,null);
		container.addView(header_specific, params);
	}
	
	public void refreshContentHeader()
	{
		header_specific.removeAllViews();
		
		switch (options.category)
		{
		case RedDivision:
			buildHeaderForRedDevision();
			break;
		case BlueGeneral:
			buildBlueForBlueGeneral();
			break;
		case BlueSpan:
			buildHeaderBlueSpan();
			break;
		case RedGeneral:
			buildHeaderForRedGenreal();
			break;
		case RedPosition:
			buildHeaderForRedPosition();
			break;
		default:
			break;			
		}
		
		header_specific.invalidate();
	}
	
	private void buildHeaderForRedDevision()
	{		
		int row = 0;
	
		// add division row.
		switch (options.subCategory)
		{
		case RedDivisionIn11:
			{
				for (int i = 1; i <= 11; ++ i)
				{
					String divName = Integer.toString(i) + "分区";
					addHeaderCell(header_specific, divName, row, 1, (i - 1) * 3, 3);
				}
				break;
			}
		case RedDivisionIn3:
		{
			for (int i = 1; i <= 3; ++ i)
			{
				String divName = Integer.toString(i) + "分区";
				addHeaderCell(header_specific, divName, row, 1, (i - 1) * 11, 11);
			}
			break;
		}
		case RedDivisionIn4:
		{
			for (int i = 1; i <= 4; ++ i)
			{
				String divName = Integer.toString(i) + "分区";
				int start = i > 2 ? ((i - 1) * 8 + 1) : (i - 1) * 8;
				addHeaderCell(header_specific, divName, row, 1, start, 8);
			}
			
			addHeaderCell(header_specific, "中", row, 1, 16, 1);
			
			break;
		}
		case RedDivisionIn7:
		{
			for (int i = 1; i <= 7; ++ i)
			{
				String divName = Integer.toString(i) + "分区";
				int width = i == 7 ? 3 : 5;
				addHeaderCell(header_specific, divName, row, 1, (i - 1) * 5, width);
			}
			break;
		}
		default:
			break;		
		}
		
		// add numbers row.
		row = 1;
		for (int i = 1; i <= 33; ++ i)
		{
			addHeaderCell(header_specific, StringFormater.padLeft(Integer.toString(i), 2, '0'),
					row, 1, i - 1, 1);
		}
	}
	
	private void buildHeaderForRedGenreal()
	{
		int row = 0;
		
		// add row of general block.
		addHeaderCell(header_specific, "和值", row, 2, 0, 2);
		addHeaderCell(header_specific, "连号", row, 2, 2, 1);
		addHeaderCell(header_specific, "奇偶", row, 1, 3, 2);
		addHeaderCell(header_specific, "大小", row, 1, 5, 2);
		addHeaderCell(header_specific, "质合", row, 1, 7, 2);
		addHeaderCell(header_specific, "除3余数", row, 1, 9, 3);
		addHeaderCell(header_specific, "三分区", row, 1, 12, 3);
		
		// add third row of general block
		row = 1;
		int startCol = 3;
		String[] cols = new String[]{"奇","偶","大","小","质","合","1","2","3","小","中","大"};
		for (String col : cols)
		{
			addHeaderCell(header_specific, col, row, 1, startCol++, 1);
		}
		
		// build extend block according to view option
		String title = null;
		switch (options.viewOption)
		{
		case RedDiv1Detail:
		{
			if (title == null)
				title = "3分区1区个数分布";
		}
		case RedDiv2Detail:
		{
			if (title == null)
				title = "3分区2区个数分布";
		}
		case RedDiv3Detail:
		{
			if (title == null)
				title = "3分区3区个数分布";
		}
		case RedRemain0Detail:
		{
			if (title == null)
				title = "除3余0个数分布";
		}
		case RedRemain1Detail:
		{
			if (title == null)
				title = "除3余1个数分布";
		}
		case RedRemain2Detail:
		{
			if (title == null)
				title = "除3余2个数分布";
			
			// first row
			addHeaderCell(header_specific, title, 0, 1, 15, 14);
			
			// second row
			addHeaderCell(header_specific, "0个", 1, 1, 15, 2);
			addHeaderCell(header_specific, "1个", 1, 1, 17, 2);
			addHeaderCell(header_specific, "2个", 1, 1, 19, 2);
			addHeaderCell(header_specific, "3个", 1, 1, 21, 2);
			addHeaderCell(header_specific, "4个", 1, 1, 23, 2);
			addHeaderCell(header_specific, "5个", 1, 1, 25, 2);
			addHeaderCell(header_specific, "6个", 1, 1, 27, 2);
			
			break;
		}
		case RedEvenOddDetail:
		{
			if (title == null)
				title = "奇偶数分布(奇-偶)";
		}
		case RedBigSmallDetail:
		{
			if (title == null)
				title = "大小数分布(大-小)";
		}	
		case RedPrimaryCompositeDetail:
		{
			if (title == null)
				title = "质合数分布(质-合)";
			
			// first row
			addHeaderCell(header_specific, title, 0, 1, 15, 14);
			
			// second row
			addHeaderCell(header_specific, "0-6", 1, 1, 15, 2);
			addHeaderCell(header_specific, "1-5", 1, 1, 17, 2);
			addHeaderCell(header_specific, "2-4", 1, 1, 19, 2);
			addHeaderCell(header_specific, "3-3", 1, 1, 21, 2);
			addHeaderCell(header_specific, "4-2", 1, 1, 23, 2);
			addHeaderCell(header_specific, "5-1", 1, 1, 25, 2);
			addHeaderCell(header_specific, "6-0", 1, 1, 27, 2);
			break;
		}
		case RedSumDetail:
		{
			// first row
			addHeaderCell(header_specific, "和值分布", 0, 1, 15, 18);
			
			// second row
			addHeaderCell(header_specific, "小于71", 1, 1, 15, 2);
			addHeaderCell(header_specific, "71~80", 1, 1, 17, 2);
			addHeaderCell(header_specific, "81~90", 1, 1, 19, 2);
			addHeaderCell(header_specific, "91~100", 1, 1, 21, 2);
			addHeaderCell(header_specific, "101~110", 1, 1, 23, 2);
			addHeaderCell(header_specific, "111~120", 1, 1, 25, 2);
			addHeaderCell(header_specific, "121~130", 1, 1, 27, 2);
			addHeaderCell(header_specific, "131~140", 1, 1, 29, 2);
			addHeaderCell(header_specific, "大于140", 1, 1, 31, 2);
			
			break;
		}
		case RedContinuityDetail:
		{
			// first row
			addHeaderCell(header_specific, "连号个数分布", 0, 1, 15, 12);
			
			// second row
			addHeaderCell(header_specific, "0个", 1, 1, 15, 2);
			addHeaderCell(header_specific, "1个", 1, 1, 17, 2);
			addHeaderCell(header_specific, "2个", 1, 1, 19, 2);
			addHeaderCell(header_specific, "3个", 1, 1, 21, 2);
			addHeaderCell(header_specific, "4个", 1, 1, 23, 2);
			addHeaderCell(header_specific, "5个", 1, 1, 25, 2);
			
			break;
		}
		default:
			break;
		}		
	}
	
	private void buildHeaderForRedPosition()
	{
		// add the first row.
		int row = 0;
		addHeaderCell(header_specific, "红球定位", row, 1, 0, 18);

		// add numbers row.
		row = 1;
		for (int i = 1; i <= 18; ++ i)
		{
			String text = "";
			switch (options.subCategory)
			{
			case RedPosition1:
				if (i == 18)
					text = ">17";
				else 
					text = StringFormater.padLeft(Integer.toString(i), 2, '0');		
				break;
			case RedPosition2:
				if (i == 18)
					text = ">18";
				else 
					text = StringFormater.padLeft(Integer.toString(i + 1), 2, '0');	
				break;
			case RedPosition3:
				if (i == 18)
					text = ">22";
				else if (i == 1)
					text = "<07";
				else 
					text = StringFormater.padLeft(Integer.toString(i + 5), 2, '0');	
				break;
			case RedPosition4:
				if (i == 18)
					text = ">27";
				else if (i == 1)
					text = "<12";
				else 
					text = StringFormater.padLeft(Integer.toString(i + 10), 2, '0');	
				break;
			case RedPosition5:
				if (i == 1)
					text = "<16";
				else 
					text = StringFormater.padLeft(Integer.toString(i + 14), 2, '0');	
				break;
			case RedPosition6:
				if (i == 1)
					text = "<17";
				else 
					text = StringFormater.padLeft(Integer.toString(i + 15), 2, '0');	
				break;
			default:
				break;
			}
			
			addHeaderCell(header_specific, text, row, 1, i - 1, 1);
		}
		
		// add attribute columns.
		row = 0;
		addHeaderCell(header_specific, "奇偶", row, 1, 18, 2);
		addHeaderCell(header_specific, "质合", row, 1, 20, 2);
		addHeaderCell(header_specific, "除3余数", row, 1, 22, 3);
		addHeaderCell(header_specific, "五行", row, 1, 25, 5);
		
		// add third row of general block
		row = 1;
		int startCol = 18;
		String[] cols = new String[]{"奇","偶","质","合","0", "1","2","金","木","水","火","土"};
		for (String col : cols)
		{
			addHeaderCell(header_specific, col, row, 1, startCol++, 1);
		}
	}
	
	private void buildBlueForBlueGeneral()
	{
		int row = 0;

		// add division row.
		for (int i = 1; i <= 4; ++ i)
		{
			String divName = Integer.toString(i) + "分区";
			addHeaderCell(header_specific, divName, row, 1, (i - 1) * 4, 4);
		}
		
		// add numbers row.
		row = 1;
		for (int i = 1; i <= 16; ++ i)
		{
			addHeaderCell(header_specific, StringFormater.padLeft(Integer.toString(i), 2, '0'),
					row, 1, i - 1, 1);
		}

		// add attribute columns.
		row = 0;
		addHeaderCell(header_specific, "奇偶", row, 1, 16, 2);
		addHeaderCell(header_specific, "大小", row, 1, 18, 2);
		addHeaderCell(header_specific, "质合", row, 1, 20, 2);
		addHeaderCell(header_specific, "除3余数", row, 1, 22, 3);
		addHeaderCell(header_specific, "五行", row, 1, 25, 5);
		
		// add third row of general block
		row = 1;
		int startCol = 16;
		String[] cols = new String[]{"奇","偶","大","小","质","合","0", "1","2","金","木","水","火","土"};
		for (String col : cols)
		{
			addHeaderCell(header_specific, col, row, 1, startCol++, 1);
		}
	}
	
	private void buildHeaderBlueSpan()
	{
		int row = 0;

		// add division row.
		addHeaderCell(header_specific, "跨度分布", row, 1, 0, 16);
				
		// add numbers row.
		row = 1;
		for (int i = 0; i < 16; ++ i)
		{
			addHeaderCell(header_specific, StringFormater.padLeft(Integer.toString(i), 2, '0'),
					1, 1, i, 1);
		}

		// add attribute columns.
		row = 0;
		addHeaderCell(header_specific, "奇偶", row, 1, 16, 2);
		addHeaderCell(header_specific, "大小", row, 1, 18, 2);
		addHeaderCell(header_specific, "质合", row, 1, 20, 2);
		addHeaderCell(header_specific, "除3余数", row, 1, 22, 3);
		addHeaderCell(header_specific, "五行", row, 1, 25, 5);
		
		// add third row of general block
		row = 1;
		int startCol = 16;
		String[] cols = new String[]{"奇","偶","大","小","质","合","0", "1","2","金","木","水","火","土"};
		for (String col : cols)
		{
			addHeaderCell(header_specific, col, row, 1, startCol++, 1);
		}
	}
	
	private void addHeaderCell(GridLayout container, String text, 
			int row, int rowSpan, int col, int colSpan)
	{
		TextView view = (TextView) GridLayout.inflate(LotterySpirit.getInstance(), 
				R.layout.grid_header_cell_template,null);
		view.setText(text);

		Spec rowspec = GridLayout.spec(row, rowSpan); 
        Spec colspec = GridLayout.spec(col, colSpan);
        GridLayout.LayoutParams params = new GridLayout.LayoutParams(rowspec, colspec);
		container.addView(view, params);
		
		android.view.ViewGroup.LayoutParams realParams = view.getLayoutParams();
		realParams.height = unit_length * rowSpan;
		realParams.width = unit_length * colSpan;
	
		view.setLayoutParams(realParams);
	}
}
