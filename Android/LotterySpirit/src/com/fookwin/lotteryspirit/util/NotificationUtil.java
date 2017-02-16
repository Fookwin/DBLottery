package com.fookwin.lotteryspirit.util;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.widget.Toast;

public class NotificationUtil
{
	// Show a hint message box and auto disappear after timeout 
	public static void ShowMessage(Context ct, String msg)
	{
        // Show a hint message box and auto disappear after timeout
		Toast toast = Toast.makeText(ct, msg, Toast.LENGTH_SHORT);
		toast.show();
	}
	
	public static void ShowDialog(
			Context ct, 
			String title, 
			String message, 
			String posText,
			String nevText,
			boolean bCancelable,
			DialogInterface.OnClickListener posHandler,
			DialogInterface.OnClickListener nevHandler)
	{
		Dialog dlg = new AlertDialog.Builder(ct)
				.setTitle(title)
				.setMessage(message)
				.setCancelable(bCancelable)
				.setNegativeButton(nevText,
						nevHandler != null ? nevHandler : 
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int which) {
								dialog.cancel();
							}
						})
				.setPositiveButton(posText,
						posHandler != null ? posHandler : 
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int which) {

							}
						}).create();
		dlg.show();
	}
}
