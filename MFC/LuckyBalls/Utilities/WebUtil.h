//int main()
//{
//http_send h;
//string data = h.getstringbyurl(http://wap.baidu.com);
//}

#include <string>
#include <map>
#include <winsock2.h>

#pragma comment(lib ,"ws2_32.lib")

using std::string;
using std::map;

#pragma once
#define http_exception(x) x;

class socket_data
{
	public:
	socket_data()
	{
		m_s = socket(AF_INET,SOCK_STREAM,getprotobyname("tcp")->p_proto);
		if(m_s == INVALID_SOCKET)
		throw http_exception("��ʼ���׽���ʧ�ܣ�");
	}
	~socket_data()
	{
		if(m_s == NULL)
		return;
		if(closesocket(m_s) == SOCKET_ERROR)
		throw http_exception("�ر��׽���ʧ�ܣ�");
	}
	SOCKET m_s;
};

class http_head
{
	public:
	string vresoin;
	string message;
	int code;
	map<string,string> m_data;
};

class WebUtil 
{
public:
	WebUtil();
	~WebUtil();

	void recv_data(SOCKET s , string& data,char _c);
	int recv_head();
	bool _connect(string host);
	bool get_by_url(string url);
	string getstringbyurl(string url);

protected:
	http_head* m_head;
	map<string,socket_data*> m_sockets;
	map<string,socket_data*>::iterator m_value;
};
