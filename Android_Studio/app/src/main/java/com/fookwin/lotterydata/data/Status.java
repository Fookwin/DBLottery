package com.fookwin.lotterydata.data;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.fookwin.lotterydata.util.DBXmlNode;

public class Status
{
	private NumberState[] _redNumStates = null;
	private NumberState[] _blueNumStates = null;

	public final static Status parseFrom1(String str) throws JSONException
	{
		JSONObject json = new JSONObject(str);
		
		Status obj = new Status();
		obj.parseJSON(json);
		
		return obj;
	}
	
	public final NumberState[] getRedNumStates()
	{
		return _redNumStates;
	}

	public final NumberState[] getBlueNumStates()
	{
		return _blueNumStates;
	}

	public final void ReadFromXml(DBXmlNode node)
	{
		_reset();
		
		String[] reds_hit = node.GetAttribute("Red_HitCount").split("[ ]", -1);
		String[] reds_omission = node.GetAttribute("Red_Omission").split("[ ]", -1);
		String[] reds_temp = node.GetAttribute("Red_Temperature").split("[ ]", -1);

		for (int i = 0; i < 33; ++i)
		{
			if (_redNumStates[i] == null)
			{
				_redNumStates[i] = new NumberState();
			}

			_redNumStates[i].setHitCount(Integer.parseInt(reds_hit[i]));
			_redNumStates[i].setOmission(Integer.parseInt(reds_omission[i]));
			_redNumStates[i].setTemperature(Integer.parseInt(reds_temp[i]));
		}

		String[] blues_hit = node.GetAttribute("Blue_HitCount").split("[ ]", -1);
		String[] blues_omission = node.GetAttribute("Blue_Omission").split("[ ]", -1);
		String[] blues_temp = node.GetAttribute("Blue_Temperature").split("[ ]", -1);

		for (int i = 0; i < 16; ++i)
		{
			if (_blueNumStates[i] == null)
			{
				_blueNumStates[i] = new NumberState();
			}

			_blueNumStates[i].setHitCount(Integer.parseInt(blues_hit[i]));
			_blueNumStates[i].setOmission(Integer.parseInt(blues_omission[i]));
			_blueNumStates[i].setTemperature(Integer.parseInt(blues_temp[i]));
		}
	}

	private void _reset()
	{
		_redNumStates = new NumberState[33];
		_blueNumStates = new NumberState[16];
	}

	@Override
	public String toString()
	{
		return toJSON().toString();
	}
	
	private JSONObject toJSON()
	{
		JSONObject js = new JSONObject();
		
		if (_redNumStates == null)
			return js;

		try {
		
			JSONArray array_r = new JSONArray();
			for (NumberState state : _redNumStates)
			{
				array_r.put(state.toJSON());
			}			
			js.put("red", array_r);

			JSONArray array_b = new JSONArray();
			for (NumberState state : _blueNumStates)
			{
				array_b.put(state.toJSON());
			}
			js.put("blue", array_b);

		} catch (JSONException e) {
			e.printStackTrace();
		}
		
		return js;
	}
	
	private void parseJSON(JSONObject js)
	{
		_reset();
		
		try {
			JSONArray array_r = js.getJSONArray("red");
			if (array_r != null)
			{
				for (int i = 0; i < 33; ++i)
				{
					if (_redNumStates[i] == null)
					{
						_redNumStates[i] = new NumberState();
					}
					
					_redNumStates[i].parseJSON(array_r.getJSONObject(i));
				}
			}
			
			JSONArray array_b = js.getJSONArray("blue");
			if (array_b != null)
			{
				for (int i = 0; i < 16; ++i)
				{
					if (_blueNumStates[i] == null)
					{
						_blueNumStates[i] = new NumberState();
					}
					
					_blueNumStates[i].parseJSON(array_b.getJSONObject(i));
				}
			}
		} catch (JSONException e) {
			e.printStackTrace();
		}
	}
}