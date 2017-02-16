package com.fookwin.lotteryspirit.view;

import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

import android.content.res.Resources;
import android.support.v7.widget.GridLayout;
import android.support.v7.widget.GridLayout.Spec;
import android.util.TypedValue;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ListView;
import android.widget.TextView;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotterydata.util.AttributeUtil;
import com.fookwin.lotterydata.util.StringFormater;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.DiagramData;
import com.fookwin.lotteryspirit.data.DiagramOptions;
import com.fookwin.lotteryspirit.data.DiagramOptions.ViewOption;

public class DiagramListView
{
	private enum CellStateEnum
	{
		NORMAL,
		ALTERNATE1,
		ALTERNATE2,
		HIGHLIGHTE,
		DIM
	}
	
	private enum NumStateEnum
	{
		TEXT_GREY,
		TEXT_RED,
		TEXT_BLUE,
		TEXT_WHITE,
		BALL_RED,
		BALL_BLUE,
		BALL_HIGHLIGHT
	}
	
	private class DiagramListItem
	{
		public GridLayout grid_row;
		public LineGraphView line_graph;
		public ArrayList<View> grid_cells;
		
		public void formatCells(int col_count)
		{
			if (grid_cells == null)
				grid_cells = new ArrayList<View>();
			
			int exist_count = grid_cells.size();
			if (exist_count == col_count)
				return;
			
			for (int i = 0; i < exist_count; ++ i)
			{
				grid_cells.get(i).setVisibility(i < exist_count ? View.VISIBLE : View.GONE);					
			}
			
			if (exist_count < col_count)
			{
				// add more.
				for (int i = exist_count; i < col_count; ++ i)
				{
					// create a view and add to grid.
					View view = GridLayout.inflate(LotterySpirit.getInstance(), 
							R.layout.grid_content_cell_template,null);

					Spec rowspec = GridLayout.spec(0, 1); 
			        Spec colspec = GridLayout.spec(i, 1);
			        GridLayout.LayoutParams params = new GridLayout.LayoutParams(rowspec, colspec);
			        grid_row.addView(view, params);
					
					grid_cells.add(view);
				}
			}
		}
	}
	
	private class DiagramListAdapter extends BaseAdapter 
	{
		private int count = 30;
		private int unit_length = -1;
		private DiagramOptions options;
		
		// cache resources to be used here.
		private int dr_cell_normal;
		private int dr_cell_alt1;
		private int dr_cell_alt2;
		private int dr_cell_highlight;
		private int dr_cell_dim;
		
		private int dr_num_red;
		private int dr_num_blue;
		private int dr_num_highlight;
		
		private int clr_grey;
		private int clr_red;
		private int clr_white;
		private int clr_blue;
		private int clr_transparent;
	
		public DiagramListAdapter(DiagramOptions op, int width_unit)
		{
			options = op;
			unit_length = width_unit;
			
			// Initialize the data.
			count = _diagramData.size();
			
			// initialize the global colors used for grid. 
			Resources res = LotterySpirit.getInstance().getResources();			
		
			dr_cell_normal = R.drawable.background_grid_content_cell_normal;
			dr_cell_alt1 = R.drawable.background_grid_content_cell_alt;
			dr_cell_alt2 = R.drawable.background_grid_content_cell_alt2;
			dr_cell_highlight = R.drawable.background_grid_content_cell_hightlight;
			dr_cell_dim = R.drawable.background_grid_content_cell_dim;
			
			dr_num_red = R.drawable.background_grid_num_red;
			dr_num_blue = R.drawable.background_grid_num_blue;
			dr_num_highlight = R.drawable.background_grid_num_hightlight;
			
			clr_grey = res.getColor(R.color.grey);
			clr_red = res.getColor(R.color.coral);
			clr_white = res.getColor(R.color.white);
			clr_blue = res.getColor(R.color.skyblue);
			clr_transparent = res.getColor(R.color.transparent);
		}
		
		public int getCount() {
			return count;
		}

		public Object getItem(int position) {
			return null;
		}

		public long getItemId(int position) {
			return 0;
		}

		public View getView(int position, View convertView, ViewGroup parent) 
		{
			DiagramListItem tempItem = null;
			if (convertView == null)
			{
				convertView = GridLayout.inflate(LotterySpirit.getInstance(), R.layout.grid_content_row_template, null);
				
				tempItem = new DiagramListItem();								
				tempItem.grid_row = (GridLayout) convertView.findViewById(R.id.grid_view);
				tempItem.line_graph = (LineGraphView) convertView.findViewById(R.id.line_graph);
				tempItem.line_graph.setLineColor(convertView.getResources().getColor(R.color.grey));
				tempItem.line_graph.setLineWidth(3);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (DiagramListItem)convertView.getTag();
			}			

			try {
				buildListRow(position, tempItem);
				
			} catch (ParseException e) {
				e.printStackTrace();
			}
			
			// adjust the line graph width and visibility.
		    ViewGroup.LayoutParams params = tempItem.line_graph.getLayoutParams();
		    params.width = options.calculateRowWidth() * unit_length;
		    tempItem.line_graph.setLayoutParams(params);
		    tempItem.line_graph.setVisibility(options.requireLineGraph() ? View.VISIBLE : View.INVISIBLE);			    
			
			return convertView;
		}
		
		private void buildListRow(int position, DiagramListItem item) throws ParseException
		{
			DiagramData current = _diagramData.get(position);			
			if (current == null)
				return;
			switch (options.category)
			{
			case RedDivision:
			{
				item.formatCells(36);
				buildGeneralColumns(current, item);
				buildRowForRedDevision(current, item);
				break;
			}
			case BlueGeneral:
			{
				item.formatCells(33);
				buildGeneralColumns(current, item);
				buildRowForBlueGeneral(current, item);;
				break;
			}
			case BlueSpan:
			{
				item.formatCells(33);
				buildGeneralColumns(current, item);
				buildRowForBlueSpan(current, item);
				break;
			}
			case RedGeneral:
			{
				int columnCount = 17; // columns in general block.
					
				switch (options.viewOption)
				{
				case RedSumDetail:
					columnCount += 9;
					break;
				case RedContinuityDetail:
					columnCount += 6;
					break;
				case RedBigSmallDetail:
				case RedDiv1Detail:
				case RedDiv2Detail:
				case RedDiv3Detail:
				case RedEvenOddDetail:
				case RedPrimaryCompositeDetail:
				case RedRemain0Detail:
				case RedRemain1Detail:
				case RedRemain2Detail:
					columnCount += 7;
					break;
				default:
					break;				
				}
				 
				item.formatCells(columnCount);
				buildGeneralColumns(current, item);
				buildRowForRedGerenal(current, item);
				break;
			}
			case RedPosition:
			{
				item.formatCells(33);
				buildGeneralColumns(current, item);
				buildRowForRedPosition(current, item);
				break;
			}
			default:
				break;			
			}
		}

		private void buildGeneralColumns(DiagramData current, DiagramListItem item)
		{
			refreshCell(item.grid_cells.get(0), Integer.toString(current.getSource().getIssue()), 0, 3, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			refreshCell(item.grid_cells.get(1), current.getSource().getScheme().getRedsExp(), 1, 5, CellStateEnum.NORMAL, NumStateEnum.TEXT_RED);
			refreshCell(item.grid_cells.get(2), current.getSource().getScheme().getBlueExp(), 2, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_BLUE);
		}
		
		private void buildRowForBlueSpan(DiagramData lot, DiagramListItem item)
		{
			// get the data and the cell index to be at.
			int data = lot.getBlueSpan();
			int data_cell = data + 3;

			// add numbers columns.
			int colInx = 3;
			for (int i = 0; i < 16; ++i, ++colInx) {
				boolean hitted = colInx == data_cell;
				String text = hitted ? StringFormater.padLeft(
						Integer.toString(data), 2, '0') : "";

				boolean bAltCol = i < 4 || (i > 7 && i < 12);
				refreshCell(item.grid_cells.get(colInx), text, colInx, 1,
						bAltCol ? CellStateEnum.ALTERNATE1
								: CellStateEnum.NORMAL,
						hitted ? NumStateEnum.BALL_BLUE
								: NumStateEnum.TEXT_GREY);
			}

			// add attribute columns
			refreshCell(item.grid_cells.get(colInx), data % 2 == 0 ? "" : "奇",
					colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx), data % 2 == 1 ? "" : "偶",
					colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx), data > 7 ? "大" : "",
					colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx), data < 8 ? "小" : "",
					colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx),
					AttributeUtil.IsPrime(data) ? "质" : "", colInx, 1,
					CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx),
					AttributeUtil.IsPrime(data) ? "" : "合", colInx, 1,
					CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx), data % 3 == 0 ? "0" : "",
					colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx), data % 3 == 1 ? "1" : "",
					colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx), data % 3 == 2 ? "2" : "",
					colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx),
					AttributeUtil.IndexOf5Xing(data) == 1 ? "金" : "", colInx,
					1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx),
					AttributeUtil.IndexOf5Xing(data) == 2 ? "木" : "", colInx,
					1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx),
					AttributeUtil.IndexOf5Xing(data) == 3 ? "水" : "", colInx,
					1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx),
					AttributeUtil.IndexOf5Xing(data) == 4 ? "火" : "", colInx,
					1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++colInx;

			refreshCell(item.grid_cells.get(colInx),
					AttributeUtil.IndexOf5Xing(data) == 5 ? "土" : "", colInx,
					1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			
			// draw line graph.
			int pre_pos = lot.getPrevious() != null ? lot.getPrevious().getBlueSpan() + 1 : 0;
			int cur_pos = lot.getBlueSpan() + 1;
			int next_pos = lot.getNext() != null ? lot.getNext().getBlueSpan() + 1 : 0;
			
			refreshLineGraph(item.line_graph, lot.getPrevious() != null, lot.getNext() != null, true, pre_pos - cur_pos, cur_pos + 8, next_pos - cur_pos);
		}

		private void buildRowForBlueGeneral(DiagramData lot, DiagramListItem item) throws ParseException 
		{
			// get the data and the cell index to be at.
			int data = lot.getSource().getScheme().getBlue();
			int data_cell = data + 2;
			
			// add numbers columns.
			int colInx = 3;
			for (int i = 0; i < 16; ++ i, ++ colInx)
			{
				boolean hitted = colInx == data_cell;
				String text = hitted ? StringFormater.padLeft(Integer.toString(data), 2, '0') : 
					Integer.toString(lot.getSource().getStatus().getBlueNumStates()[i].getOmission());
				
				boolean bAltCol = i < 4 || (i > 7 && i < 12);
				refreshCell(item.grid_cells.get(colInx), 
						text, 
						colInx, 
						1,
						bAltCol ? CellStateEnum.ALTERNATE1 : CellStateEnum.NORMAL, 
						hitted ? NumStateEnum.BALL_BLUE : NumStateEnum.TEXT_GREY);
			}
			
			// add attribute columns			
			refreshCell(item.grid_cells.get(colInx), data % 2 == 0 ? "" : "奇", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data % 2 == 1 ? "" : "偶", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data > 8 ? "大" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data < 9 ? "小" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IsPrime(data) ? "质" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IsPrime(data) ? "" : "合", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data % 3 == 0 ? "0" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data % 3 == 1 ? "1" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data % 3 == 2 ? "2" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data) == 1 ? "金" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data) == 2 ? "木" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data) == 3 ? "水" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data) == 4 ? "火" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data) == 5 ? "土" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			
			// draw line graph.
			int pre_pos = lot.getPrevious() != null ? lot.getPrevious().getSource().getScheme().getBlue() : 0;
			int cur_pos = lot.getSource().getScheme().getBlue();
			int next_pos = lot.getNext() != null ? lot.getNext().getSource().getScheme().getBlue() : 0;
			
			refreshLineGraph(item.line_graph, lot.getPrevious() != null, lot.getNext() != null, true, pre_pos - cur_pos, cur_pos + 8, next_pos - cur_pos);
		}
		
		private void buildRowForRedPosition(DiagramData lot, DiagramListItem item) 
		{			
			// get the data and the cell index to be at.
			int[] data = getRedPositionData(lot.getSource());			
			
			// add numbers columns.
			int colInx = 3;
			for (int i = 0; i < 18; ++ i, ++ colInx)
			{
				boolean hitted = colInx == data[1];
				String text = hitted ? StringFormater.padLeft(Integer.toString(data[0]), 2, '0') : "";
				
				refreshCell(item.grid_cells.get(colInx), 
						text, 
						colInx, 
						1,
						CellStateEnum.NORMAL, 
						hitted ? NumStateEnum.BALL_RED : NumStateEnum.TEXT_GREY);
			}
			
			// add attribute columns			
			refreshCell(item.grid_cells.get(colInx), data[0] % 2 == 0 ? "" : "奇", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data[0] % 2 == 1 ? "" : "偶", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IsPrime(data[0]) ? "质" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IsPrime(data[0]) ? "" : "合", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data[0] % 3 == 0 ? "0" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data[0] % 3 == 1 ? "1" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), data[0] % 3 == 2 ? "2" : "", colInx, 1, CellStateEnum.ALTERNATE2, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data[0]) == 1 ? "金" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data[0]) == 2 ? "木" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data[0]) == 3 ? "水" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data[0]) == 4 ? "火" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			++ colInx;
			
			refreshCell(item.grid_cells.get(colInx), AttributeUtil.IndexOf5Xing(data[0]) == 5 ? "土" : "", colInx, 1, CellStateEnum.NORMAL, NumStateEnum.TEXT_GREY);
			
			// draw line graph.
			int pre_pos = lot.getPrevious() != null ? getRedPositionData(lot.getPrevious().getSource())[1] : 0;
			int cur_pos = data[1];
			int next_pos = lot.getNext() != null ? getRedPositionData(lot.getNext().getSource())[1] : 0;
			
			refreshLineGraph(item.line_graph, lot.getPrevious() != null, lot.getNext() != null, true, pre_pos - cur_pos, cur_pos + 6, next_pos - cur_pos);
		}
		
		private int[] getRedPositionData(Lottery lot)
		{
			int[] data = new int[] {0,0};
			
			switch (options.subCategory)
			{
			case RedPosition1:
				data[0] = lot.getScheme().Red(0);
				if (data[0] > 17)
					data[1] = 20;
				else 
					data[1] = data[0] + 2;		
				break;
			case RedPosition2:
				data[0] = lot.getScheme().Red(1);
				if (data[0] > 18)
					data[1] = 20;
				else 
					data[1] = data[0] + 1;
				break;
			case RedPosition3:
				data[0] = lot.getScheme().Red(2);
				if (data[0] > 22)
					data[1] = 20;
				else if (data[0] < 7)
					data[1] = 3;	
				else 
					data[1] = data[0] - 3;
				break;
			case RedPosition4:
				data[0] = lot.getScheme().Red(3);
				if (data[0] > 27)
					data[1] = 20;
				else if (data[0] < 12)
					data[1] = 3;	
				else 
					data[1] = data[0] - 8;
				break;
			case RedPosition5:
				data[0] = lot.getScheme().Red(4);
				if (data[0] < 16)
					data[1] = 3;
				else 
					data[1] = data[0] - 12;
				break;
			case RedPosition6:
				data[0] = lot.getScheme().Red(5);
				if (data[0] < 17)
					data[1] = 3;
				else 
					data[1] = data[0] - 13;
				break;
			default:
				break;
			}
			
			return data;
		}
		
		private void buildRowForRedGerenal(DiagramData lot, DiagramListItem item) 
		{
			Scheme data = lot.getSource().getScheme();
			
			// general block.
			int colinx = 3;
			refreshCell(item.grid_cells.get(colinx), Integer.toString(data.getSum()), colinx, 2, CellStateEnum.ALTERNATE1, NumStateEnum.TEXT_GREY);
			++ colinx;
			
			int[] remainBy3 = new int[] { 0,0,0 };
			data.GetRemainBy3(remainBy3);
			
			int[] smb = new int[] { 0,0,0 };
			data.GetSmallMiddleBig(smb);
			
			String[] general_cols = new String[]
					{
						Integer.toString(data.getContinuity()),
						Integer.toString(6 - data.getEvenNumCount()),
						Integer.toString(data.getEvenNumCount()),
						Integer.toString(6 - data.getSmallNumCount()),
						Integer.toString(data.getSmallNumCount()),
						Integer.toString(data.getPrimeNumCount()),
						Integer.toString(6 - data.getPrimeNumCount()),
						Integer.toString(remainBy3[0]),
						Integer.toString(remainBy3[1]),
						Integer.toString(remainBy3[2]),
						Integer.toString(smb[0]),
						Integer.toString(smb[1]),
						Integer.toString(smb[2])
					};
			
			for (String text : general_cols)
			{	
				CellStateEnum cellstate = CellStateEnum.ALTERNATE1;
				if (colinx == 4 ||colinx == 7 || colinx == 8 || colinx == 11 ||colinx == 12 ||colinx == 13)
					cellstate = CellStateEnum.NORMAL;
				
				refreshCell(item.grid_cells.get(colinx), text, colinx, 1, cellstate, NumStateEnum.TEXT_GREY);
				++ colinx;
			}	

			// expand block
			int[] attri = getRedAttributeData(lot.getSource());
			for (int i = 0; i < attri[2]; ++ i)
			{
				boolean isDataCell = (i == attri[1]);
				refreshCell(item.grid_cells.get(colinx),
						isDataCell ? Integer.toString(attri[0]) : "",
						colinx, 2, 
						isDataCell ? CellStateEnum.HIGHLIGHTE : CellStateEnum.NORMAL,
						isDataCell ? NumStateEnum.TEXT_WHITE : NumStateEnum.TEXT_GREY);
				++colinx;
			}
			
			// draw line graph.
			int pre_pos = lot.getPrevious() != null ? getRedAttributeData(lot.getPrevious().getSource())[1] : 0;
			int cur_pos = attri[1];
			int next_pos = lot.getNext() != null ? getRedAttributeData(lot.getNext().getSource())[1] : 0;
			
			refreshLineGraph(item.line_graph, lot.getPrevious() != null, lot.getNext() != null, false, (pre_pos - cur_pos) * 2, cur_pos * 2 + 24, (next_pos - cur_pos) * 2);
		}
		
		private int[] getRedAttributeData(Lottery lot)
		{
			int[] data = new int[] {0,0,0};
			
			int[] remainBy3 = new int[] { 0,0,0 };
			lot.getScheme().GetRemainBy3(remainBy3);
			
			int[] smb = new int[] { 0,0,0 };
			lot.getScheme().GetSmallMiddleBig(smb);
			
			switch (options.viewOption)
			{
			case RedDiv1Detail:
			{
				data[0] = smb[0];
				data[1] = smb[0];
				data[2] = 7;
				break;
			}
			case RedDiv2Detail:
			{
				data[0] = smb[1];
				data[1] = smb[1];
				data[2] = 7;
				break;
			}
			case RedDiv3Detail:
			{
				data[0] = smb[2];
				data[1] = smb[2];
				data[2] = 7;
				break;
			}
			case RedRemain0Detail:
			{
				data[0] = remainBy3[0];
				data[1] = remainBy3[0];
				data[2] = 7;
				break;
			}
			case RedRemain1Detail:
			{
				data[0] = remainBy3[1];
				data[1] = remainBy3[1];
				data[2] = 7;
				break;
			}
			case RedRemain2Detail:
			{
				data[0] = remainBy3[2];
				data[1] = remainBy3[2];
				data[2] = 7;
				break;
			}
			case RedEvenOddDetail:
			{
				data[0] = 6 - lot.getScheme().getEvenNumCount();
				data[1] = data[0];
				data[2] = 7;
				break;
			}
			case RedBigSmallDetail:
			{
				data[0] = 6 - lot.getScheme().getSmallNumCount();
				data[1] = data[0];
				data[2] = 7;
				break;
			}	
			case RedPrimaryCompositeDetail:
			{		
				data[0] = lot.getScheme().getPrimeNumCount();
				data[1] = data[0];
				data[2] = 7;
				break;
			}
			case RedSumDetail:
			{				
				data[0] = lot.getScheme().getSum();
				
				int temp = Math.max(0, data[0] - 70);
				data[1] = Math.min(8, temp == 0 ? 0 : (temp - 1) / 10 + 1);
				data[2] = 9;
				break;
			}
			case RedContinuityDetail:
			{			
				data[0] = lot.getScheme().getContinuity();
				data[1] = data[0];
				data[2] = 6;
				break;
			}
			default:
				break;
			}
			
			return data;
		}
		
		private void buildRowForRedDevision(DiagramData lot, DiagramListItem item) throws ParseException
		{
			boolean[] dimOmissionStates = null;
			if (options.viewOption == ViewOption.RedObmissionBreak)
				dimOmissionStates = lot.getDivisionsOmissionBreak(options.subCategory);
			
			// add numbers row.
			for (int i = 1; i <= 33; ++ i)
			{	
				int divIndex = DiagramOptions.getDivisionIndex(options.subCategory, i);				
				boolean alt = divIndex % 2 == 0;
				boolean hitted = lot.getSource().getScheme().Contains(i);
				
				boolean dim = false;
				if (options.viewOption == ViewOption.RedObmissionBreak && !hitted)
				{
					dim = dimOmissionStates[divIndex];
				}
				
				boolean highlight = false;
				if (hitted)
				{
					if ((options.viewOption == ViewOption.RedHorizantalConnection && lot.isHorizontalConnected(i)) ||
						(options.viewOption == ViewOption.RedVerticalConnection && lot.isVerticalConnected(i)) ||
						(options.viewOption == ViewOption.RedObliqueConnection && lot.isObliqueConnected(i)) ||
						(options.viewOption == ViewOption.RedEvenConnection && lot.isEvenObliqueConnected(i)) ||
						(options.viewOption == ViewOption.RedOddConnection && lot.isOddObliqueConnected(i)))
					highlight = true;
				}
				
				String text = hitted ? 
						StringFormater.padLeft(Integer.toString(i), 2, '0') : 
						Integer.toString(lot.getSource().getStatus().getRedNumStates()[i - 1].getOmission());
				
				refreshCell(item.grid_cells.get(i + 2), 
						text, 
						i + 2, 
						1,
						dim ? CellStateEnum.DIM : (alt ? CellStateEnum.ALTERNATE1 : CellStateEnum.NORMAL), 
						hitted ? (highlight ? NumStateEnum.BALL_HIGHLIGHT : NumStateEnum.BALL_RED) : NumStateEnum.TEXT_GREY);
			}
		}	

		private void refreshCell(View view, String text, int col, int width, 
				CellStateEnum cell_state, NumStateEnum num_state)
		{
			// get the text view.
			TextView text_view = (TextView) view.findViewById(R.id.cell_text_view);
			
			// set text.
			text_view.setText(text); 
			switch (num_state)
			{
			case BALL_BLUE:
				text_view.setBackgroundResource(dr_num_blue);
				text_view.setTextColor(clr_white);
				break;
			case BALL_HIGHLIGHT:
				text_view.setBackgroundResource(dr_num_highlight);
				text_view.setTextColor(clr_white);
				break;
			case BALL_RED:
				text_view.setBackgroundResource(dr_num_red);
				text_view.setTextColor(clr_white);
				break;
			case TEXT_BLUE:
				text_view.setBackgroundColor(clr_transparent);
				text_view.setTextColor(clr_blue);
				break;
			case TEXT_RED:
				text_view.setBackgroundColor(clr_transparent);
				text_view.setTextColor(clr_red);
				break;
			case TEXT_WHITE:
				text_view.setBackgroundColor(clr_transparent);
				text_view.setTextColor(clr_white);
				break;
			default:
				text_view.setBackgroundColor(clr_transparent);
				text_view.setTextColor(clr_grey);
				break;
			}
        
			// adjust the size and position
	        GridLayout.LayoutParams realParams = (GridLayout.LayoutParams) view.getLayoutParams();
			realParams.height = unit_length;
			realParams.width = unit_length * width;
		
			view.setLayoutParams(realParams);
			
			// set view background.
			switch (cell_state)
			{
			case ALTERNATE1:
				view.setBackgroundResource(dr_cell_alt1);
				break;
			case ALTERNATE2:
				view.setBackgroundResource(dr_cell_alt2);
				break;
			case DIM:
				view.setBackgroundResource(dr_cell_dim);
				break;
			case HIGHLIGHTE:
				view.setBackgroundResource(dr_cell_highlight);
				break;
			default:
				view.setBackgroundResource(dr_cell_normal);
				break;		
			}
		}

		private void refreshLineGraph(LineGraphView line_graph, boolean showLine1, boolean showLine2, boolean isCircle,
				int preOffsetX, int curStartX, int nextOffsetX)
		{
			// check the line visibility.
			if (isCircle)
			{
				if (showLine1 && preOffsetX == 0)
					showLine1 = false;
				
				if (showLine2 && nextOffsetX == 0)
					showLine2 = false;
			}
			else
			{
				if (showLine1 && Math.abs(preOffsetX) < 4)
					showLine1 = false;
				
				if (showLine2 && Math.abs(nextOffsetX) < 4)
					showLine2 = false;
			}
			
			line_graph.setShowLine1(showLine1);
			line_graph.setShowLine2(showLine2);
			
			// get line data.
			if (isCircle)
			{
				if (showLine1)
				{
					float[] line_def = new float[]{
							((float)curStartX + (float)preOffsetX / 2 + 0.5f) * unit_length,
							0f,
							((float)curStartX + 0.5f)  * unit_length,
							(float)unit_length / 2
					};
					
					adjustLine(line_def, true, true);
					line_graph.setLine1Position(line_def[0], line_def[1], line_def[2], line_def[3]);
				}
				
				if (showLine2)
				{
					float[] line_def = new float[]{
							((float)curStartX + 0.5f)  * unit_length,
							(float)unit_length / 2,
							((float)curStartX + (float)nextOffsetX / 2 + 0.5f)  * unit_length,
							(float)unit_length
					};
					
					adjustLine(line_def, true, false);
					line_graph.setLine2Position(line_def[0], line_def[1], line_def[2], line_def[3]);
				}
			}
			else
			{
				if (showLine1)
				{
					float[] line_def = new float[]{
							((float)curStartX + (float)preOffsetX / 2 + 1f)  * unit_length,
							0f,
							((float)curStartX + 1f)  * unit_length,
							(float)unit_length / 2
					};
					
					adjustLine(line_def, false, true);
					line_graph.setLine1Position(line_def[0], line_def[1], line_def[2], line_def[3]);
				}
				
				if (showLine2)
				{
					float[] line_def = new float[]{
							((float)curStartX + 1f)  * unit_length,
							(float)unit_length / 2,
							((float)curStartX + (float)nextOffsetX / 2 + 1f)  * unit_length,
							(float)unit_length
					};
					
					adjustLine(line_def, false, false);
					line_graph.setLine2Position(line_def[0], line_def[1], line_def[2], line_def[3]);
				}
			}
		}
		
		private void adjustLine(float[] def, boolean bCircle, boolean isLine1)
		{
			if (bCircle)
			{
				double length = Math.sqrt(Math.pow(def[0] - def[2], 2) + Math.pow((double)unit_length / 2, 2));
				double offsetX = (double)unit_length * (def[2] - def[0]) / (length * 2);
				double offsetY = Math.pow((double)unit_length / 2, 2) / length;

				if (isLine1) 
				{
					def[2] -= offsetX;
					def[3] -= offsetY;
				} 
				else 
				{
					def[0] += offsetX;
					def[1] += offsetY;
				}
			}
			else
			{
				float tempFactor = (float)unit_length * (float)unit_length / 4f;		
	
				float dHDistance = def[0] - def[2];
				float offsetY = - tempFactor / Math.abs(dHDistance);
				float offsetX = dHDistance > 0 ? (float)unit_length : -(float)unit_length;
				
				if (isLine1) 
				{
					def[2] += offsetX;
					def[3] += offsetY;
				} 
				else 
				{
					def[0] -= offsetX;
					def[1] -= offsetY;
				}
			}
		}
	}
	
	private List<DiagramData> _diagramData = null;
	private DiagramListAdapter list_adapter;
	private ListView content_list;
	private int unit_length;
	
	public void setData(List<DiagramData> data)
	{
		_diagramData = data;
	}

	public void addToContainer(ListView list, DiagramOptions op)
	{
		// calculate the width of unit.
		Resources res = list.getResources();
		unit_length = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 25, res.getDisplayMetrics());
		
		// get the content list.
		content_list = list;
		
		// get list control.
		list_adapter = new DiagramListAdapter(op, unit_length);	
		content_list.setAdapter(list_adapter);			
	
		// the list width is just wrapped the header view not the actual item, need reset it.
		if (list_adapter.getCount() > 0)
		{			
	        View listItem = list_adapter.getView(0, null, content_list);
	        listItem.measure(0, 0); 
	        
		    ViewGroup.LayoutParams params = content_list.getLayoutParams();
		    params.width = listItem.getMeasuredWidth();
		    content_list.setLayoutParams(params);
		    
		    //ViewGroup.LayoutParams params1 = swipeRefresh_container.getLayoutParams();
		    //params1.width = listItem.getMeasuredWidth();
		    //swipeRefresh_container.setLayoutParams(params1);
	    }
	}
	
	public void ScrollToListBottom()
	{
		content_list.post(new Runnable() {
	        @Override
	        public void run() {
	            // Select the last row so it will scroll into view...
	        	content_list.setSelection(list_adapter.getCount() - 1);
	        }
	    });
	}
	
	public void refreshList()
	{		
		list_adapter.notifyDataSetChanged();
		
		// the list width is just wrapped the header view not the actual item, need reset it.
		if (list_adapter.getCount() > 0)
		{			
	        View listItem = list_adapter.getView(0, null, content_list);
	        listItem.measure(0, 0); 
	        
		    ViewGroup.LayoutParams params = content_list.getLayoutParams();
		    params.width = listItem.getMeasuredWidth();
		    content_list.setLayoutParams(params);
		    
		    //ViewGroup.LayoutParams params1 = container.getLayoutParams();
		    //params1.width = listItem.getMeasuredWidth();
		    //container.setLayoutParams(params1);
	    }
	}
}
