using System;

namespace RemotingSample
{
	public class MyRemoteObject : MarshalByRefObject  {
    
    public string MetodoOla() {
      return "ola!";
		}
  }

  public interface IChat {

    string name();
    
  }

  //Classe unica do servidor
  //os clientes enviam mensagens para o servidor atraves desta interface
  public interface IServerChat{

    //ping message
    string Ping();

    //adiciona cliente a uma lista de clientes
    void AddUser(string nick, string url);

    //recebe mensagem de um cliente e envia para todos os outros clientes
    //usando a interface IClientChat de cada cliente
    void SendServer(string nick, string message);

  }

  //classe dos clientes, uma instancia por cliente
  //o servidor usa esta interface para enviar mensagens aos clientes
  public interface IClientChat{

    //recebe mensagem do servidor e imprime no UI
    void SendClient(string message);

  }
}