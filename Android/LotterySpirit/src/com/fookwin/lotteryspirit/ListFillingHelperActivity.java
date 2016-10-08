package com.fookwin.lotteryspirit;

import java.util.ArrayList;

import com.fookwin.lotterydata.data.DantuoSchemeSelector;
import com.fookwin.lotterydata.data.RandomSchemeSelector;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotterydata.data.StandardSchemeSelector;
import com.fookwin.lotterydata.data.StandardSchemeSelector.RedBlueConnectionTypeEnum;
import com.fookwin.lotteryspirit.util.BitmapUtil;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.support.v7.widget.GridLayout;
import android.support.v7.widget.GridLayout.Spec;
import android.util.DisplayMetrics;
import android.view.Gravity;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.view.WindowManager;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.TextView;

public class ListFillingHelperActivity extends Activity {

	private static Object source = null;
	
	public static void setSource(Object _s)
	{
		source = _s;
	}
	private GridLayout grid_view;
	private ImageView form_image;
	private int bitmapWidth;
	private int bitmapHeight;
	
	private int rowCount = 37;
	private int colCount = 16;	
	private View[][] cells = new View[rowCount][colCount];
	
	private ArrayList<Scheme> listForPaging = null;
	private int pageCount = 1;
	private int pageIndex = 1;
	private int cellWidth;
	private int cellHeith;
	private TextView status_text;
	private View to_pre_btn;
	private View to_next_btn;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
        // Display welcome screen in full screen.
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, 
				WindowManager.LayoutParams.FLAG_FULLSCREEN);	
		
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_list_filling_helper);
		
        // Get the screen size.
        DisplayMetrics dm = new DisplayMetrics();  
		getWindowManager().getDefaultDisplay().getMetrics(dm); 		
		int screenHeight = dm.heightPixels;
		int screenWidth = dm.widthPixels;
		
		cellWidth = (int)((double)screenWidth / colCount);
		cellHeith = (int)((double)screenHeight / rowCount);
		
		bitmapWidth = cellWidth * colCount;
		bitmapHeight = cellHeith * rowCount;
		
		// create a view and add to grid.
		form_image = (ImageView) findViewById(R.id.form_image);	
		Bitmap srcpic = BitmapFactory.decodeResource(getResources(),R.drawable.image_form);	
		Bitmap newpic = BitmapUtil.GetNewBitmap(srcpic, screenWidth, screenHeight,bitmapWidth, bitmapHeight);
		form_image.setImageBitmap(newpic);
		
		// create a view and add to grid.
		grid_view = (GridLayout) findViewById(R.id.fill_grid);
		
		initGridCells();
		
		fillGrid();
		
		// init status text
		Animation fixedAnim; 
		fixedAnim = AnimationUtils.loadAnimation(this, R.anim.rotation_right);  
		fixedAnim.setFillAfter(true); 
		
		status_text = (TextView) findViewById(R.id.status_text);
		status_text.setAnimation(fixedAnim);

		// init buttons
		to_pre_btn = findViewById(R.id.to_pre_btn);
		to_pre_btn.setOnClickListener(new OnClickListener(){

			@Override
			public void onClick(View v) {
				if (pageIndex > 1)
				{
					pageIndex --;
					fillForPage(pageIndex);
					
					if (pageIndex == 1)
						to_pre_btn.setVisibility(View.GONE);
					
					to_next_btn.setVisibility(View.VISIBLE);
					
					updateStatusText();
				}
			}
		});
		to_pre_btn.setVisibility(View.GONE); // always invisible for first page.
		
		to_next_btn = findViewById(R.id.to_next_btn);
		to_next_btn.setOnClickListener(new OnClickListener(){

			@Override
			public void onClick(View v) {
				if (pageIndex < pageCount)
				{
					pageIndex ++;
					fillForPage(pageIndex);
					
					if (pageIndex == pageCount)
						to_next_btn.setVisibility(View.GONE);
					
					to_pre_btn.setVisibility(View.VISIBLE);
					
					updateStatusText();
				}
			}
		});
		
		
		if (listForPaging == null || pageCount == 1)
		{
			to_next_btn.setVisibility(View.GONE);
		}
		
		updateStatusText();
	}
	
	private void updateStatusText()
	{
		String text = String.valueOf(pageIndex) + " / " + String.valueOf(pageCount);
		status_text.setText(text);
	}
	
	private void initGridCells()
	{		
		for (int row = 0; row < rowCount; ++ row)
		{
			for (int col = 0; col < colCount; ++ col)
			{
				// create a view and add to grid.
				View view = this.getLayoutInflater().inflate(R.layout.view_form_cell, null);
				
				Spec rowspec = GridLayout.spec(row, 1); 
		        Spec colspec = GridLayout.spec(col, 1);
		        GridLayout.LayoutParams params = new GridLayout.LayoutParams(rowspec, colspec);
		        params.height = cellHeith;
		        params.width = cellWidth;
		        grid_view.addView(view, params);
		        
		        cells[row][col] = view;
		        view.setVisibility(View.INVISIBLE); // make it invisible default.
			}
		}
		
		FrameLayout.LayoutParams  layoutParam = (android.widget.FrameLayout.LayoutParams) grid_view.getLayoutParams();
		layoutParam.gravity = Gravity.CENTER;
		layoutParam.width = bitmapWidth;
		layoutParam.height = bitmapHeight;
		grid_view.setLayoutParams(layoutParam);
	}
	
	private void fillGrid()
	{
		if (source instanceof StandardSchemeSelector)
		{
			StandardSchemeSelector standardSel = (StandardSchemeSelector) source;
			
			fillStandardSelector(standardSel);
			return;
		}
		
		
		if (source instanceof RandomSchemeSelector)
		{
			RandomSchemeSelector randomSel = (RandomSchemeSelector) source;
			fillSchemeList(randomSel.GetResult());
			return;
		}
		
		if (source instanceof DantuoSchemeSelector)
		{
			DantuoSchemeSelector dantuoSel = (DantuoSchemeSelector) source;
			fillDantuoSelector(dantuoSel);
			return;
		}
		
		if (source instanceof ArrayList<?>)
		{
			@SuppressWarnings("unchecked")
			ArrayList<Scheme> schemeList = (ArrayList<Scheme>) source;
			fillSchemeList(schemeList);
			return;
		}
	}
	
	private void fillStandardSelector(StandardSchemeSelector sel)
	{
		if (sel.getApplyMatrixFilter() || 
			sel.getBlueConnectionType() != RedBlueConnectionTypeEnum.Duplicate)
		{
			// fill with single scheme.
			fillSchemeList(sel.GetResult());
			return;
		}
		
		// fushi
		int[] reds = sel.getSelectedReds().getNumbers();
		int[] blues = sel.getSelectedBlues().getNumbers();
		
		// fill reds
		for (int i = 0; i < reds.length; ++ i)
		{
			int row = getRedRow(reds[i], 0);
			int col = getRedCol(reds[i]);
			
			cells[row][col].setVisibility(View.VISIBLE);
		}
		
		// fill blues
		for (int i = 0; i < blues.length; ++ i)
		{
			int row = getBlueRow(blues[i], 0);
			int col = getBlueCol(blues[i]);
			
			cells[row][col].setVisibility(View.VISIBLE);
		}
		
		// mark options for fushi
		if (reds.length > 6)
		{
			int row = 32;
			int col = colCount - reds.length + 1;
			cells[row][col].setVisibility(View.VISIBLE);
		}
		
		if (blues.length > 1)
		{
			int row = blues.length > 9 ? 35 : 34;
			int col = blues.length > 9 ? colCount - blues.length + 4 : colCount - blues.length - 4;
			cells[row][col].setVisibility(View.VISIBLE);
		}
	}
	
	private void fillDantuoSelector(DantuoSchemeSelector sel)
	{
		int[] dans = sel.getSelectedDans().getNumbers();
		int[] tuos = sel.getSelectedTuos().getNumbers();
		int[] blues = sel.getSelectedBlues().getNumbers();
		
		// fill dan reds
		for (int i = 0; i < dans.length; ++ i)
		{
			int row = getRedRow(dans[i], 0);
			int col = getRedCol(dans[i]);
			
			cells[row][col].setVisibility(View.VISIBLE);
		}
		
		// fill tuo reds
		for (int i = 0; i < tuos.length; ++ i)
		{
			int row = getRedRow(tuos[i], 1);
			int col = getRedCol(tuos[i]);
			
			cells[row][col].setVisibility(View.VISIBLE);
		}
		
		// fill blues
		for (int i = 0; i < blues.length; ++ i)
		{
			int row = getBlueRow(blues[i], 0);
			int col = getBlueCol(blues[i]);
			
			cells[row][col].setVisibility(View.VISIBLE);
		}
		
		// mark options for dantuo
		cells[5][7].setVisibility(View.VISIBLE);
		cells[11][7].setVisibility(View.VISIBLE);
	}
	
	private void fillSchemeList(ArrayList<Scheme> list)
	{
		// prepare the data for paging.
		listForPaging = list;
		pageCount = (list.size() - 1) / 5 + 1;
		pageIndex = 1;
		
		// go to first page.
		fillForPage(pageIndex);
	}
	
	private void fillForPage(int pageIndex)
	{
		if (listForPaging == null || pageIndex > pageCount)
			return;
		
		// clear previous.
		{
			for (int row = 0; row < rowCount; ++ row)
				for (int col = 0; col < colCount; ++ col)
					cells[row][col].setVisibility(View.INVISIBLE);
		}
		
		int block = 0;
		for (int index = (pageIndex - 1) * 5; block < 5 && index < listForPaging.size(); ++ index, ++ block)
		{
			Scheme item = listForPaging.get(index);
			
			int[] reds = item.GetRedNums();
			
			// fill reds
			for (int i = 0; i < reds.length; ++ i)
			{
				int row = getRedRow(reds[i], block);
				int col = getRedCol(reds[i]);
				
				cells[row][col].setVisibility(View.VISIBLE);
			}
			
			// fill blue
			int row = getBlueRow(item.getBlue(), block);
			int col = getBlueCol(item.getBlue());
			
			cells[row][col].setVisibility(View.VISIBLE);
		}
	}
	
	private int getRedCol(int red)
	{
		int col = colCount - ((red - 1) % 7) - 2; // add left offset.
		
		return col;
	}
	
	private int getRedRow(int red, int block)
	{
		int row = (red - 1) / 7;
		row += block * 6; // block offset
		row += 1; // add left offset;
		
		return row;
	}
	
	private int getBlueCol(int blue)
	{
		int col = colCount - ((blue - 1) % 4) - 11; // add top offset.
		
		return col;
	}
	
	private int getBlueRow(int blue, int block)
	{
		int row = (blue - 1) / 4;
		row += block * 6; // block offset
		row += 1; // add left offset;
		
		return row;
	}
}
