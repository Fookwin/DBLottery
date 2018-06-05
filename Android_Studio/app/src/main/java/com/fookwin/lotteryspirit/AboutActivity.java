package com.fookwin.lotteryspirit;

import android.app.ActionBar;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.view.MenuItem;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;

public class AboutActivity extends Activity {

	// override default behaviour of the browser
	private class MyWebViewClient extends WebViewClient {
		@Override
		public boolean shouldOverrideUrlLoading(WebView view, String url) {
			//view.loadUrl(url);
			//return true;
			return false;
		}

		public void onReceivedError(WebView view, int errorCode, String description, String failingUrl) {
			try {
				view.stopLoading();
			} catch (Exception e) {
			}

			if (view.canGoBack()) {
				view.goBack();
			}

			view.loadUrl("about:blank");
			AlertDialog alertDialog = new AlertDialog.Builder(AboutActivity.this).create();
			alertDialog.setTitle("Error");
			alertDialog.setMessage("Check your internet connection and try again.");
			alertDialog.setButton(DialogInterface.BUTTON_POSITIVE, "Try Again", new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog, int which) {
					finish();
					startActivity(getIntent());
				}
			});

			alertDialog.show();
			super.onReceivedError(view, errorCode, description, failingUrl);
		}
	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_about);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);

		// display web view
		WebView myWebView = (WebView) findViewById(R.id.webview);
		myWebView.setWebViewClient(new AboutActivity.MyWebViewClient());
		WebSettings webSettings = myWebView.getSettings();
		webSettings.setJavaScriptEnabled(true);
		webSettings.setDomStorageEnabled(true);
		myWebView.loadUrl("http://www.fookwin.com/fookwin.management");
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	            finish();
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
}
