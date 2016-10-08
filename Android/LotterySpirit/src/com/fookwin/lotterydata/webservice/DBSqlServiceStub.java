/**
 * DBSqlServiceStub.java
 *
 * This file was auto-generated from WSDL
 * by the Apache Axis2 version: 1.6.2  Built on : Apr 17, 2012 (05:33:49 IST)
 */
package com.fookwin.lotterydata.webservice;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import com.fookwin.lotterydata.webservice.DBSqlServiceStub;

/*
 *  DBSqlServiceStub java implementation
 */

public class DBSqlServiceStub 
{
	private static final String S_URL = "http://dbdataquery.cloudapp.net/DBSqlService.svc";
	private static final String S_ACTIONPREFIX = "http://tempuri.org/ISqlService/";
	private static final String S_NAMESPACE = "http://tempuri.org/";
	private static final int TIMEOUT = 60000;
	private static final int SOAPVERSION = SoapEnvelope.VER11;

	/**
	 * Default Constructor
	 */
	public DBSqlServiceStub() {

	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getLotteriesByIssue
	 * @param getLotteriesByIssue
	 */

	public GetLotteriesByIssueResponse getLotteriesByIssue(GetLotteriesByIssue getLotteriesByIssue) throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getLotteriesByIssue.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetLotteriesByIssue.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetLotteriesByIssueResponse response = new GetLotteriesByIssueResponse();
		response.Parse(result);

		return response;	
	}
	
	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getLotteriesBaseByIssue
	 * @param getLotteriesBaseByIssue
	 */

	public GetLotteriesBaseByIssueResponse getLotteriesBaseByIssue(GetLotteriesBaseByIssue getLotteriesBaseByIssue) throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getLotteriesBaseByIssue.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetLotteriesBaseByIssue.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetLotteriesBaseByIssueResponse response = new GetLotteriesBaseByIssueResponse();
		response.Parse(result);

		return response;	
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getLatestAttributes
	 * @param getLatestAttributes
	 */

	public GetLatestAttributesResponse getLatestAttributes(GetLatestAttributes getLatestAttributes)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getLatestAttributes.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetLatestAttributes.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetLatestAttributesResponse response = new GetLatestAttributesResponse();
		response.Parse(result);

		return response;	
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getAllLotteries
	 * @param getAllLotteries
	 */

	public GetAllLotteriesResponse getAllLotteries(GetAllLotteries getAllLotteries) throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getAllLotteries.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetAllLotteries.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetAllLotteriesResponse response = new GetAllLotteriesResponse();
		response.Parse(result);

		return response;		
	}
	
	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getAllLotteriesBase
	 * @param getAllLotteriesBase
	 */

	public GetAllLotteriesBaseResponse getAllLotteriesBase(GetAllLotteriesBase getAllLotteriesBase) throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getAllLotteriesBase.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetAllLotteriesBase.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetAllLotteriesBaseResponse response = new GetAllLotteriesBaseResponse();
		response.Parse(result);

		return response;		
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getLotteryData
	 * @param getLotteryData
	 */

	public GetLotteryDataResponse getLotteryData(GetLotteryData getLotteryData)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getLotteryData.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetLotteryData.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetLotteryDataResponse response = new GetLotteryDataResponse();
		response.Parse(result);

		return response;		
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getLotteriesByIndex
	 * @param getLotteriesByIndex
	 */

	public GetLotteriesByIndexResponse getLotteriesByIndex(GetLotteriesByIndex getLotteriesByIndex)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getLotteriesByIndex.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetLotteriesByIndex.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetLotteriesByIndexResponse response = new GetLotteriesByIndexResponse();
		response.Parse(result);

		return response;		
	}
	
	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getLotteriesBaseByIndex
	 * @param getLotteriesBaseByIndex
	 */

	public GetLotteriesBaseByIndexResponse getLotteriesBaseByIndex(GetLotteriesBaseByIndex getLotteriesBaseByIndex)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getLotteriesBaseByIndex.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetLotteriesBaseByIndex.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetLotteriesBaseByIndexResponse response = new GetLotteriesBaseByIndexResponse();
		response.Parse(result);

		return response;		
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getDataVersion
	 * @param getDataVersion
	 */

	public GetDataVersionResponse getDataVersion(GetDataVersion getDataVersion) throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getDataVersion.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);
        trans.debug = true;
        trans.call(GetDataVersion.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn; 
		
        // generate the response.
        GetDataVersionResponse response = new GetDataVersionResponse();
		response.Parse(result);

		return response;		
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getLatestReleaseInfo
	 * @param getLatestReleaseInfo
	 */

	public GetLatestReleaseInfoResponse getLatestReleaseInfo(GetLatestReleaseInfo getLatestReleaseInfo) throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getLatestReleaseInfo.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetLatestReleaseInfo.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetLatestReleaseInfoResponse response = new GetLatestReleaseInfoResponse();
		response.Parse(result);

		return response;		
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getAttributesTemplate
	 * @param getAttributesTemplate
	 */

	public GetAttributesTemplateResponse getAttributesTemplate(GetAttributesTemplate getAttributesTemplate)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getAttributesTemplate.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetAttributesTemplate.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetAttributesTemplateResponse response = new GetAttributesTemplateResponse();
		response.Parse(result);

		return response;		
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see webservice.client.test.DBSqlService#getMatrixTableItem
	 * @param getMatrixTableItem
	 */

	public GetMatrixTableItemResponse getMatrixTableItem(GetMatrixTableItem getMatrixTableItem)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getMatrixTableItem.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetMatrixTableItem.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetMatrixTableItemResponse response = new GetMatrixTableItemResponse();
		response.Parse(result);

		return response;	
	}
	
	public GetHelpResponse getHelp(GetHelp getHelp)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getHelp.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetHelp.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetHelpResponse response = new GetHelpResponse();
		response.Parse(result);

		return response;	
	}
	
	// http://dbdataquery.cloudapp.net/DBSqlService.svc
	public static class GetLatestReleaseInfo
	{	
		protected static final String METHOD = "GetLatestReleaseInfo";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		public SoapObject GetSoapObject()
		{			
			return new SoapObject(S_NAMESPACE, METHOD);
		}
	}

	public static class GetDataVersionResponse
	{
		protected String localGetDataVersionResult;

		public String getGetDataVersionResult() {
			return localGetDataVersionResult;
		}

		public void setGetDataVersionResult(String param) {
			this.localGetDataVersionResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetDataVersionResult = response.getPropertyAsString("GetDataVersionResult");
		}
	}

	public static class GetLatestAttributesResponse
	{
		protected String localGetLatestAttributesResult;

		public String getGetLatestAttributesResult() {
			return localGetLatestAttributesResult;
		}

		public void setGetLatestAttributesResult(String param) {
			this.localGetLatestAttributesResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetLatestAttributesResult = response.getPropertyAsString("GetLatestAttributesResult");
		}
	}
	
	public static class GetAttributesTemplate
	{
		protected static final String METHOD = "GetAttributesTemplate";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		public SoapObject GetSoapObject()
		{			
			return new SoapObject(S_NAMESPACE, METHOD);
		}
	}

	public static class GetMatrixTableItem
	{
		protected static final String METHOD = "GetMatrixTableItem";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected int localCandidateCount;
		protected int localSelectCount;
		
		public int getCandidateCount() {
			return localCandidateCount;
		}

		public void setCandidateCount(int param) {
			this.localCandidateCount = param;
		}

		public int getSelectCount() {
			return localSelectCount;
		}

		public void setSelectCount(int param) {
			this.localSelectCount = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("candidateCount", localCandidateCount);
			output.addProperty("selectCount", localSelectCount);
			
			return output;
		}
	}

	public static class GetLotteryData
	{
		protected static final String METHOD = "GetLotteryData";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected int localIssue;

		public int getIssue() {
			return localIssue;
		}

		public void setIssue(int param) {
			this.localIssue = param;
		}
		
		public void Parse(SoapObject response)
		{
			localIssue = Integer.parseInt(response.getPropertyAsString("issue"));
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("issue", localIssue);
			
			return output;
		}
	}

	public static class GetLotteriesByIssueResponse
	{
		protected String localGetLotteriesByIssueResult;

		public String getGetLotteriesByIssueResult() {
			return localGetLotteriesByIssueResult;
		}

		public void setGetLotteriesByIssueResult(String param) {
			this.localGetLotteriesByIssueResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetLotteriesByIssueResult = response.getPropertyAsString("GetLotteriesByIssueResult");
		}
	}
	
	public static class GetLotteriesBaseByIssueResponse
	{
		protected String localGetLotteriesBaseByIssueResult;

		public String getGetLotteriesBaseByIssueResult() {
			return localGetLotteriesBaseByIssueResult;
		}

		public void setGetLotteriesBaseByIssueResult(String param) {
			this.localGetLotteriesBaseByIssueResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetLotteriesBaseByIssueResult = response.getPropertyAsString("GetLotteriesBaseByIssueResult");
		}
	}
	
	public static class GetAllLotteriesResponse
	{
		protected String localGetAllLotteriesResult;

		public String getGetAllLotteriesResult() {
			return localGetAllLotteriesResult;
		}

		public void setGetAllLotteriesResult(String param) {
			this.localGetAllLotteriesResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetAllLotteriesResult = response.getPropertyAsString("GetAllLotteriesResult");
		}
	}
	
	public static class GetAllLotteriesBaseResponse
	{
		protected String localGetAllLotteriesBaseResult;

		public String getGetAllLotteriesBaseResult() {
			return localGetAllLotteriesBaseResult;
		}

		public void setGetAllLotteriesBaseResult(String param) {
			this.localGetAllLotteriesBaseResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetAllLotteriesBaseResult = response.getPropertyAsString("GetAllLotteriesBaseResult");
		}
	}

	public static class GetLotteriesByIssue 
	{
		protected static final String METHOD = "GetLotteriesByIssue";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected int localIssue_from;
		protected int localIssue_to;

		public int getIssue_from() {
			return localIssue_from;
		}

		public void setIssue_from(int param) {
			this.localIssue_from = param;
		}		

		public int getIssue_to() {
			return localIssue_to;
		}

		public void setIssue_to(int param) {
			this.localIssue_to = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("issue_from", localIssue_from);
			output.addProperty("issue_to", localIssue_to);
			
			return output;
		}
	}
	
	public static class GetLotteriesBaseByIssue 
	{
		protected static final String METHOD = "GetLotteriesBaseByIssue";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected int localIssue_from;
		protected int localIssue_to;

		public int getIssue_from() {
			return localIssue_from;
		}

		public void setIssue_from(int param) {
			this.localIssue_from = param;
		}		

		public int getIssue_to() {
			return localIssue_to;
		}

		public void setIssue_to(int param) {
			this.localIssue_to = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("issue_from", localIssue_from);
			output.addProperty("issue_to", localIssue_to);
			
			return output;
		}
	}

	public static class GetMatrixTableItemResponse
	{
		protected String localGetMatrixTableItemResult;

		public String getGetMatrixTableItemResult() {
			return localGetMatrixTableItemResult;
		}

		public void setGetMatrixTableItemResult(String param) {
			this.localGetMatrixTableItemResult = param;
		}

		public void Parse(SoapObject response)
		{
			localGetMatrixTableItemResult = response.getPropertyAsString("GetMatrixTableItemResult");
		}
	}
	
	public static class GetAllLotteries
	{
		protected static final String METHOD = "GetAllLotteries";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		public SoapObject GetSoapObject()
		{			
			return new SoapObject(S_NAMESPACE, METHOD);
		}
	}
	
	public static class GetAllLotteriesBase
	{
		protected static final String METHOD = "GetAllLotteriesBase";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		public SoapObject GetSoapObject()
		{			
			return new SoapObject(S_NAMESPACE, METHOD);
		}
	}

	public static class GetDataVersion
	{
		protected static final String METHOD = "GetDataVersion";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		public SoapObject GetSoapObject()
		{			
			return new SoapObject(S_NAMESPACE, METHOD);
		}
	}

	public static class GetAttributesTemplateResponse
	{
		protected String localGetAttributesTemplateResult;

		public String getGetAttributesTemplateResult() {
			return localGetAttributesTemplateResult;
		}

		public void setGetAttributesTemplateResult(String param){
			this.localGetAttributesTemplateResult = param;
		}

		public void Parse(SoapObject response)
		{
			localGetAttributesTemplateResult = response.getPropertyAsString("GetAttributesTemplateResult");
		}
	}

	public static class GetLatestReleaseInfoResponse
	{
		protected String localGetLatestReleaseInfoResult;

		public String getGetLatestReleaseInfoResult() {
			return localGetLatestReleaseInfoResult;
		}

		public void setGetLatestReleaseInfoResult(String param) {
			this.localGetLatestReleaseInfoResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetLatestReleaseInfoResult = response.getPropertyAsString("GetLatestReleaseInfoResult");
		}
	}

	public static class GetLotteryDataResponse 
	{
		protected String localGetLotteryDataResult;

		public String getGetLotteryDataResult() {
			return localGetLotteryDataResult;
		}

		public void setGetLotteryDataResult(String param) {
			this.localGetLotteryDataResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetLotteryDataResult = response.getPropertyAsString("GetLotteryDataResult");
		}
	}

	public static class GetLotteriesByIndexResponse
	{
		protected String localGetLotteriesByIndexResult;

		public String getGetLotteriesByIndexResult() {
			return localGetLotteriesByIndexResult;
		}

		public void setGetLotteriesByIndexResult(String param) {
			this.localGetLotteriesByIndexResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetLotteriesByIndexResult = response.getPropertyAsString("GetLotteriesByIndexResult");
		}
	}
	
	public static class GetLotteriesBaseByIndexResponse
	{
		protected String localGetLotteriesBaseByIndexResult;

		public String getGetLotteriesBaseByIndexResult() {
			return localGetLotteriesBaseByIndexResult;
		}

		public void setGetLotteriesBaseByIndexResult(String param) {
			this.localGetLotteriesBaseByIndexResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetLotteriesBaseByIndexResult = response.getPropertyAsString("GetLotteriesBaseByIndexResult");
		}
	}

	public static class GetLatestAttributes
	{
		protected static final String METHOD = "GetLatestAttributes";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		public SoapObject GetSoapObject()
		{			
			return new SoapObject(S_NAMESPACE, METHOD);
		}
	}

	public static class GetLotteriesByIndex
	{
		protected static final String METHOD = "GetLotteriesByIndex";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected int localIndex_from;
		protected int localIndex_to;

		public int getIndex_from() {
			return localIndex_from;
		}

		public void setIndex_from(int param) {
			this.localIndex_from = param;
		}

		public int getIndex_to() {
			return localIndex_to;
		}

		public void setIndex_to(int param) {
			this.localIndex_to = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("index_from", localIndex_from);
			output.addProperty("index_to", localIndex_to);
			
			return output;
		}
	}
	
	public static class GetLotteriesBaseByIndex
	{
		protected static final String METHOD = "GetLotteriesBaseByIndex";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected int localIndex_from;
		protected int localIndex_to;

		public int getIndex_from() {
			return localIndex_from;
		}

		public void setIndex_from(int param) {
			this.localIndex_from = param;
		}

		public int getIndex_to() {
			return localIndex_to;
		}

		public void setIndex_to(int param) {
			this.localIndex_to = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("index_from", localIndex_from);
			output.addProperty("index_to", localIndex_to);
			
			return output;
		}
	}

	public static class GetHelp
	{
		protected static final String METHOD = "GetHelp";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		public SoapObject GetSoapObject()
		{			
			return new SoapObject(S_NAMESPACE, METHOD);
		}
	}
	
	public static class GetHelpResponse
	{
		protected String localGetHelpResult;

		public String getGetHelpResult() {
			return localGetHelpResult;
		}

		public void setGetHelpResult(String param) {
			this.localGetHelpResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetHelpResult = response.getPropertyAsString("GetHelpResult");
		}
	}

}
