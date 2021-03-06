﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.VisualStudio.ServiceReference.Platforms, version 11.0.50727.1
// 
namespace LuckyBallsData.DBSQLService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DBSQLService.ISqlService")]
    public interface ISqlService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetLotteryData", ReplyAction="http://tempuri.org/ISqlService/GetLotteryDataResponse")]
        System.Threading.Tasks.Task<string> GetLotteryDataAsync(int issue);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetAllLotteries", ReplyAction="http://tempuri.org/ISqlService/GetAllLotteriesResponse")]
        System.Threading.Tasks.Task<string> GetAllLotteriesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetLotteryCount", ReplyAction="http://tempuri.org/ISqlService/GetLotteryCountResponse")]
        System.Threading.Tasks.Task<int> GetLotteryCountAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetLotteriesByIndex", ReplyAction="http://tempuri.org/ISqlService/GetLotteriesByIndexResponse")]
        System.Threading.Tasks.Task<string> GetLotteriesByIndexAsync(int index_from, int index_to);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetLotteriesByIssue", ReplyAction="http://tempuri.org/ISqlService/GetLotteriesByIssueResponse")]
        System.Threading.Tasks.Task<string> GetLotteriesByIssueAsync(int issue_from, int issue_to);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetDataVersion", ReplyAction="http://tempuri.org/ISqlService/GetDataVersionResponse")]
        System.Threading.Tasks.Task<string> GetDataVersionAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetAttributesTemplate", ReplyAction="http://tempuri.org/ISqlService/GetAttributesTemplateResponse")]
        System.Threading.Tasks.Task<string> GetAttributesTemplateAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetLatestAttributes", ReplyAction="http://tempuri.org/ISqlService/GetLatestAttributesResponse")]
        System.Threading.Tasks.Task<string> GetLatestAttributesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetLatestReleaseInfo", ReplyAction="http://tempuri.org/ISqlService/GetLatestReleaseInfoResponse")]
        System.Threading.Tasks.Task<string> GetLatestReleaseInfoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetMatrixTableItem", ReplyAction="http://tempuri.org/ISqlService/GetMatrixTableItemResponse")]
        System.Threading.Tasks.Task<string> GetMatrixTableItemAsync(int candidateCount, int selectCount);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetHelp", ReplyAction="http://tempuri.org/ISqlService/GetHelpResponse")]
        System.Threading.Tasks.Task<string> GetHelpAsync();
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetVersion", ReplyAction="http://tempuri.org/ISqlService/GetVersionResponse")]
        System.Threading.Tasks.Task<LuckyBallsData.DBSQLService.GetVersionResponse> GetVersionAsync(LuckyBallsData.DBSQLService.GetVersionRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/RegisterUserChannel", ReplyAction="http://tempuri.org/ISqlService/RegisterUserChannelResponse")]
        System.Threading.Tasks.Task RegisterUserChannelAsync(string userId, string channelUri, string platform);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetUserChannels", ReplyAction="http://tempuri.org/ISqlService/GetUserChannelsResponse")]
        System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<string>> GetUserChannelsAsync(string platform);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/RememberUserLastLoginDate", ReplyAction="http://tempuri.org/ISqlService/RememberUserLastLoginDateResponse")]
        System.Threading.Tasks.Task RememberUserLastLoginDateAsync(string userId);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetLatestSoftwareVersion", ReplyAction="http://tempuri.org/ISqlService/GetLatestSoftwareVersionResponse")]
        System.Threading.Tasks.Task<LuckyBallsData.DBSQLService.GetLatestSoftwareVersionResponse> GetLatestSoftwareVersionAsync(LuckyBallsData.DBSQLService.GetLatestSoftwareVersionRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISqlService/GetReleaseNotes", ReplyAction="http://tempuri.org/ISqlService/GetReleaseNotesResponse")]
        System.Threading.Tasks.Task<string> GetReleaseNotesAsync(int fromVersion);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetVersion", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetVersionRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int version;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public long revisions;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public int latestIssue;
        
        public GetVersionRequest() {
        }
        
        public GetVersionRequest(int version, long revisions, int latestIssue) {
            this.version = version;
            this.revisions = revisions;
            this.latestIssue = latestIssue;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetVersionResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetVersionResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int version;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public long revisions;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public int latestIssue;
        
        public GetVersionResponse() {
        }
        
        public GetVersionResponse(int version, long revisions, int latestIssue) {
            this.version = version;
            this.revisions = revisions;
            this.latestIssue = latestIssue;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetLatestSoftwareVersion", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetLatestSoftwareVersionRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int version;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public bool schemaChanged;
        
        public GetLatestSoftwareVersionRequest() {
        }
        
        public GetLatestSoftwareVersionRequest(int version, bool schemaChanged) {
            this.version = version;
            this.schemaChanged = schemaChanged;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetLatestSoftwareVersionResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetLatestSoftwareVersionResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int version;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public bool schemaChanged;
        
        public GetLatestSoftwareVersionResponse() {
        }
        
        public GetLatestSoftwareVersionResponse(int version, bool schemaChanged) {
            this.version = version;
            this.schemaChanged = schemaChanged;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISqlServiceChannel : LuckyBallsData.DBSQLService.ISqlService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SqlServiceClient : System.ServiceModel.ClientBase<LuckyBallsData.DBSQLService.ISqlService>, LuckyBallsData.DBSQLService.ISqlService {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public SqlServiceClient() : 
                base(SqlServiceClient.GetDefaultBinding(), SqlServiceClient.GetDefaultEndpointAddress()) {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_ISqlService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SqlServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(SqlServiceClient.GetBindingForEndpoint(endpointConfiguration), SqlServiceClient.GetEndpointAddress(endpointConfiguration)) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SqlServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(SqlServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SqlServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(SqlServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SqlServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Threading.Tasks.Task<string> GetLotteryDataAsync(int issue) {
            return base.Channel.GetLotteryDataAsync(issue);
        }
        
        public System.Threading.Tasks.Task<string> GetAllLotteriesAsync() {
            return base.Channel.GetAllLotteriesAsync();
        }
        
        public System.Threading.Tasks.Task<int> GetLotteryCountAsync() {
            return base.Channel.GetLotteryCountAsync();
        }
        
        public System.Threading.Tasks.Task<string> GetLotteriesByIndexAsync(int index_from, int index_to) {
            return base.Channel.GetLotteriesByIndexAsync(index_from, index_to);
        }
        
        public System.Threading.Tasks.Task<string> GetLotteriesByIssueAsync(int issue_from, int issue_to) {
            return base.Channel.GetLotteriesByIssueAsync(issue_from, issue_to);
        }
        
        public System.Threading.Tasks.Task<string> GetDataVersionAsync() {
            return base.Channel.GetDataVersionAsync();
        }
        
        public System.Threading.Tasks.Task<string> GetAttributesTemplateAsync() {
            return base.Channel.GetAttributesTemplateAsync();
        }
        
        public System.Threading.Tasks.Task<string> GetLatestAttributesAsync() {
            return base.Channel.GetLatestAttributesAsync();
        }
        
        public System.Threading.Tasks.Task<string> GetLatestReleaseInfoAsync() {
            return base.Channel.GetLatestReleaseInfoAsync();
        }
        
        public System.Threading.Tasks.Task<string> GetMatrixTableItemAsync(int candidateCount, int selectCount) {
            return base.Channel.GetMatrixTableItemAsync(candidateCount, selectCount);
        }
        
        public System.Threading.Tasks.Task<string> GetHelpAsync() {
            return base.Channel.GetHelpAsync();
        }
        
        public System.Threading.Tasks.Task<LuckyBallsData.DBSQLService.GetVersionResponse> GetVersionAsync(LuckyBallsData.DBSQLService.GetVersionRequest request) {
            return base.Channel.GetVersionAsync(request);
        }
        
        public System.Threading.Tasks.Task RegisterUserChannelAsync(string userId, string channelUri, string platform) {
            return base.Channel.RegisterUserChannelAsync(userId, channelUri, platform);
        }
        
        public System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<string>> GetUserChannelsAsync(string platform) {
            return base.Channel.GetUserChannelsAsync(platform);
        }
        
        public System.Threading.Tasks.Task RememberUserLastLoginDateAsync(string userId) {
            return base.Channel.RememberUserLastLoginDateAsync(userId);
        }
        
        public System.Threading.Tasks.Task<LuckyBallsData.DBSQLService.GetLatestSoftwareVersionResponse> GetLatestSoftwareVersionAsync(LuckyBallsData.DBSQLService.GetLatestSoftwareVersionRequest request) {
            return base.Channel.GetLatestSoftwareVersionAsync(request);
        }
        
        public System.Threading.Tasks.Task<string> GetReleaseNotesAsync(int fromVersion) {
            return base.Channel.GetReleaseNotesAsync(fromVersion);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync() {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync() {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_ISqlService)) {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_ISqlService)) {
                return new System.ServiceModel.EndpointAddress("http://dbdataquery.cloudapp.net/DBSqlService.svc");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding() {
            return SqlServiceClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_ISqlService);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress() {
            return SqlServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_ISqlService);
        }
        
        public enum EndpointConfiguration {
            
            BasicHttpBinding_ISqlService,
        }
    }
}
