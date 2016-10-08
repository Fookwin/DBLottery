package com.fookwin.lotteryspirit.view;

import java.util.Date;
import java.util.Timer;
import java.util.TimerTask;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.util.StringFormater;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.LBDataManager;

import android.annotation.SuppressLint;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import android.widget.TextView;

public class ReleaseTimeDownView
{
	private View headerView;
	private TextView next_issue;
	private TextView next_issue_state;
	private TextView next_issue_timespan;
	
	private Timer _timer = null;
    private long _totalSecond;
    private final long _cutoffSecond = 7200;
    
    @SuppressLint("HandlerLeak")
	public Handler mHandler = new Handler() {
        public void handleMessage(Message msg)
        {
			if (timeCountDown())
			{
				int seconds = (int)(_totalSecond % 60L);
				int minutes =  (int)((_totalSecond / 60L) % 60L);
				int hours = (int)((_totalSecond / 3600L) % 24L);
				int days = (int)_totalSecond / 86400;
				
				String display = "";
				display += StringFormater.padLeft(Integer.toString(days), 2, '0') + "天 ";
				display += StringFormater.padLeft(Integer.toString(hours), 2, '0') + "时";
				display += StringFormater.padLeft(Integer.toString(minutes), 2, '0') + "分 ";
				display += StringFormater.padLeft(Integer.toString(seconds), 2, '0') + "秒 ";
				
                next_issue_timespan.setText(display);

                next_issue_state.setText(_totalSecond > _cutoffSecond ? "正在热销中" : "销售已截止");
            }
            else
            {
                _timer.cancel();
                next_issue_state.setText("开奖统计中");
            }
        }
    };
	
	public void AddToContainer(LinearLayout container)
	{
		if (headerView == null)
		{
			// create the view tab and add to container.
			headerView = LinearLayout.inflate(LotterySpirit.getInstance(), R.layout.view_release_timedown,null);
			
			// get the sub views.
			next_issue = (TextView) headerView.findViewById(R.id.next_issue_name);
			next_issue_state =(TextView) headerView.findViewById(R.id.next_issue_state);
			next_issue_timespan = (TextView) headerView.findViewById(R.id.next_issue_timespan);
			
			LayoutParams params = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
			container.addView(headerView, params);
		}
	}
	
	public void StartTimer()
	{		
		if (LBDataManager.GetInstance().getReleaseInfo() == null)
            return;

        // Get the release date.
        Date releaseAt = LBDataManager.GetInstance().getReleaseInfo().getNextReleaseTime();
        Date currentDate = new Date();
        _totalSecond = (releaseAt.getTime() - currentDate.getTime()) / 1000L;

        if (_totalSecond > 0)
        {
    		if (_timer == null)
    		{
    			_timer = new Timer();
    		}  

    		_timer.schedule(new TimerTask()
    		{
    			@Override
    			public void run() 
    			{
    				mHandler.sendEmptyMessage(0);
    			}
    		
    		}, 0, 1000);
        }
        else
        {
        	_totalSecond = 0;

            String display = "00天 00时 00分 00秒";
            next_issue_timespan.setText(display);
            next_issue_state.setText("开奖统计中");
        }

        String nextRelease = Integer.toString(LBDataManager.GetInstance().getReleaseInfo().getNextIssue());
        next_issue.setText(nextRelease);
	}
	
    private boolean timeCountDown()
    {
        if (_totalSecond <= 0)
            return false;
        else
        {
        	--_totalSecond;
            return true;
        }
    }
}
