package com.fookwin.lotteryspirit.util;

import android.graphics.Bitmap;
import android.graphics.Matrix;

public class BitmapUtil
{
   public static Bitmap GetNewBitmap(Bitmap oldBitmap,float screenWidth,float screenHeight,float newWidth,float newHeight)
   {
	   //获取图片原始大小
	    int width = oldBitmap.getWidth();
	    int height = oldBitmap.getHeight();
	   //获取屏幕大小		 
        float scaleWidth  = newWidth  / width;
        float scaleHeight = newHeight / height;
        //得到新图片
        Matrix matrix = new Matrix(); 
        matrix.postScale(scaleWidth,scaleHeight); //长和宽放大缩小的比例
        Bitmap newBitmap = Bitmap.createBitmap(oldBitmap,0,0,width,height,matrix,true);
	    return newBitmap;
   }
}
