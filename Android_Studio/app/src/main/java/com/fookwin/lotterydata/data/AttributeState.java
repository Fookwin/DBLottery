package com.fookwin.lotterydata.data;

import org.json.JSONException;
import org.json.JSONObject;

public class AttributeState
{
	private String _key = "";
	private int _value = 0;

	public final String getKey()
	{
		return _key;
	}
	public final void setKey(String value)
	{
		_key = value;
	}

	public final int getValue()
	{
		return _value;
	}
	public final void setValue(int value)
	{
		_value = value;
	}
	
	@Override
	public String toString() {
		return toJSON().toString();
	}
	
	public JSONObject toJSON()
	{
		JSONObject object = new JSONObject();
		try {
			object.put("key", _key);
			object.put("val", _value);
		} catch (JSONException e) {
			e.printStackTrace();
		}
		
		return object;
	}
	
	public void parseJSON(JSONObject json)
	{
		try {
			_key = json.getString("key");
			_value = json.getInt("val");
		} catch (JSONException e) {
			e.printStackTrace();
		}		
	}
}