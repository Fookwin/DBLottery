package com.fookwin.lotteryspirit.view;

import com.fookwin.lotteryspirit.R;

import android.content.Context;
import android.os.Handler;
import android.util.AttributeSet;
import android.view.View;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.LinearLayout;
import android.widget.RadioButton;

public class SchemeOutputFormatSetting extends LinearLayout implements OnCheckedChangeListener 
{

	private Handler onValueChangedHandler;
	private RadioButton RB_Red_Comma;
	private RadioButton RB_Red_Dot;
	private RadioButton RB_Red_Strigula;
	private RadioButton RB_Red_Colon;
	private RadioButton RB_Red_Plus;
	private RadioButton RB_Blue_Comma;
	private RadioButton RB_Blue_Dot;
	private RadioButton RB_Blue_Strigula;
	private RadioButton RB_Blue_Colon;
	private RadioButton RB_Blue_Plus;
	private RadioButton RB_Red_Space;
	private RadioButton RB_Blue_Space;

	public SchemeOutputFormatSetting(Context context)
	{
		super(context);
		
		initCtrls(context);		
	}
	
	public SchemeOutputFormatSetting(Context context, AttributeSet attrs) 
	{
		super(context, attrs);
		
		initCtrls(context);
	}
	
	public SchemeOutputFormatSetting(Context context, AttributeSet attrs, int defStyle) 
	{
		super(context, attrs, defStyle);
		
		initCtrls(context);
	}
	
	private void initCtrls(Context context)
	{
		View view = LinearLayout.inflate(context, R.layout.view_format_setting, this);
		
		RB_Red_Comma = (RadioButton) view.findViewById(R.id.RB_Red_Comma);
		RB_Red_Comma.setOnCheckedChangeListener(this);
		RB_Red_Dot = (RadioButton) view.findViewById(R.id.RB_Red_Dot);
		RB_Red_Dot.setOnCheckedChangeListener(this);
		RB_Red_Strigula = (RadioButton) view.findViewById(R.id.RB_Red_Strigula);
		RB_Red_Strigula.setOnCheckedChangeListener(this);
		RB_Red_Colon = (RadioButton) view.findViewById(R.id.RB_Red_Colon);
		RB_Red_Colon.setOnCheckedChangeListener(this);
		RB_Red_Plus = (RadioButton) view.findViewById(R.id.RB_Red_Plus);
		RB_Red_Plus.setOnCheckedChangeListener(this);
		RB_Blue_Comma = (RadioButton) view.findViewById(R.id.RB_Blue_Comma);
		RB_Blue_Comma.setOnCheckedChangeListener(this);
		RB_Blue_Dot = (RadioButton) view.findViewById(R.id.RB_Blue_Dot);
		RB_Blue_Dot.setOnCheckedChangeListener(this);
		RB_Blue_Strigula = (RadioButton) view.findViewById(R.id.RB_Blue_Strigula);
		RB_Blue_Strigula.setOnCheckedChangeListener(this);
		RB_Blue_Colon = (RadioButton) view.findViewById(R.id.RB_Blue_Colon);
		RB_Blue_Colon.setOnCheckedChangeListener(this);
		RB_Blue_Plus = (RadioButton) view.findViewById(R.id.RB_Blue_Plus);
		RB_Blue_Plus.setOnCheckedChangeListener(this);
		RB_Red_Space = (RadioButton) view.findViewById(R.id.RB_Red_Space);
		RB_Red_Space.setOnCheckedChangeListener(this);
		RB_Blue_Space = (RadioButton) view.findViewById(R.id.RB_Blue_Space);
		RB_Blue_Space.setOnCheckedChangeListener(this);
	}
	
	public void setOnValueChangedHandler(Handler handler)
	{
		onValueChangedHandler = handler;
	}
	
	public String getOutputFormatFromUI()
    {
		String red_sep = " ", blue_sep = " ";

        if (RB_Red_Comma.isChecked())
            red_sep = ",";
        else if (RB_Red_Dot.isChecked())
            red_sep = ".";
        else if (RB_Red_Strigula.isChecked())
            red_sep = "-";
        else if (RB_Red_Colon.isChecked())
            red_sep = ":";
        else if (RB_Red_Plus.isChecked())
            red_sep = "+";

        if (RB_Blue_Comma.isChecked())
            blue_sep = ",";
        else if (RB_Blue_Dot.isChecked())
            blue_sep = ".";
        else if (RB_Blue_Strigula.isChecked())
            blue_sep = "-";
        else if (RB_Blue_Colon.isChecked())
            blue_sep = ":";
        else if (RB_Blue_Plus.isChecked())
            blue_sep = "+";

        return red_sep + blue_sep;
    }

	public void setOutputFormatUI(String format)
    {
    	String red_sep = " ", blue_sep = " ";

        char[] chars = format.toCharArray();
        if (chars.length == 2)
        {
            red_sep = String.valueOf(chars[0]);
            blue_sep = String.valueOf(chars[1]);
        }

        if (red_sep == ",")
            RB_Red_Comma.setChecked(true);
        else if (red_sep == ".")
            RB_Red_Dot.setChecked(true);
        else if (red_sep == "-")
            RB_Red_Strigula.setChecked(true);
        else if (red_sep == ":")
            RB_Red_Colon.setChecked(true);
        else if (red_sep == "+")
            RB_Red_Plus.setChecked(true);
        else
            RB_Red_Space.setChecked(true);

        if (blue_sep == ",")
            RB_Blue_Comma.setChecked(true);
        else if (blue_sep == ".")
            RB_Blue_Dot.setChecked(true);
        else if (blue_sep == "-")
            RB_Blue_Strigula.setChecked(true);
        else if (blue_sep == ":")
            RB_Blue_Colon.setChecked(true);
        else if (blue_sep == "+")
            RB_Blue_Plus.setChecked(true);
        else
            RB_Blue_Space.setChecked(true);
    }

	@Override
	public void onCheckedChanged(CompoundButton arg0, boolean arg1)
	{
		if (onValueChangedHandler != null)
			onValueChangedHandler.sendEmptyMessage(0);		
	}
}
