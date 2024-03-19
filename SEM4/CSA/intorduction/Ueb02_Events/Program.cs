// See https://aka.ms/new-console-template for more information

public delegate void MyEventHandler(object send, MyEventArgs e);


public class MyEventArgs : EventArgs
{
    public string EventData { get; }
    public MyEventArgs(string data)
    {
        this.EventData = data;
    }
}

public class EventProducer
{
    public event MyEventHandler MyEvent;

    public void OnMyEvent(string data)
    {
        if (MyEvent != null)
            MyEvent(this, new MyEventArgs(data));
    }
}


public class EventConsumer
{
    
}