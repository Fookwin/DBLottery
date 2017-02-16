/**
 * DBUseControlServiceStub.java
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

/*
 *  DBUseControlServiceStub java implementation
 */

public class DBUseControlServiceStub 
{
	private static final String S_URL = "http://dbdataquery.cloudapp.net/DBUseControlService.svc";
	private static final String S_ACTIONPREFIX = "http://tempuri.org/IUseControlService/";
	private static final String S_NAMESPACE = "http://tempuri.org/";
	private static final int TIMEOUT = 60000;
	private static final int SOAPVERSION = SoapEnvelope.VER11;

	/**
	 * Default Constructor
	 */
	public DBUseControlServiceStub() {

	}

	/**
	 * Auto generated method signature
	 * 
	 * @see DBUseControlService#getReleaseNotes
	 * @param getReleaseNotes
	 */

	public GetReleaseNotesResponse getReleaseNotes(GetReleaseNotes getReleaseNotes) throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = getReleaseNotes.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetReleaseNotes.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        GetReleaseNotesResponse response = new GetReleaseNotesResponse();
		response.Parse(result);

		return response;
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see DBUseControlService#getLatestSoftwareVersion
	 * @param getLatestSoftwareVersion
	 * @throws Exception 
	 */

	public GetLatestSoftwareVersionResponse getLatestSoftwareVersion(GetLatestSoftwareVersion getLatestSoftwareVersion) throws Exception
	{		
		// construct the input soap object for the request.
		SoapObject input = getLatestSoftwareVersion.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(GetLatestSoftwareVersion.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }            
            
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
		GetLatestSoftwareVersionResponse response = new GetLatestSoftwareVersionResponse();
		response.Parse(result);

		return response;	
	}

	/**
	 * Auto generated method signature
	 * 
	 * @see DBUseControlService#userLogin
	 * @param userLogin
	 */

	public UserLoginResponse userLogin(UserLogin userLogin)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = userLogin.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(UserLogin.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        UserLoginResponse response = new UserLoginResponse();
        response.Parse(result);

		return response;
	}

	
	public static class GetReleaseNotesResponse
	{
		protected String localGetReleaseNotesResult;

		public String getGetReleaseNotesResult() {
			return localGetReleaseNotesResult;
		}

		public void setGetReleaseNotesResult(String param) {
			this.localGetReleaseNotesResult = param;
		}
		
		public void Parse(SoapObject response)
		{
			localGetReleaseNotesResult = response.getPropertyAsString("GetReleaseNotesResult");
		}
	}
	
	
	public static class GetReleaseNotes
	{
		protected static final String METHOD = "GetReleaseNotes";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected int localPlatform;
		protected int localClientVersion;

		public int getPlatform() {
			return localPlatform;
		}

		public void setPlatform(int param) {
			this.localPlatform = param;
		}

		public int getClientVersion() {
			return localClientVersion;
		}
		
		public void setClientVersion(int param) {
			this.localClientVersion = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("platform", localPlatform);
			output.addProperty("clientVersion", localClientVersion);
			
			return output;
		}
	}
	
	public PostFeedbackResponse postFeedback(PostFeedback postFeedback)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = postFeedback.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(PostFeedback.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        PostFeedbackResponse response = new PostFeedbackResponse();
        response.Parse(result);

		return response;
	}
	
	public PostRecordResponse postRecord(PostRecord postRecord)	throws Exception
	{
		// construct the input soap object for the request.
		SoapObject input = postRecord.GetSoapObject();
		
		// construct the soap envelope.
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SOAPVERSION);
        envelope.bodyOut = input;
        envelope.dotNet = true;
        envelope.setOutputSoapObject(input);
        
        // Call the method.
        HttpTransportSE trans = new HttpTransportSE(S_URL, TIMEOUT);        
        trans.call(PostRecord.getSoapAction(), envelope);
		
        // receive the result.
        if (envelope.bodyIn instanceof SoapFault)
        {
            SoapFault fault = (SoapFault)envelope.bodyIn;
            throw new Exception(fault.faultstring);
        }   
        
        SoapObject result = (SoapObject) envelope.bodyIn;         
		
        // generate the response.
        PostRecordResponse response = new PostRecordResponse();
        response.Parse(result);

		return response;
	}
	
	public static class UserLoginResponse
	{
		public void Parse(SoapObject response)
		{
		}
	}
	
	
	public static class UserLogin
	{
		protected static final String METHOD = "UserLogin";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected String localDevId;
		protected int localPlatform;
		protected int localClientVersion;
		protected String localInfo;

		public String getDevId() {
			return localDevId;
		}

		public void setDevId(String param) {
			this.localDevId = param;
		}

		public int getPlatform() {
			return localPlatform;
		}

		public void setPlatform(int param) {
			this.localPlatform = param;
		}

		public int getClientVersion() {
			return localClientVersion;
		}

		public void setClientVersion(int param) {
			this.localClientVersion = param;
		}

		public String getInfo() {
			return localInfo;
		}

		public void setInfo(String param) {
			this.localInfo = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("devId", localDevId);
			output.addProperty("platform", localPlatform);
			output.addProperty("clientVersion", localClientVersion);
			output.addProperty("info", localInfo);
			
			return output;
		}
	}

	public static class GetLatestSoftwareVersion
	{
		protected static final String METHOD = "GetLatestSoftwareVersion";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}		
		
		protected int localPlatform;
		
		public int getPlatform() {
			return localPlatform;
		}

		public void setPlatform(int param) {
			this.localPlatform = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("platform", localPlatform);
			
			return output;
		}
	}

	public static class GetLatestSoftwareVersionResponse {

		protected int localVersion;
		protected boolean localForceUpgradingRequired;

		public int getVersion() {
			return localVersion;
		}

		public void setVersion(int param) {
			this.localVersion = param;
		}

		public boolean getForceUpgradingRequired() {
			return localForceUpgradingRequired;
		}

		public void setForceUpgradingRequired(boolean param) {
			this.localForceUpgradingRequired = param;
		}
		
		public void Parse(SoapObject response)
		{
			localVersion = Integer.parseInt(response.getPropertyAsString("version"));
			localForceUpgradingRequired = Boolean.parseBoolean(response.getPropertyAsString("forceUpgradingRequired"));
		}
	}

	public static class PostFeedbackResponse
	{
		public void Parse(SoapObject response)
		{
		}
	}
	
	
	public static class PostFeedback
	{
		protected static final String METHOD = "PostFeedback";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected String feedback;

		public String getFeedback() {
			return feedback;
		}

		public void setFeedback(String param) {
			this.feedback = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("feedback", feedback);
			
			return output;
		}
	}
	
	public static class PostRecordResponse
	{
		public void Parse(SoapObject response)
		{
		}
	}
	
	
	public static class PostRecord
	{
		protected static final String METHOD = "PostRecord";
		
		public static String getSoapAction()
		{
			return S_ACTIONPREFIX + METHOD;
		}
		
		protected String record;

		public String getRecord() {
			return record;
		}

		public void setRecord(String param) {
			this.record = param;
		}
		
		public SoapObject GetSoapObject()
		{
			SoapObject output = new SoapObject(S_NAMESPACE, METHOD);
			output.addProperty("record", record);
			
			return output;
		}
	}
}
