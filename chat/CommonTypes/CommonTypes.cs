using System;

namespace RemotingSample
{
	public class MyRemoteObject : MarshalByRefObject  {
    
    public string MetodoOla() {
      return "ola!";
		}
  }

  public interface ICServer {

    string name();

  }
}