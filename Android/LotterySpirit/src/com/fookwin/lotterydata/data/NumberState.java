package com.fookwin.lotterydata.data;

import org.json.JSONException;
import org.json.JSONObject;

public class NumberState
{
	private int _hitCount = 0;
	private int _omission = 0;
	private int _temperature = 0; // <2 -> cool; >=2 & <= 8 -> normal; > 8 hot;

	public final int getHitCount()
	{
		return _hitCount;
	}
	public final void setHitCount(int value)
	{
		_hitCount = value;
	}

	public final int getOmission()
	{
		return _omission;
	}
	public final void setOmission(int value)
	{
		_omission = value;
	}

	public final int getTemperature()
	{
		return _temperature;
	}
	public final void setTemperature(int value)
	{
		_temperature = value;
	}
	
	@Override
	public String toString() {
		return toJSON().toString();
	}
	
	public JSONObject toJSON()
	{
		JSONObject object = new JSONObject();
		try {
			object.put("hit", _hitCount);
			object.put("omi", _omission);
			object.put("tmp", _temperature);
		} catch (JSONException e) {
			e.printStackTrace();
		}
		
		return object;
	}
	
	public void parseJSON(JSONObject json)
	{
		try {
			_hitCount = json.getInt("hit");
			_omission = json.getInt("omi");
			_temperature = json.getInt("tmp");
		} catch (JSONException e) {
			e.printStackTrace();
		}		
	}
}