package com.fookwin.lotterydata.data;

import java.util.ArrayList;

import com.fookwin.lotterydata.util.SelectUtil;

public class Purchase
{
	private ArrayList<SchemeSelector> _selectors = new ArrayList<SchemeSelector>();
	private ArrayList<Constraint> _filters = new ArrayList<Constraint>();
	private ArrayList<Scheme> _selectionOriginal = new ArrayList<Scheme>();
	private ArrayList<Scheme> _selectionFinal = new ArrayList<Scheme>();

	private boolean filterRecomputeRequired = false;
	private boolean selectorRecomputeRequired = false;
	private boolean previousComputeFailed = false;
	
	private int _selectedCount = 0;
	private int _filteredCount = 0;
	private int _removedCount = 0;
	
	private int _id = -1; // -1 means not saved.
	private boolean _dirty = false;
	
	public final boolean isDirty()
	{
		return _dirty;
	}
	
	public final void setDirty(boolean dirty)
	{
		_dirty = dirty;
	}
	
	public final void setId(int id)
	{
		_id = id;
	}
	
	public final int getId()
	{
		return _id;
	}
	
	public final int getSelectedCount()
	{
		return _selectedCount;
	}
	
	public final int getFilteredCount()
	{
		return _filteredCount;
	}
	
	public final int getRemovedCount()
	{
		return _removedCount;
	}
	
	public final ArrayList<SchemeSelector> getSelectors()
	{
		return _selectors;
	}

	public final ArrayList<Constraint> getConstraints()
	{
		return _filters;
	}

	public final ArrayList<Scheme> getSelection()
	{
		// compute the result if needed.
		compute();
		
		return _selectionFinal;
	}
	
	public final ArrayList<Scheme> getOriginalSelection()
	{
		// compute the result if needed.
		compute();
		
		return _selectionOriginal;
	}
	
	public boolean requireCompute()
	{
		return previousComputeFailed || selectorRecomputeRequired || filterRecomputeRequired;
	}
	
	public void markSelectorsRecomputeRequired()
	{
		selectorRecomputeRequired = true;
	}
	
	public void markConstraintRecomputeRequired()
	{
		filterRecomputeRequired = true;
	}
	
	@SuppressWarnings("unchecked")
	public void removeScheme(int pos)
	{
		if (_selectionFinal == _selectionOriginal)
		{
			// make a clone to cache the original selection
			_selectionFinal = (ArrayList<Scheme>) _selectionOriginal.clone();
		}
		
		// remove it.
		_selectionFinal.remove(pos);
		_removedCount ++;
	}
	
	public void resumeSelection()
	{
		// put the final back as original
		_selectionFinal = _selectionOriginal;
		_removedCount = 0;
	}
	
	@SuppressWarnings("unchecked")
	public void randomReomveSchemes(int remainCount, boolean resumeOriginal)
	{
		if (resumeOriginal)
		{
			// resume to the original selection, so that we can get different random result each time.
			resumeSelection();
		}
		
		if (_selectionFinal == _selectionOriginal)
		{
			// make a clone to cache the original selection
			_selectionFinal = (ArrayList<Scheme>) _selectionOriginal.clone();
		}
		
		int preCount = _selectionFinal.size();
		SelectUtil.RadomRemain(_selectionFinal, remainCount);
		_removedCount += preCount - _selectionFinal.size();
	}
	
	private void compute()
	{
		if (previousComputeFailed)
		{
			// recompute is required.
			selectorRecomputeRequired = true;
			filterRecomputeRequired = true;
		}
		
		if (selectorRecomputeRequired)
		{
            try
            {
            	_selectionOriginal.clear();
	            for (SchemeSelector selector : _selectors)
	            {
	            	_selectionOriginal.addAll(selector.GetResult());
	            }
            }
            catch (OutOfMemoryError e)
            {
            	previousComputeFailed = true;
            	throw e;
            }
            
    		// Reset the result
            _selectedCount = _selectionOriginal.size();
            _filteredCount = 0;
            
			selectorRecomputeRequired = false;
			filterRecomputeRequired = true; // re-filtering is required too.
			
			_dirty = true;
		}
		
		if (filterRecomputeRequired)
		{
            // Re-filtering the scheme selection.
            SelectUtil.Filter(_selectionOriginal, _filters);
            
            _filteredCount = _selectedCount - _selectionOriginal.size();
    		
			filterRecomputeRequired = false;
			
			// have to get rid of user's removal.
			_removedCount = 0;			
			_selectionFinal = _selectionOriginal;
			
			_dirty = true;
		}
		
		previousComputeFailed = false;	
	}
}