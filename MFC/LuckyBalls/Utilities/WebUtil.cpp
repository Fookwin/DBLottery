#include "stdafx.h"
#include "WebUtil.h"

#define MAXHEADERSIZE 1024

WebUtil::WebUtil()
{
	int err;
	WORD wVersion;
	WSADATA WSAData;
	wVersion=MAKEWORD(2,2);
	err=WSAStartup(wVersion,&WSAData);
	if(err!=0)
	{
		throw http_exception("装载Socket库失败！");
	}
	if(LOBYTE( WSAData.wVersion ) != 2)
	{
		WSACleanup();
		throw http_exception("无法找到合适的Socket库.");
	}
	m_head = nullptr;
}

WebUtil::~WebUtil()
{
	WSACleanup();
	map<string,socket_data*>::iterator first = m_sockets.begin();
	map<string,socket_data*>::iterator last = m_sockets.end();
	while(first++ != last)
		delete first->second;
}

bool WebUtil::_connect(string host)
{
	if(host.empty())
		return FALSE;

	int pos = (int)host.find(':');
	int port;
	hostent* m_phostent;
	if(pos == -1)
	{
		port = 80;
		m_phostent = gethostbyname( host.c_str() );
	}
	else
	{
		port = atoi(host.substr(pos).c_str());
		m_phostent = gethostbyname( host.substr(0 , pos).c_str() );
	}

	if(m_phostent == NULL)
	{
		return FALSE;
	}

	std::pair<map<string,socket_data*>::iterator , bool> temp = m_sockets.insert(make_pair(host , nullptr));
	if(temp.second)
		temp.first->second = new socket_data;

	m_value = temp.first;
	struct sockaddr_in destaddr;
	memset((void *)&destaddr,0,sizeof(destaddr)); 
	destaddr.sin_family=AF_INET;
	destaddr.sin_port=htons(80);
	memcpy(&destaddr.sin_addr,m_phostent->h_addr_list[0],4);
	if(connect(m_value->second->m_s,(struct sockaddr*)&destaddr,sizeof(destaddr))!=0)
	{
		delete m_value->second;
		m_value->second = NULL;
		m_sockets.erase(m_value);
		return FALSE;
	}
	return TRUE;
}

inline void WebUtil::recv_data(SOCKET s , string& data,char _c)
{
	char c;
	while(recv(s , &c , 1 , 0))
	{
		if(c == _c)
		break;
		data.push_back(c);
	}
}

int WebUtil::recv_head()
{
	SOCKET s = m_value->second->m_s;
	char c;
	string temp1,temp2;
	if(m_head != nullptr)
	delete m_head;
	m_head = new http_head;
	recv_data(s , m_head->vresoin , ' ');
	recv_data(s , temp1 , ' ');
	m_head->code = atoi(temp1.c_str());
	temp1.clear();
	recv_data(s , m_head->message ,'\r');
	recv(s , &c , 1 , 0);
	bool name = true;
	while(recv(s , &c , 1 , 0))
	{
		if(c == '\r')
		{
			recv(s , &c , 1 , 0);
			if(temp1.empty())
			break;
			m_head->m_data[temp1] = temp2;
			temp1.clear();
			temp2.clear();
			name = true;
		}
		else if(c == ':')
		{
			recv(s , &c , 1 , 0);
			name = false;
		}
		else
		{
			if(name)
			temp1.push_back(c);
			else
			temp2.push_back(c);
		}
	}
	return 0;
}

inline bool WebUtil::get_by_url(string url)
{
	if(url.compare( 0 , 7 ,"http://"))
		throw http_exception("错误的URL格式！");

	int pos = (int)url.find('/',7);
	string host;
	string data;
	if(pos == -1)
	{
		host = url.substr(7);
		data = "/";
	}
	else
	{
		host = url.substr(7 ,pos - 7);
		data = url.substr(pos);
	}
	_connect(host);
	string send_data = "GET ";
	send_data += data;
	send_data += " HTTP/1.1\r\nHost:";
	send_data += host;
	send_data += "\r\nConnection:Keep-Alive\r\n\r\n";
	if(send(m_value->second->m_s,send_data.c_str(), (int)send_data.size(),0)==SOCKET_ERROR)
	{
		return FALSE;
	}
	recv_head();
	return TRUE;
}

string WebUtil::getstringbyurl(string url)
{
	get_by_url(url);
	int len = atoi(m_head->m_data["Content-Length"].c_str());
	char* c = new char[len];
	recv(m_value->second->m_s ,c ,len ,0);
	string temp(c);
	return temp;
}