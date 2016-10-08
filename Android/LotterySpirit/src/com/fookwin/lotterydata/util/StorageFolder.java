package com.fookwin.lotterydata.util;

import java.io.*;
import java.util.ArrayList;

import com.fookwin.lotterydata.data.DataManageBase;

import android.content.Context;
import android.os.Environment;

public class StorageFolder 
{
	private File _folder = null;
	private static StorageFolder _appExternalFolder = null;
	private static StorageFolder _appPrivateFolder = null;
	
	public static StorageFolder getPrivateFolder()
	{
		if (_appPrivateFolder == null)
		{
			Context _context = DataManageBase.Instance().getAppContext();
			if (_context == null)
				return null;
			
			File target = new File(_context.getFilesDir().getPath());
			if (!target.exists())
			{
				if (!target.mkdirs())
					return null;
			}
			
			_appPrivateFolder = new StorageFolder(target);
		}
		
		return _appPrivateFolder;
	}
	
	public static StorageFolder getSDCardFolder()
	{
		if (_appExternalFolder == null)
		{
			if (!Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED))
				return null;
			
			File target = new File(Environment.getExternalStorageDirectory() + "/Fookwin/Lottery/SSQ");
			if (!target.exists())
			{
				if (!target.mkdirs())
					return null;
			}
			
			_appExternalFolder = new StorageFolder(target);
		}
		
		return _appExternalFolder;
	}

	public StorageFolder(File folder)
	{
		_folder = folder;
	}
	
	public String getPath() {
		return _folder.getPath();
	}
	
	public String getDisplayName() {
		return _folder.getName();
	}

	public ArrayList<StorageFolder> getFolders() 
	{
		if (_folder == null)
			return null;
		
		ArrayList<StorageFolder> _folders = new ArrayList<StorageFolder>();
		
		// get all sub folders.
		try
		{   
			File[] files = _folder.listFiles();
			
            if(files != null)
            {  
                for (File file : files)
                {  
                    if (!file.isFile())
                    	_folders.add(new StorageFolder(file));
                }  
            }  
        }
		catch(Exception ex)
		{  
            ex.printStackTrace();  
        }  
		
		return _folders;
	}
	
	public ArrayList<StorageFile> getFiles() 
	{
		if (_folder == null)
			return null;
		
		ArrayList<StorageFile> _files = new ArrayList<StorageFile>();
		
		// get all sub folders.
		try
		{   
			File[] files = _folder.listFiles();
				
            if(files != null)
            {  
                for (File file : files)
                {  
                    if (file.isFile())
                    	_files.add(new StorageFile(file));
                }  
            }  
        }
		catch(Exception ex)
		{  
            ex.printStackTrace();  
        }  
		
		return _files;
	}

	public StorageFile getFile(String fileName) 
	{
		if (_folder != null && fileName != "")
		{
			String filePath = _folder.getPath() + "/" + fileName;
			File tempFile = new File(filePath);
			if (tempFile.exists() && tempFile.isFile())
				return new StorageFile(tempFile);
		}
		
		return null;
	}
	
	public StorageFolder getFolder(String folderName) {
		if (_folder != null && folderName != "")
		{
			String filePath = _folder.getPath() + "/" + folderName;
			File tempFolder = new File(filePath);
			if (tempFolder.exists() && !tempFolder.isFile())
				return new StorageFolder(tempFolder);
		}
		
		return null;
	}
	
	public StorageFile createFile(String fileName) {
		if (_folder != null && fileName != "")
		{
			String filePath = _folder.getPath() + "/" + fileName;
			File tempFile = new File(filePath);
			
			// Create it.
			if (!tempFile.exists())
			{
				try {
					tempFile.createNewFile();
				} catch (IOException e) {
					return null;
				}
			}
			
			return new StorageFile(tempFile);
		}
		return null;
	}

	public StorageFolder createFolder(String folderName) {
		if (_folder != null && folderName != "")
		{
			String filePath = _folder.getPath() + "/" + folderName;
			File tempFolder = new File(filePath);
			
			// Create it.
			if (!tempFolder.exists())
			{
				if (!tempFolder.mkdirs())
					return null;
			}
			
			return new StorageFolder(tempFolder);
		}
		return null;
	}

	public void delete() {
		if (_folder != null)
		{
			deleteRecursive(_folder);
			
			_folder.delete();
		}
	}
	
	private void deleteRecursive(File fileOrDirectory) {
	    if (fileOrDirectory.isDirectory())
	    {
	        for (File child : fileOrDirectory.listFiles())
	        	deleteRecursive(child);
	    }

	    fileOrDirectory.delete();
	}
}
