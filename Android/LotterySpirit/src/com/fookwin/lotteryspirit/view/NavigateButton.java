package com.fookwin.lotteryspirit.view;

import com.fookwin.lotteryspirit.R;

import android.content.Context;
import android.util.AttributeSet;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

public class NavigateButton extends LinearLayout{

	private ImageView button_image_view;
	private TextView button_text_view;
	private int selected_color;
	private int unselected_color;
	private int selected_image;
	private int unselected_image;
	private View button_view;

	public NavigateButton(Context context) 
	{
		super(context);		
		initCtrls(context);
	}
	
	public NavigateButton(Context context, AttributeSet attrs) 
	{
		super(context, attrs);		
		initCtrls(context);
	}
	
	public NavigateButton(Context context, AttributeSet attrs, int defStyle) 
	{
		super(context, attrs, defStyle);		
		initCtrls(context);
	}

	@Override
	public void setSelected(boolean selected)
	{
		super.setSelected(selected);
		
		if (selected)
		{
			button_image_view.setImageResource(selected_image);
			button_text_view.setTextColor(selected_color);
		}
		else
		{
			button_image_view.setImageResource(unselected_image);
			button_text_view.setTextColor(unselected_color);
		}
	}

	private void initCtrls(Context context)
	{
		button_view = LinearLayout.inflate(context, R.layout.button_navigation, this);		
		button_image_view = (ImageView) button_view.findViewById(R.id.button_image);
		button_text_view = (TextView) button_view.findViewById(R.id.button_text);
	}
	
	public void setImage(int sel_img, int unsel_img)
	{
		selected_image = sel_img;
		unselected_image = unsel_img;
		
		if (button_view.isSelected())
		{
			button_image_view.setImageResource(selected_image);
		}
		else
		{
			button_image_view.setImageResource(unselected_image);
		}
	}
	
	public void setTextColor(int sel_clr, int unsel_clr)
	{
		selected_color = sel_clr;
		unselected_color = unsel_clr;
		
		if (button_view.isSelected())
		{
			button_text_view.setTextColor(selected_color);
		}
		else
		{
			button_text_view.setTextColor(unselected_color);
		}
	}
	
	public void setText(String text)
	{
		button_text_view.setText(text);
	}
}
