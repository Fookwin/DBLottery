package com.fookwin.lotteryspirit.view;

import com.fookwin.lotteryspirit.R;

import android.content.Context;
import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.AttributeSet;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;

public class NumberEditor extends LinearLayout implements OnClickListener 
{
	public static int MAXVALUE = Integer.MAX_VALUE;
	
	private View reduceButton;
	private View increaseButton;
	private TextView label;
	private EditText value;
	private int minValue = 0;
	private int maxValue = MAXVALUE;
	private Handler onValueChangedHandler;
	private Handler onNameClickedHandler;
	private TextView unit;

	private boolean _clickable;

	private View click_icon;

	public NumberEditor(Context context)
	{
		super(context);
		
		initCtrls(context);		
	}
	
	public NumberEditor(Context context, AttributeSet attrs) 
	{
		super(context, attrs);
		
		initCtrls(context);
	}
	
	public NumberEditor(Context context, AttributeSet attrs, int defStyle) 
	{
		super(context, attrs, defStyle);
		
		initCtrls(context);
	}
	
	public void setClickable(boolean enable)
	{
		_clickable = enable;
		
		click_icon.setVisibility(_clickable ? View.VISIBLE : View.GONE);
	}
	
	public boolean clickable()
	{
		return _clickable;
	}
	
	private void initCtrls(Context context)
	{
		View view = LinearLayout.inflate(context, R.layout.view_num_input, this);
		
		reduceButton = view.findViewById(R.id.reduce_button);
		reduceButton.setOnClickListener(this);
		increaseButton = view.findViewById(R.id.increase_button);
		increaseButton.setOnClickListener(this);

		unit = (TextView) view.findViewById(R.id.value_unit);
		label = (TextView) view.findViewById(R.id.value_name);
		label.setOnClickListener(this);
		
		value = (EditText) view.findViewById(R.id.value_text);
		value.addTextChangedListener(new TextWatcher()
		{
			@Override
			public void afterTextChanged(Editable s) 
			{
	        	int current = s.length() > 0 ? Integer.parseInt(s.toString()) : minValue - 1;
				if (current < minValue)
					current = minValue;
				else if (current > maxValue)
					current = maxValue;
				else
					return;
				
				s.replace(0, s.length(), Integer.toString(current));
			}

			@Override
			public void beforeTextChanged(CharSequence s, int start, int count,
					int after) 
			{
			}

			@Override
			public void onTextChanged(CharSequence s, int start, int before,
					int count) 
			{
				if (onValueChangedHandler != null)
				{
					onValueChangedHandler.sendEmptyMessage(getValue());
				}
			}			
		});
		value.setOnClickListener(this);
		
		click_icon = view.findViewById(R.id.click_icon);
		click_icon.setVisibility(_clickable ? View.VISIBLE : View.GONE);
		click_icon.setOnClickListener(this);
	}
	
	public void setOnValueChangedHandler(Handler handler)
	{
		onValueChangedHandler = handler;
	}
	
	public void setTitle(String title)
	{
		label.setText(title);
	}
	
	public void setUnit(String _unit)
	{
		unit.setText(_unit);
	}
	
	public void setValue(int num)
	{
		if (num >= minValue && num <= maxValue)
		{
			value.setText(Integer.toString(num));
		}
	}
	
	public void setRegion(int min, int max)
	{
		minValue = min;
		maxValue = max;
	}
	
	public void setOnValueNameClickedHandler(Handler _handler)
	{
		onNameClickedHandler = _handler;
	}
	
	public int getValue()
	{
		try
		{
			return Integer.parseInt(value.getText().toString());
		}
		catch (NumberFormatException nfe)
		{
		}
		
		return minValue - 1; // return an invalid value.	
	}

	@Override
	public void onClick(View v) 
	{
		int current = getValue();
		if (v.getId() == R.id.reduce_button)
		{
			if (current > minValue)
			{
				setValue(current - 1);
			}
		}
		else if (v.getId() == R.id.increase_button)
		{
			if (current < maxValue)
			{
				setValue(current + 1);
			}
		}	
		else if (v.getId() == R.id.click_icon || v.getId() == R.id.value_name)
		{
			if (_clickable && onNameClickedHandler != null)
				onNameClickedHandler.sendEmptyMessage(0);
		}
	}
}
