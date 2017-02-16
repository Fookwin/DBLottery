package com.fookwin.lotteryspirit.data;

import java.text.ParseException;
import java.util.List;

import com.fookwin.lotterydata.data.History;
import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotteryspirit.data.DiagramOptions.SubCategory;

public class DiagramData
{	
	private enum ConnectionTypeEnum
	{
		HORIZONTAL,
		VERTICAL,
		OBLIQUE,
		EVENOBLIQUE,
		ODDOBLIQUE
	}
	
	private Lottery source;
	private int blue_span = -1;
	
	private DiagramData previous, next;
	
	public static void generateDiagramData(List<DiagramData> output) throws ParseException
	{
		History history = LBDataManager.GetInstance().getHistory();
		
		DiagramData _previous = null;
		int size = history.getCount(), limit = 30;
		for (int i = size - limit; i < size ; ++ i)
		{
			DiagramData data = new DiagramData();
			data.source = history.getByIndex(i);
			data.source.initDetails();

			if (_previous != null)
			{
				data.previous = _previous;
				_previous.next = data;
			}
			
			output.add(data);
			
			_previous = data;
		}	
	}
	
	private int getConnectionCount(int num, ConnectionTypeEnum con_type, boolean upper, boolean left)
	{
		int count = 0;
		switch (con_type)
		{
		case EVENOBLIQUE:
		{
			if (left)
			{				
				DiagramData test = upper ? previous : next;
				if (test != null && num > 2 && test.source.getScheme().Contains(num - 2))
				{
					++ count;
				
					test = upper ? previous.previous : next.next;
					if (test != null && num > 4 && test.source.getScheme().Contains(num - 4))
						++ count;
					
					// detect 2 is enough.
				}
			}
			else
			{
				DiagramData test = upper ? previous : next;
				if (test != null && num < 31 && test.source.getScheme().Contains(num + 2))
				{
					++ count;
				
					test = upper ? previous.previous : next.next;
					if (test != null && num < 29 && test.source.getScheme().Contains(num + 4))
						++ count;
					
					// detect 2 is enough.
				}			
			}
			
			break;
		}
		case HORIZONTAL:
		{
			if (left)
			{
				if (num > 1 && source.getScheme().Contains(num - 1))
				{
					++ count;
				
					if (num > 2 && source.getScheme().Contains(num - 2))
						++ count;
					
					// detect 2 is enough.
				}
			}
			else
			{
				if (num < 33 && source.getScheme().Contains(num + 1))
				{
					++ count;
				
					if (num < 32 && source.getScheme().Contains(num + 2))
						++ count;
					
					// detect 2 is enough.
				}			
			}				
				
			break;
		}
		case OBLIQUE:
		{
			if (left)
			{				
				DiagramData test = upper ? previous : next;
				if (test != null && num > 1 && test.source.getScheme().Contains(num - 1))
				{
					++ count;
				
					test = upper ? previous.previous : next.next;
					if (test != null && num > 2 && test.source.getScheme().Contains(num - 2))
						++ count;
					
					// detect 2 is enough.
				}
			}
			else
			{
				DiagramData test = upper ? previous : next;
				if (test != null && num < 33 && test.source.getScheme().Contains(num + 1))
				{
					++ count;
				
					test = upper ? previous.previous : next.next;
					if (test != null && num < 32 && test.source.getScheme().Contains(num + 2))
						++ count;
					
					// detect 2 is enough.
				}			
			}
			
			break;
		}
		case ODDOBLIQUE:
		{
			if (left)
			{				
				DiagramData test = upper ? previous : next;
				if (test != null && num > 2 && test.source.getScheme().Contains(num - 2))
				{
					++ count;
				
					test = upper ? previous.previous : next.next;
					if (test != null && num > 4 && test.source.getScheme().Contains(num - 4))
						++ count;
					
					// detect 2 is enough.
				}
			}
			else
			{
				DiagramData test = upper ? previous : next;
				if (test != null && num < 32 && test.source.getScheme().Contains(num + 2))
				{
					++ count;
				
					test = upper ? previous.previous : next.next;
					if (test != null && num < 30 && test.source.getScheme().Contains(num + 4))
						++ count;
					
					// detect 2 is enough.
				}			
			}
			
			break;
		}
		case VERTICAL:
		{
			if (upper)
			{
				if (previous != null && previous.source.getScheme().Contains(num))
				{
					++ count;
				
					if (previous.previous != null && previous.previous.source.getScheme().Contains(num))
						++ count;
					
					// detect 2 is enough.
				}
			}
			else
			{
				if (next != null && next.source.getScheme().Contains(num))
				{
					++ count;
				
					if (next.next != null && next.next.source.getScheme().Contains(num))
						++ count;
					
					// detect 2 is enough.
				}			
			}
			
			break;
		}
		default:
			break;		
		}
		return count;
	}

	public Lottery getSource() {
		return source;
	}
	
	public int getBlueSpan() {
		if (blue_span < 0)
		{
			if (previous == null)
				blue_span = 0;
			else
				blue_span = Math.abs(source.getScheme().getBlue() - previous.source.getScheme().getBlue());
		}
		return blue_span;
	}

	public DiagramData getPrevious() {
		return previous;
	}

	public DiagramData getNext() {
		return next;
	}

	public boolean isObliqueConnected(int num) 
	{
		int pre_count = getConnectionCount(num, ConnectionTypeEnum.OBLIQUE, true, true);
		if (pre_count > 1)
			return true;
		
		int post_count = getConnectionCount(num, ConnectionTypeEnum.OBLIQUE, false, false);
		if (post_count + pre_count > 1)
			return true;
		
		pre_count = getConnectionCount(num, ConnectionTypeEnum.OBLIQUE, true, false);
		if (pre_count > 1)
			return true;
		
		post_count = getConnectionCount(num, ConnectionTypeEnum.OBLIQUE, false, true);
		if (post_count + pre_count > 1)
			return true;
		
		return false;
	}

	public boolean isHorizontalConnected(int num)
	{
		int pre_count = getConnectionCount(num, ConnectionTypeEnum.HORIZONTAL, true, true);
		if (pre_count > 1)
			return true;
		
		int post_count = getConnectionCount(num, ConnectionTypeEnum.HORIZONTAL, true, false);
		if (post_count + pre_count > 1)
			return true;
		
		return false;
	}

	public boolean isVerticalConnected(int num) 
	{
		int pre_count = getConnectionCount(num, ConnectionTypeEnum.VERTICAL, true, true);
		if (pre_count > 1)
			return true;
		
		int post_count = getConnectionCount(num, ConnectionTypeEnum.VERTICAL, false, true);
		if (post_count + pre_count > 1)
			return true;
		
		return false;
	}

	public boolean isEvenObliqueConnected(int num) 
	{
		if (num % 2 == 1)
			return false;
		
		int pre_count = getConnectionCount(num, ConnectionTypeEnum.EVENOBLIQUE, true, true);
		if (pre_count > 1)
			return true;
		
		int post_count = getConnectionCount(num, ConnectionTypeEnum.EVENOBLIQUE, false, false);
		if (post_count + pre_count > 1)
			return true;
		
		pre_count = getConnectionCount(num, ConnectionTypeEnum.EVENOBLIQUE, true, false);
		if (pre_count > 1)
			return true;
		
		post_count = getConnectionCount(num, ConnectionTypeEnum.EVENOBLIQUE, false, true);
		if (post_count + pre_count > 1)
			return true;
		
		return false;
	}

	public boolean isOddObliqueConnected(int num) 
	{
		if (num % 2 == 0)
			return false;
		
		int pre_count = getConnectionCount(num, ConnectionTypeEnum.ODDOBLIQUE, true, true);
		if (pre_count > 1)
			return true;
		
		int post_count = getConnectionCount(num, ConnectionTypeEnum.ODDOBLIQUE, false, false);
		if (post_count + pre_count > 1)
			return true;
		
		pre_count = getConnectionCount(num, ConnectionTypeEnum.ODDOBLIQUE, true, false);
		if (pre_count > 1)
			return true;
		
		post_count = getConnectionCount(num, ConnectionTypeEnum.ODDOBLIQUE, false, true);
		if (post_count + pre_count > 1)
			return true;
		
		return false;
	}
	
	public boolean[] getDivisionsOmissionBreak(SubCategory op)
	{	
		int div = DiagramOptions.getDivisionCount(op);
		boolean[] output = new boolean[div];
		
		for (int i = 0; i < div; ++ i)
		{
			if (div == 5 && i == 2)
				output[i] = false;
			else
				output[i] = true;
		}
		
		switch (op)
		{
		case RedDivisionIn3:
		{
			for (int i = 1; i <= 33;)
			{
				if (source.getScheme().Contains(i))
				{
					int divIndex = DiagramOptions.getDivisionIndex(op, i);
					output[divIndex] = false;
					
					// goto the first num in next division.
					i = (divIndex + 1) * 11 + 1;
					
					continue;
				}
				
				 ++ i;
			}
			break;
		}
		case RedDivisionIn4:
		{
			for (int i = 1; i <= 33;)
			{
				if (source.getScheme().Contains(i))
				{
					int divIndex = DiagramOptions.getDivisionIndex(op, i);
					output[divIndex] = false;
					
					// goto the first num in next division.
					if (divIndex == 0)
						i = 9;
					else if (divIndex == 1 || divIndex == 2)
						i = 18;
					else if (divIndex == 3)
						i = 26;
					else
						break;
					
					continue;
				}
				
				 ++ i;
			}
			break;
		}
		case RedDivisionIn7:
		{
			for (int i = 1; i <= 33;)
			{
				if (source.getScheme().Contains(i))
				{
					int divIndex = DiagramOptions.getDivisionIndex(op, i);
					output[divIndex] = false;
					
					// goto the first num in next division.
					i = (divIndex + 1) * 5 + 1;
					
					continue;
				}
				
				 ++ i;
			}
			break;
		}
		case RedDivisionIn11:
		{
			for (int i = 1; i <= 33;)
			{
				if (source.getScheme().Contains(i))
				{
					int divIndex = DiagramOptions.getDivisionIndex(op, i);
					output[divIndex] = false;
					
					// goto the first num in next division.
					i = (divIndex + 1) * 3 + 1;
					
					continue;
				}
				
				 ++ i;
			}
			break;
		}
		default:
			return output;
		}
		
		return output;
	}
}
