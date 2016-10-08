package com.fookwin.lotterydata.util;

import java.io.*;
import java.util.ArrayList;

public class StorageFile 
{
	private File _file = null;
	
	public StorageFile(File file)
	{
		_file = file;
	}
	
	public String readString() {
		if (_file == null)
			return null;
		
		try 
		{ 
			FileInputStream inStream = new FileInputStream(_file);
			ByteArrayOutputStream outStream = new ByteArrayOutputStream(); 
			
			byte[] buffer = new byte[1024]; 
			int length = -1; 
			while((length = inStream.read(buffer)) != -1 ){ 
				outStream.write(buffer, 0, length); 
			} 
			outStream.close(); 
			inStream.close(); 
			
			return outStream.toString();
		}
		catch (IOException e)
		{ 
			return null;			
		} 
	}

	public ArrayList<String> readLines() 
	{
		if (_file == null)
			return null;
		
		BufferedReader reader = null;  

		try 
		{
			ArrayList<String> output = new ArrayList<String>();
			
			reader = new BufferedReader(new FileReader(_file));  

			String tempString = null;  

			while ((tempString = reader.readLine()) != null)
			{ 
				output.add(tempString);
			}  

			reader.close();
			
			return output;
		} 
		catch (IOException e) 
		{  
			e.printStackTrace();
		} 
		finally 
		{ 
			if (reader != null) 
			{ 
				try {
					reader.close();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
		
		return null;
	}

	public void writeText(String text)
	{
		if (_file == null)
			return;	
		
		try
		{
			FileOutputStream fout = new FileOutputStream(_file);   
			byte [] bytes = text.getBytes();   
	 
			fout.write(bytes);   
			fout.close();   
		}  
		catch(Exception e)
		{   
			e.printStackTrace();   
		} 
	}
	
	public String getPath()
	{
		return _file.getPath();
	}
	
	public String getDisplayName()
	{
		return _file.getName();
	}
	
	public void delete()
	{
		if (_file != null)
			_file.delete();
	}
}
