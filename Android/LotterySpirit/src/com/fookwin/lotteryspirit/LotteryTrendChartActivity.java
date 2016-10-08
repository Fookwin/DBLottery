package com.fookwin.lotteryspirit;

import java.text.ParseException;
import java.util.ArrayList;

import com.fookwin.lotterydata.data.Set;
import com.fookwin.lotteryspirit.data.DiagramData;
import com.fookwin.lotteryspirit.data.DiagramOptions;
import com.fookwin.lotteryspirit.data.HelpCenter;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.data.DiagramOptions.Category;
import com.fookwin.lotteryspirit.data.DiagramOptions.SubCategory;
import com.fookwin.lotteryspirit.data.DiagramOptions.ViewOption;
import com.fookwin.lotteryspirit.view.DiagramHeaderView;
import com.fookwin.lotteryspirit.view.DiagramListView;
import com.fookwin.lotteryspirit.view.NumberSelectorView;
import com.fookwin.lotteryspirit.view.NumberSelectorView.NumInfoEnum;
import com.fookwin.lotteryspirit.view.NumberSelectorView.SelectModeEnum;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.app.ActionBar.OnNavigationListener;
import android.app.ProgressDialog;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.DisplayMetrics;
import android.view.ContextThemeWrapper;
import android.view.Menu;
import android.view.MenuItem;
import android.view.SubMenu;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.SpinnerAdapter;

public class LotteryTrendChartActivity extends Activity
{
	private LinearLayout diagram_header;
	private ListView diagram_list;
	
	private DiagramOptions options = new DiagramOptions();
	
	private DiagramHeaderView header_view;
	private DiagramListView list_view;
	
	private SpinnerAdapter categories_adapter;
	private ContextThemeWrapper theme_context;
	
	private MenuItem sub_categories_red_position;
	private MenuItem sub_categories_red_division;
	private MenuItem display_option_red_division;
	private MenuItem display_option_red_genral;
	private ImageView helpIcon;
	private ImageView toBottom_Icon;
	
	private NumberSelectorView red_select_view;
	private NumberSelectorView blue_select_view;	
	private LinearLayout mark_red_view;
	private LinearLayout mark_blue_view;
	
	@SuppressLint("HandlerLeak")
	Handler numSelectionChangedhandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) 
		{
			super.handleMessage(msg);		

			// refresh the status.
		}
	};
	private LinearLayout mark_num_field;
	private ImageView mark_Icon;

	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_lottery_trend_chart);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);

		theme_context = new ContextThemeWrapper(this, android.R.style.Theme_Holo);
		
		categories_adapter = ArrayAdapter.createFromResource(theme_context,  
				R.array.diagram_categories, android.R.layout.simple_spinner_dropdown_item);		

		// update action bar
		updateActionBar();
	
		// get controls.
		diagram_header = (LinearLayout)findViewById(R.id.diagram_header);
		diagram_list = (ListView)findViewById(R.id.diagram_list);
		
		// mark up panels
		mark_red_view = (LinearLayout)findViewById(R.id.mark_red_view);
		mark_blue_view = (LinearLayout)findViewById(R.id.mark_blue_view);
		mark_num_field = (LinearLayout)findViewById(R.id.mark_num_field);
		
		// build the view.
		header_view = new DiagramHeaderView();
		header_view.setOptions(options);
		header_view.addToContainer(diagram_header);
		
        final ProgressDialog dialog = new ProgressDialog(this);
        dialog.setMessage("加载走势图数据...");
        dialog.setIndeterminate(true);
        dialog.setCancelable(false);
        dialog.show();
        
		// build the list.
		list_view = new DiagramListView();
		final ArrayList<DiagramData> data = new ArrayList<DiagramData>();
		final Handler handler = new Handler()
		{
			@Override
			public void handleMessage(Message msg) {
				super.handleMessage(msg);
				
				list_view.setData(data);
				list_view.addToContainer(diagram_list, options);
		        list_view.refreshList();
		        
		        dialog.dismiss();
			}
		};
		
		new Thread(new Runnable()
		{
			public void run() 
			{	
				try {
					DiagramData.generateDiagramData(data);
				} catch (ParseException e) {
					e.printStackTrace();
				}
				handler.sendEmptyMessage(-1);
			}
		}).start();
		
        header_view.refreshContentHeader();
        
		helpIcon = (ImageView)findViewById(R.id.helpIcon);
		helpIcon.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				HelpCenter.Instance().Show(31);
			}			
		});
		
		toBottom_Icon = (ImageView)findViewById(R.id.toBottom_Icon);
		toBottom_Icon.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				list_view.ScrollToListBottom();
			}			
		});
		
		mark_Icon = (ImageView)findViewById(R.id.mark_Icon);
		mark_Icon.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				if (mark_num_field.getVisibility() == View.GONE)
				{
					mark_num_field.setVisibility(View.VISIBLE);
					mark_Icon.setImageResource(R.drawable.icon_flag_hide);
				}
				else
				{
					mark_num_field.setVisibility(View.GONE);
					mark_Icon.setImageResource(R.drawable.icon_flag);					
				}
			}			
		});
		
		// Get the screen size.
        DisplayMetrics dm = new DisplayMetrics();  
		getWindowManager().getDefaultDisplay().getMetrics(dm); 
		int screenWidth = dm.widthPixels;
		
		Set markedRedsInclude = LBDataManager.GetInstance().getMarkedReds(true);
		Set markedBluesInclude = LBDataManager.GetInstance().getMarkedBlues(true);
		Set markedRedsExclude = LBDataManager.GetInstance().getMarkedReds(false);
		Set markedBluesExclude = LBDataManager.GetInstance().getMarkedBlues(false);
		
		red_select_view = new NumberSelectorView();
		red_select_view.setNumSet(markedRedsInclude, markedRedsExclude);
		red_select_view.setNumInfoMode(NumInfoEnum.NONE);
		red_select_view.addToContainer(mark_red_view, SelectModeEnum.RANDOM_RED, screenWidth - 30);
	
		blue_select_view = new NumberSelectorView();
		blue_select_view.setNumSet(markedBluesInclude, markedBluesExclude);
		blue_select_view.setNumInfoMode(NumInfoEnum.NONE);
		blue_select_view.addToContainer(mark_blue_view, SelectModeEnum.RANDOM_BLUE, screenWidth - 30);
	}
	
	public void updateActionBar()
	{
		ActionBar actionBar = getActionBar();  
		actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_LIST);
		actionBar.setTitle("");
		
		actionBar.setListNavigationCallbacks( categories_adapter, new OnNavigationListener()
		{
			@Override
			public boolean onNavigationItemSelected(int position, long itemId) 
			{
				Category old = options.category;
	            switch (position) 
	            {
	            case 0:
	            	options.category = Category.RedGeneral;
	            	options.subCategory = SubCategory.None;
	            	options.viewOption = ViewOption.RedSumDetail;
	            	break;
	            case 1:
	            	options.category = Category.RedDivision;
	            	options.subCategory = SubCategory.RedDivisionIn3;
	            	options.viewOption = ViewOption.None;
	            	break;
	            case 2:
	            	options.category = Category.RedPosition;
	            	options.subCategory = SubCategory.RedPosition1;
	            	options.viewOption = ViewOption.None;
	            	break;
	            case 3:
	            	options.category = Category.BlueGeneral;
	            	options.subCategory = SubCategory.None;
	            	options.viewOption = ViewOption.None;
	            	break;
	            case 4:
	            	options.category = Category.BlueSpan;
	            	options.subCategory = SubCategory.None;
	            	options.viewOption = ViewOption.None;
	            	break;
	            }	            
				
	            if (options.category != old)
	            {
	            	new Handler().postDelayed(new Runnable()
		            {
		                @Override
		                public void run() 
		                {	            		
		            		// update menu from option.
		            		updateMenuFromOption();
		            		
				            header_view.refreshContentHeader();
				            list_view.refreshList();
		                }
		            }, 0);
	            }

				return false;
			}		
		}); 
		
		// update the active category.
		updateActiveCategoryFromOption();
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) 
	{
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.diagram_options, menu);
		
		sub_categories_red_position = menu.findItem(R.id.sub_categories_red_position);
		sub_categories_red_division = menu.findItem(R.id.sub_categories_red_division);
		display_option_red_division = menu.findItem(R.id.display_options_red_division);
		display_option_red_genral = menu.findItem(R.id.display_options_red_general);
		
		updateMenuFromOption();
		
		return true;
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item)
	{
		SubCategory old_sub = options.subCategory;
		ViewOption old_display = options.viewOption;
		switch (item.getItemId())
		{
		case R.id.red_division_3:
			options.subCategory = SubCategory.RedDivisionIn3;
			break;
		case R.id.red_division_4:
			options.subCategory = SubCategory.RedDivisionIn4;
			break;
		case R.id.red_division_7:
			options.subCategory = SubCategory.RedDivisionIn7;
			break;
		case R.id.red_division_11:
			options.subCategory = SubCategory.RedDivisionIn11;
			break;
		case R.id.red_position_1:
			options.subCategory = SubCategory.RedPosition1;
			break;
		case R.id.red_position_2:
			options.subCategory = SubCategory.RedPosition2;
			break;
		case R.id.red_position_3:
			options.subCategory = SubCategory.RedPosition3;
			break;
		case R.id.red_position_4:
			options.subCategory = SubCategory.RedPosition4;
			break;
		case R.id.red_position_5:
			options.subCategory = SubCategory.RedPosition5;
			break;
		case R.id.red_position_6:
			options.subCategory = SubCategory.RedPosition6;
			break;
		case R.id.display_RedSumDetail:
			options.viewOption = ViewOption.RedSumDetail;
			break;
		case R.id.display_RedContinuityDetail:
			options.viewOption = ViewOption.RedContinuityDetail;
			break;
		case R.id.display_RedEvenOddDetail:
			options.viewOption = ViewOption.RedEvenOddDetail;
			break;
		case R.id.display_RedBigSmallDetail:
			options.viewOption = ViewOption.RedBigSmallDetail;
			break;
		case R.id.display_RedPrimaryCompositeDetail:
			options.viewOption = ViewOption.RedPrimaryCompositeDetail;
			break;
		case R.id.display_RedRemain0Detail:
			options.viewOption = ViewOption.RedRemain0Detail;
			break;
		case R.id.display_RedRemain1Detail:
			options.viewOption = ViewOption.RedRemain1Detail;
			break;
		case R.id.display_RedRemain2Detail:
			options.viewOption = ViewOption.RedRemain2Detail;
			break;
		case R.id.display_RedDiv1Detail:
			options.viewOption = ViewOption.RedDiv1Detail;
			break;
		case R.id.display_RedDiv2Detail:
			options.viewOption = ViewOption.RedDiv2Detail;
			break;
		case R.id.display_RedDiv3Detail:
			options.viewOption = ViewOption.RedDiv3Detail;
			break;
		case R.id.display_nothing:
			options.viewOption = ViewOption.None;
			break;
		case R.id.display_RedHorizantalConnection:
			options.viewOption = ViewOption.RedHorizantalConnection;
			break;
		case R.id.display_RedVerticalConnection:
			options.viewOption = ViewOption.RedVerticalConnection;
			break;
		case R.id.display_RedObliqueConnection:
			options.viewOption = ViewOption.RedObliqueConnection;
			break;
		case R.id.display_RedOddConnection:
			options.viewOption = ViewOption.RedOddConnection;
			break;
		case R.id.display_RedEvenConnection:
			options.viewOption = ViewOption.RedEvenConnection;
			break;
		case R.id.display_RedObmissionBreak:
			options.viewOption = ViewOption.RedObmissionBreak;
			break;
        case android.R.id.home:
            finish();
            return true;
		}
		
        if (old_sub != options.subCategory || old_display != options.viewOption)
        {
        	new Handler().postDelayed(new Runnable()
            {
                @Override
                public void run() 
                {         
            		// update menu from option.
            		updateMenuFromOption();
            		
		            header_view.refreshContentHeader();
		            list_view.refreshList();
                }
            }, 0);
        }

		return super.onOptionsItemSelected(item);
	}

	private void updateMenuFromOption()
	{
		// make all options invisible 
		sub_categories_red_position.setVisible(false);
		sub_categories_red_division.setVisible(false);
		display_option_red_division.setVisible(false);
		display_option_red_genral.setVisible(false);
		
		switch (options.category)
		{
		case RedDivision:
		{			
			CharSequence title_sub = "";
			SubMenu subMenu_sub = sub_categories_red_division.getSubMenu();
			switch (options.subCategory)
			{
			case RedDivisionIn11:
				title_sub = subMenu_sub.findItem(R.id.red_division_11).getTitle();
				break;
			case RedDivisionIn3:
				title_sub = subMenu_sub.findItem(R.id.red_division_3).getTitle();
				break;
			case RedDivisionIn4:
				title_sub = subMenu_sub.findItem(R.id.red_division_4).getTitle();
				break;
			case RedDivisionIn7:
				title_sub = subMenu_sub.findItem(R.id.red_division_7).getTitle();
				break;
			default:
				break;			
			}
			
			sub_categories_red_division.setTitle(title_sub);
			sub_categories_red_division.setVisible(true);
			
			CharSequence title_display = "";
			SubMenu subMenu_display = display_option_red_division.getSubMenu();
			switch (options.viewOption)
			{	
			case None:
				title_display = subMenu_display.findItem(R.id.display_nothing).getTitle();
				break;			
			case RedEvenConnection:
				title_display = subMenu_display.findItem(R.id.display_RedEvenConnection).getTitle();
				break;
			case RedHorizantalConnection:
				title_display = subMenu_display.findItem(R.id.display_RedHorizantalConnection).getTitle();
				break;
			case RedObliqueConnection:
				title_display = subMenu_display.findItem(R.id.display_RedObliqueConnection).getTitle();
				break;
			case RedObmissionBreak:
				title_display = subMenu_display.findItem(R.id.display_RedObmissionBreak).getTitle();
				break;
			case RedOddConnection:
				title_display = subMenu_display.findItem(R.id.display_RedOddConnection).getTitle();
				break;
			case RedVerticalConnection:
				title_display = subMenu_display.findItem(R.id.display_RedVerticalConnection).getTitle();
				break;
			default:
				break;			
			}
			
			display_option_red_division.setTitle(title_display);			
			display_option_red_division.setVisible(true);
			break;
		}
		case RedGeneral:
		{			
			CharSequence title_display = "";
			SubMenu subMenu_display = display_option_red_genral.getSubMenu();
			switch (options.viewOption)
			{			
			case RedBigSmallDetail:
				title_display = subMenu_display.findItem(R.id.display_RedBigSmallDetail).getTitle();
				break;
			case RedContinuityDetail:
				title_display = subMenu_display.findItem(R.id.display_RedContinuityDetail).getTitle();
				break;
			case RedDiv1Detail:
				title_display = subMenu_display.findItem(R.id.display_RedDiv1Detail).getTitle();
				break;
			case RedDiv2Detail:
				title_display = subMenu_display.findItem(R.id.display_RedDiv2Detail).getTitle();
				break;
			case RedDiv3Detail:
				title_display = subMenu_display.findItem(R.id.display_RedDiv3Detail).getTitle();
				break;
			case RedEvenOddDetail:
				title_display = subMenu_display.findItem(R.id.display_RedEvenOddDetail).getTitle();
				break;
			case RedPrimaryCompositeDetail:
				title_display = subMenu_display.findItem(R.id.display_RedPrimaryCompositeDetail).getTitle();
				break;
			case RedRemain0Detail:
				title_display = subMenu_display.findItem(R.id.display_RedRemain0Detail).getTitle();
				break;
			case RedRemain1Detail:
				title_display = subMenu_display.findItem(R.id.display_RedRemain1Detail).getTitle();
				break;
			case RedRemain2Detail:
				title_display = subMenu_display.findItem(R.id.display_RedRemain2Detail).getTitle();
				break;
			case RedSumDetail:
				title_display = subMenu_display.findItem(R.id.display_RedSumDetail).getTitle();
				break;
			default:
				break;		
			}
			
			display_option_red_genral.setTitle(title_display);			
			display_option_red_genral.setVisible(true);
			break;
		}
		case RedPosition:
		{			
			CharSequence title_sub = "";
			SubMenu subMenu_sub = sub_categories_red_position.getSubMenu();
			switch (options.subCategory)
			{
			case RedPosition1:
				title_sub = subMenu_sub.findItem(R.id.red_position_1).getTitle();
				break;
			case RedPosition2:
				title_sub = subMenu_sub.findItem(R.id.red_position_2).getTitle();
				break;
			case RedPosition3:
				title_sub = subMenu_sub.findItem(R.id.red_position_3).getTitle();
				break;
			case RedPosition4:
				title_sub = subMenu_sub.findItem(R.id.red_position_4).getTitle();
				break;
			case RedPosition5:
				title_sub = subMenu_sub.findItem(R.id.red_position_5).getTitle();
				break;
			case RedPosition6:
				title_sub = subMenu_sub.findItem(R.id.red_position_6).getTitle();
				break;
			default:
				break;			
			}
			
			sub_categories_red_position.setTitle(title_sub);
			sub_categories_red_position.setVisible(true);
			break;
		}
		default:
			break;		
		}
	}
	
	private void updateActiveCategoryFromOption()
	{
		// update navigation list.
		ActionBar actionBar = getActionBar();

		int category = 0;
		switch (options.category) {
		case BlueGeneral: {
			category = 3;
			break;
		}
		case BlueSpan: {
			category = 4;
			break;
		}
		case RedDivision: {
			category = 1;
			break;
		}
		case RedGeneral: {
			category = 0;
			break;
		}
		case RedPosition: {
			category = 2;
			break;
		}
		default:
			break;
		}

		if (actionBar.getSelectedNavigationIndex() != category)
			actionBar.setSelectedNavigationItem(category);
	}
}
