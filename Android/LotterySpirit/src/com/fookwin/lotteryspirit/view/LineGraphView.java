package com.fookwin.lotteryspirit.view;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.util.AttributeSet;
import android.view.View;

public class LineGraphView extends View
{
	Paint paint = new Paint();
	boolean showLine1 = false, showLine2 = false;
	float line1_startX, line1_startY, line1_stopX, line1_stopY;
	float line2_startX, line2_startY, line2_stopX, line2_stopY;
	
	public LineGraphView(Context context, AttributeSet attrs, int defStyleAttr)
	{
		super(context, attrs, defStyleAttr);
	}

	public LineGraphView(Context context, AttributeSet attrs) 
	{
		super(context, attrs);
	}

	public LineGraphView(Context context)
	{		
		super(context);
	}

	@Override
	public void onDraw(Canvas canvas)
	{
		if (showLine1)
			canvas.drawLine(line1_startX, line1_startY, line1_stopX, line1_stopY, paint);
		
		if (showLine2)
			canvas.drawLine(line2_startX, line2_startY, line2_stopX, line2_stopY, paint);
	}
	
	public void setLine1Position(float _startX, float _startY, float _stopX, float _stopY)
	{
		line1_startX = _startX;
		line1_startY = _startY;	
		line1_stopX = _stopX;
		line1_stopY = _stopY;
	}
	
	public void setLine2Position(float _startX, float _startY, float _stopX, float _stopY)
	{
		line2_startX = _startX;
		line2_startY = _startY;	
		line2_stopX = _stopX;
		line2_stopY = _stopY;
	}
	
	public void setLineColor(int color)
	{
		paint.setColor(color);
	}
	
	public void setLineWidth(int width)
	{
		paint.setStrokeWidth((float) width);
	}
	
	public void setShowLine1(boolean show)
	{
		showLine1 = show;
	}
	
	public void setShowLine2(boolean show)
	{
		showLine2 = show;
	}
}
